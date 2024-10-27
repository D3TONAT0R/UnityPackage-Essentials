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
		const string TEMPLATE_ARCHIVE_PATH = "Packages/UnityPackage-Essentials/Editor/BuildProcessors/MacOSExecutableTemplate.zip";
		const string EXECUTABLE_NAME = "TemplateExecutable";

		public int callbackOrder => 0;

		public void OnPostprocessBuild(BuildReport report)
		{
			if(report.summary.platform == BuildTarget.StandaloneOSX)
			{
				var rootPath = report.summary.outputPath;
				var executableFilePath = rootPath + "/Contents/MacOS/" + PlayerSettings.productName;
				//Copy contents of the executable file
				var builtData = File.ReadAllBytes(executableFilePath);
				File.Delete(executableFilePath);
				//Unzip archive containing the executable file with the x attribute set
				ZipFile.ExtractToDirectory(TEMPLATE_ARCHIVE_PATH, rootPath + "/Contents/MacOS/");
				File.Move(rootPath + "/Contents/MacOS/" + EXECUTABLE_NAME, executableFilePath);
				//Rewrite contents of the executable file
				File.WriteAllBytes(executableFilePath, builtData);
				Debug.Log("MacOS executable successfully fixed.");
			}
		}
	} 
}