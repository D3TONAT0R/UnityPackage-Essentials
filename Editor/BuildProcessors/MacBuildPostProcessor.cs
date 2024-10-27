using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace D3TEditor.BuildProcessors
{
	public class MacBuildPostProcessor : IPostprocessBuildWithReport
	{
		const int X_OWNER_BIT = 1 << 22;
		const int X_GROUP_BIT = 1 << 19;
		const int X_OTHER_BIT = 1 << 16;
		const int ALL_X_BITS = X_OWNER_BIT | X_GROUP_BIT | X_OTHER_BIT;

		public int callbackOrder => int.MaxValue;

		public void OnPostprocessBuild(BuildReport report)
		{
			if(report.summary.platform == BuildTarget.StandaloneOSX)
			{
				var rootPath = report.summary.outputPath;
				var executableFilePath = "Contents/MacOS/" + PlayerSettings.productName;

				//Delete existing zip if present
				if(File.Exists(rootPath + ".zip"))
				{
					File.Delete(rootPath + ".zip");
				}
				//Compress executable into a zip file
				ZipFile.CreateFromDirectory(rootPath, rootPath + ".zip");

				//Modify zip to set the x flag
				using(var zip = ZipFile.Open(rootPath + ".zip", ZipArchiveMode.Update))
				{
					zip.GetEntry(executableFilePath).ExternalAttributes |= ALL_X_BITS;
				}

				//Self test
				//PerformAttributeTest(rootPath, executableFilePath);

				//Delete original build directory
				Directory.Delete(rootPath, true);
			}
		}

		private static void PerformAttributeTest(string rootPath, string executableFilePath)
		{
			using(var zip = ZipFile.OpenRead(rootPath + ".zip"))
			{
				bool test = (zip.GetEntry(executableFilePath).ExternalAttributes & ALL_X_BITS) == ALL_X_BITS;
				if(test) Debug.Log("Perms test passed");
				else Debug.LogError("Perms test failed: "+Convert.ToString(zip.GetEntry(executableFilePath).ExternalAttributes, 2));
			}
		}
	} 
}