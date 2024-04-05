using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;

namespace D3TEditor
{
	public static class RunBuildButtons
	{
		[InitializeOnLoadMethod]
		private static void Init()
		{
			EditorApplication.delayCall += () =>
			{
				string[] applications;
#if UNITY_EDITOR_WIN
				applications = Directory.GetFiles("Builds/", "*.exe", SearchOption.AllDirectories);
#else
				applications = Directory.GetFiles("Builds/", "*.app", SearchOption.AllDirectories);
#endif
				foreach(var app in applications)
				{
					var filename = Path.GetFileName(app);
					if(filename.StartsWith("UnityCrashHandler")) continue;
					string folderName = Path.GetFileName(Path.GetDirectoryName(app));
					string appPath = app;
					MenuUtility.AddMenuItem("File/Run Build/"+folderName, null, 220, () => RunExe(appPath));
				}
			};
		}

		private static void RunExe(string exe)
		{
			Process.Start(exe);
		}
	}
}
