using System;
using System.IO;
using UnityEssentials;
using UnityEngine;

namespace UnityEssentialsEditor.TimeTracking
{
	[System.Serializable]
	public class TrackedUserTimes
	{
		public readonly string userId;
		public SessionTimers storedSample;
		public SessionTimers currentSessionSample;

		public bool FailedToLoad { get; private set; }

		public bool IsDirty => currentSessionSample.CombinedTime > 0;

		public bool IsAnon => string.IsNullOrEmpty(userId) || userId.ToLower() == "anonymous";

		public float TotalCurrentSessionTime => currentSessionSample.GetTotal(TrackedTimeType.All);

		public string FileLocation => Path.Combine(EditorTimeTracker.FileRootDirectory, userId + ".json");

		private TrackedUserTimes(string userId)
		{
			this.userId = userId;
		}

		public static TrackedUserTimes Create(string userId)
		{
			var user = new TrackedUserTimes(userId);
			user.Initialize();
			return user;
		}

		public void Initialize()
		{
			LoadExistingTimes();
			currentSessionSample = new SessionTimers();
		}

		public void Increase(float delta)
		{
			currentSessionSample.Increase(delta);
		}

		public void LoadExistingTimes()
		{
			if(!Directory.Exists(EditorTimeTracker.FileRootDirectory))
			{
				Directory.CreateDirectory(EditorTimeTracker.FileRootDirectory);
			}
			string filePath = FileLocation;
			if(File.Exists(filePath))
			{
				try
				{
					var json = File.ReadAllText(FileLocation);
					storedSample = JsonUtility.FromJson<SessionTimers>(json);
					FailedToLoad = false;
				}
				catch(Exception e)
				{
					e.LogException("Failed to load editor time data for user "+userId);
					FailedToLoad = true;
				}
			}
			else
			{
				try
				{
					storedSample = new SessionTimers();
					var json = JsonUtility.ToJson(storedSample);
					File.WriteAllText(FileLocation, json);
				}
				catch(Exception e)
				{
					e.LogException("Failed to create editor time data for user " + userId);
				}
				FailedToLoad = false;
			}
		}

		public void SaveToFile()
		{
			try
			{
				if(!Directory.Exists(EditorTimeTracker.FileRootDirectory))
				{
					Directory.CreateDirectory(EditorTimeTracker.FileRootDirectory);
				}
				if(FailedToLoad)
				{
					LoadExistingTimes();
				}
				if(FailedToLoad) return;

				var totals = SessionTimers.Combine(currentSessionSample, storedSample);
				var json = JsonUtility.ToJson(totals, true);
				File.WriteAllText(FileLocation, json);

				//Restart timers
				storedSample = totals;
				currentSessionSample = new SessionTimers();
			}
			catch(Exception e)
			{
				e.LogException("Failed to save editor time data for user " + userId);
			}
		}

		public float GetTotalTime(TrackedTimeType typeFlags = TrackedTimeType.All)
		{
			if(FailedToLoad)
			{
				LoadExistingTimes();
			}
			if(FailedToLoad) return 0;
			float t = 0;
			if(storedSample != null) t += storedSample.GetTotal(typeFlags);
			t += currentSessionSample.GetTotal(typeFlags);
			return t;
		}
	}
}
