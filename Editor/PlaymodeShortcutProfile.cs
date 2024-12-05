using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public static class PlaymodeShortcutProfile
	{
		private static string FilePath => Path.Combine(Directory.GetParent(Application.dataPath).FullName, "LastProfile.tmp");

		[InitializeOnLoadMethod]
		public static void Init()
		{
			EditorApplication.playModeStateChanged += OnChange;
		}

		private static void OnChange(PlayModeStateChange change)
		{
			string profileName = EssentialsProjectSettings.Instance.playmodeShortcutProfileName;
			if(string.IsNullOrWhiteSpace(profileName)) return;
			if(change == PlayModeStateChange.EnteredPlayMode)
			{
				if(!new List<string>(ShortcutManager.instance.GetAvailableProfileIds()).Contains(profileName))
				{
					Debug.LogWarning("Could not find shortcuts profile for playmode: " + profileName);
					return;
				}
				StoreEditModeProfile();
				ShortcutManager.instance.activeProfileId = profileName;
			}
			else if(change == PlayModeStateChange.EnteredEditMode)
			{
				RestoreEditModeProfile();
			}
		}

		private static void StoreEditModeProfile()
		{
			if(!File.Exists(FilePath))
			{
				File.WriteAllText(FilePath, ShortcutManager.instance.activeProfileId);
			}
			else
			{
				Debug.LogWarning("Temporary shortcut profile file already exists.");
			}
		}

		private static void RestoreEditModeProfile()
		{
			if(File.Exists(FilePath))
			{
				ShortcutManager.instance.activeProfileId = File.ReadAllText(FilePath);
				File.Delete(FilePath);
			}
			else
			{
				Debug.LogWarning("Could not find last shortcut profile information.");
			}
		}
	}
}
