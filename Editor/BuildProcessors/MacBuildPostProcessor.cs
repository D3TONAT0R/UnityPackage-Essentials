using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace D3TEditor.BuildProcessors
{
#if UNITY_6000_0_OR_NEWER
	public class MacBuildPostProcessor : IPostprocessBuildWithReport
	{
		const int WHATEVER_BIT = 1 << 31;
		const int X_OWNER_BIT = 1 << 22;
		const int X_GROUP_BIT = 1 << 19;
		const int X_OTHER_BIT = 1 << 16;
		const int ALL_X_BITS = X_OWNER_BIT | X_GROUP_BIT | X_OTHER_BIT;
		const uint UNIX_FLAGS = 0b10000001111011010000000000000000;
		//						  \------/\------/\------/\------/
		//-rwxr-xr-x
		//No X:	0x81a40000
		//X: 	0x81ed0000
		const uint FINAL_UNIX_FLAGS = 0x81FF0000;

		public int callbackOrder => int.MaxValue;

		public void OnPostprocessBuild(BuildReport report)
		{
			if(report.summary.platform == BuildTarget.StandaloneOSX)
			{
				var rootPath = report.summary.outputPath;
				var executableFilePath = "Contents/MacOS/" + PlayerSettings.productName;
				//Clear the executable file
				//File.WriteAllBytes(rootPath + "/" + executableFilePath, Array.Empty<byte>());

				//Delete existing zip if present
				if(File.Exists(rootPath + ".zip"))
				{
					File.Delete(rootPath + ".zip");
				}
				//Compress executable into a zip file
				ZipFile.CreateFromDirectory(rootPath, rootPath + ".zip", System.IO.Compression.CompressionLevel.NoCompression, true);

				//RemoveOtherStuff(rootPath + ".zip");

				//File.Copy(rootPath + ".zip", rootPath + "-original.zip", true);
				//Modify zip to set the x flag
				using(var zip = ZipFile.Open(rootPath + ".zip", ZipArchiveMode.Update))
				{
					var entry = zip.GetEntry(executableFilePath);
					unchecked
					{
						int i = (int)FINAL_UNIX_FLAGS;
						entry.ExternalAttributes |= i;
					}
				}
				
				//Self test
				//PerformAttributeTest(rootPath, executableFilePath);

				//Delete original build directory
				//Directory.Delete(rootPath, true);
			}
		}

		private static void PerformAttributeTest(string rootPath, string executableFilePath)
		{
			using(var zip = ZipFile.OpenRead(rootPath + ".zip"))
			{
				//var zipNoX = ZipFile.OpenRead(rootPath + "-no-x.zip");
				//var zipX = ZipFile.OpenRead(rootPath + "-x.zip");
				var attributes = zip.GetEntry(executableFilePath).ExternalAttributes;
				bool test = attributes == FINAL_UNIX_FLAGS;
				/*
				test &= (attributes & X_OWNER_BIT) == X_OWNER_BIT;
				test &= (attributes & X_GROUP_BIT) == X_GROUP_BIT;
				test &= (attributes & X_OTHER_BIT) == X_OTHER_BIT;
				*/
				if(!test) Debug.LogError("Unix perms test failed: "+Convert.ToString(zip.GetEntry(executableFilePath).ExternalAttributes, 2));
			}
		}

		private static void RemoveOtherStuff(string path)
		{
			using var zip = ZipFile.Open(path, ZipArchiveMode.Update);
			foreach(var entry in zip.Entries.ToArray())
			{
				if(entry.FullName.StartsWith("Contents/") && !entry.FullName.StartsWith("Contents/MacOS/"))
				{
					entry.Delete();
				}
			}
		}
	} 
#endif
}