using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace UnityEssentialsEditor
{
	public static class RunBuildButtons
	{
		private const int PRIORITY = 220;

		[InitializeOnLoadMethod]
		private static void Init()
		{
			if(EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.delayCall += () =>
				{
					int appCount = 0;
					foreach(var app in ListGameBuildsApps())
					{
						appCount++;
						string folderName = Path.GetFileName(Path.GetDirectoryName(app));
						string appPath = app;
						MenuUtility.AddMenuItem("File/Run Build/" + folderName, null, PRIORITY, () => RunBuild(appPath));
					}
					if(appCount == 0) MenuUtility.AddMenuItem("File/Run Build/No Builds Found", null, PRIORITY, null, false, () => false);
				};
			}
		}

		private static IEnumerable<string> ListGameBuildsApps()
		{
			List<string> apps = new List<string>();
#if UNITY_EDITOR_WIN
			if(Directory.Exists("Build")) apps.AddRange(Directory.GetFiles("Build/", "*.exe", SearchOption.AllDirectories));
			if(Directory.Exists("Builds")) apps.AddRange(Directory.GetFiles("Builds/", "*.exe", SearchOption.AllDirectories));
#else
			if(Directory.Exists("Build")) apps.AddRange(Directory.GetFiles("Build/", "*.app", SearchOption.AllDirectories));
			if(Directory.Exists("Builds")) apps.AddRange(Directory.GetFiles("Builds/", "*.app", SearchOption.AllDirectories));
#endif
			foreach(var app in apps)
			{
				var filename = Path.GetFileName(app);
				if(!filename.StartsWith("UnityCrashHandler"))
				{
					yield return app;
				}
			}
		}

		private static void RunBuild(string appPath)
		{
			System.Diagnostics.Process.Start(appPath);
		}
	}
}
