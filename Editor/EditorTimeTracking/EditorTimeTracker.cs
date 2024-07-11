using UnityEssentials;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.TimeTracking
{
	public static class EditorTimeTracker
	{
		[System.Serializable]
		public class SamplesData
		{
			public List<UserTimeSample> times = new List<UserTimeSample>();
		}

		[System.Serializable]
		public class UserTimeSample
		{
			public string userId;
			public TimeSample times;
		}

		private const float MIN_SAVE_INTERVAL = 60;

		public static bool Enabled => !LoadingFailed && EssentialsProjectSettings.Instance.enableEditorTimeTracking;

		public static bool LoadingFailed { get; set; } = false;

		internal static Dictionary<string, TimeSample> times = new Dictionary<string, TimeSample>();

		private static double lastCheckTime;
		private static double lastSaveTime;

		private static string FileName => PersistentFileUtility.GetFullPath(PersistentFileUtility.FileLocation.DataPath, "EditorTimes.json");

		[InitializeOnLoadMethod]
		private static void Init()
		{
			LoadFromFile();
			EditorApplication.update += Update;
			AssemblyReloadEvents.beforeAssemblyReload += OnDestroy;
			EditorApplication.quitting += OnDestroy;
			//Save data when the project is changed
			EditorApplication.projectChanged += () => Save(false);
		}

		private static void OnDestroy()
		{
			Save(false);
		}

		private static void Update()
		{
			if(Enabled && lastCheckTime != 0)
			{
				float delta = (float)(EditorApplication.timeSinceStartup - lastCheckTime);
				string user = GetUserName();
				if(!times.ContainsKey(user))
				{
					times.Add(user, new TimeSample());
				}
				times[user].Increase(delta);
			}
			lastCheckTime = EditorApplication.timeSinceStartup;
		}

		private static string GetUserName()
		{
			string id = CloudProjectSettings.userId;
			if(!string.IsNullOrWhiteSpace(id)) return id;
			else return $"({Environment.UserName})";
		}

		private static void LoadFromFile()
		{
			LoadingFailed = false;
			if(File.Exists(FileName))
			{
				try
				{
					string json = File.ReadAllText(FileName);
					var data = JsonUtility.FromJson<SamplesData>(json);
					times.Clear();
					foreach(var d in data.times)
					{
						times.Add(d.userId, d.times);
					}
				}
				catch(Exception e)
				{
					e.LogException("Failed to read tracked editor times, disabling time tracking");
					LoadingFailed = true;
				}
			}
		}

		private static void Save(bool force)
		{
			if(EditorApplication.timeSinceStartup - lastSaveTime < MIN_SAVE_INTERVAL && !force)
			{
				//We already saved not too long ago, don't save again
				return;
			}
			SamplesData data = new SamplesData();
			foreach(var kv in times)
			{
				if(kv.Value.CombinedTime < 10f) continue;
				data.times.Add(new UserTimeSample() { userId = kv.Key, times = kv.Value });
			}
			string json = JsonUtility.ToJson(data, true);
			File.WriteAllText(FileName, json);
			lastSaveTime = EditorApplication.timeSinceStartup;
		}
	}
}
