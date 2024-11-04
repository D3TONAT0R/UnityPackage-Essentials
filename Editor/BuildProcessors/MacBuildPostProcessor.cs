using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Assertions;

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
		const uint UNIX_FLAGS_X   = 0b10000001111011010000000000000000;
		const uint UNIX_FLAGS_ALL = 0b10000001111111110000000000000000;
		const uint UNIX_FLAGS_STD = 0b10000001101001000000000000000000;
		const uint UNIX_FLAGS_B6  = 0b10000001101101100000000000000000;
		//							  _______rwxrwxrwx________________
		//						      \------/\------/\------/\------/
		//TODO: test with first bit set
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
				var buildName = Path.GetFileName(rootPath);
				var executableFilePath = $"{buildName}/Contents/MacOS/{PlayerSettings.productName}";

				//Delete existing zip if present
				if(File.Exists(rootPath + ".zip"))
				{
					File.Delete(rootPath + ".zip");
				}

				//Compress executable into a zip file
				ZipFile.CreateFromDirectory(rootPath, rootPath + ".zip", System.IO.Compression.CompressionLevel.NoCompression, true);
				//RemoveOtherStuff(rootPath + ".zip");

				int entryCount;
				//Modify zip to set the executable attributes
				using(var zip = ZipFile.Open(rootPath + ".zip", ZipArchiveMode.Update))
				{
					entryCount = zip.Entries.Count;
					//Set standard unix flags for all files and executable flags for the executable
					foreach(var entry in zip.Entries)
					{
						bool executable = entry.FullName == executableFilePath;
						SetUnixFlags(entry, executable ? UNIX_FLAGS_ALL : UNIX_FLAGS_B6);
					}
				}

				//Manual trickery to pretend that the zip was created on unix
				SetHostOS(rootPath, entryCount);

				//Self test
				CheckExecutableFlags(rootPath, executableFilePath);

				//Delete original build directory
				//Directory.Delete(rootPath, true);
			}
		}

		private static void SetHostOS(string rootPath, int entryCount)
		{
			/*
			OFFSET              Count TYPE   Description
			0000h                   4 char   ID='PK',03,04
			0004h                   1 word   Version needed to extract archive
			0006h                   1 word   General purpose bit field (bit mapped)
			*/
			const byte P = (byte)'P';
			const byte K = (byte)'K';
			//Minimum version needed to extract: 0x14 (20) = 2.0
			const byte V = 0x14;
			var bytes = File.ReadAllBytes(rootPath + ".zip");
			int modifiedEntries = 0;
			for(int i = 0; i < bytes.Length; i++)
			{
				if(i + 6 >= bytes.Length) break;
				if(bytes[i] == P && bytes[i+1] == K && bytes[i+2] == 0x01 && bytes[i+3] == 0x02 && bytes[i+4] == V)
				{
					bytes[i + 5] = 0x03;
					modifiedEntries++;
					//Approximate minimum central directory entry length
					i += 22;
				}
			}
			if(entryCount != modifiedEntries)
			{
				Debug.LogError($"Number of modified entries does not match actual entry count (expected: {entryCount}, modified: {modifiedEntries})");
			}
			else
			{
				Debug.Log($"Host OS changed successfully ({modifiedEntries} entries modified)");
			}
			File.WriteAllBytes(rootPath + ".zip", bytes);
		}

		private static void SetUnixFlags(ZipArchiveEntry entry, uint flags)
		{
			unchecked
			{
				int i = (int)flags;
				entry.ExternalAttributes |= i;
			}
		}

		private static void CheckExecutableFlags(string rootPath, string executableFilePath)
		{
			using(var zip = ZipFile.OpenRead(rootPath + ".zip"))
			{
				//var zipNoX = ZipFile.OpenRead(rootPath + "-no-x.zip");
				//var zipX = ZipFile.OpenRead(rootPath + "-x.zip");
				var attributes = zip.GetEntry(executableFilePath).ExternalAttributes;
				bool test;
				unchecked
				{
					test = ((uint)attributes & UNIX_FLAGS_ALL) == UNIX_FLAGS_ALL;
				}
				/*
				test &= (attributes & X_OWNER_BIT) == X_OWNER_BIT;
				test &= (attributes & X_GROUP_BIT) == X_GROUP_BIT;
				test &= (attributes & X_OTHER_BIT) == X_OTHER_BIT;
				*/
				string base2 = Convert.ToString(zip.GetEntry(executableFilePath).ExternalAttributes, 2).PadLeft(32, '0');
				if(!test) Debug.LogError("Unix perms test failed: "+base2);
				else Debug.Log("Unix perms test passed: " + base2);
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