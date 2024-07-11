using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public static class PlaymodeEditorKeybinds
	{
		private static string FilePath => Path.Combine(Directory.GetParent(Application.dataPath).FullName, "LastProfile.tmp");

		private const string playmodeProfile = "Playmode";

		[InitializeOnLoadMethod]
		public static void Init()
		{
			EditorApplication.playModeStateChanged += OnChange;
		}

		private static void OnChange(PlayModeStateChange change)
		{
			if(!new List<string>(ShortcutManager.instance.GetAvailableProfileIds()).Contains(playmodeProfile)) return;
			if(change == PlayModeStateChange.EnteredPlayMode)
			{
				StoreEditModeProfile();
				ShortcutManager.instance.activeProfileId = playmodeProfile;
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
				Debug.LogError("Failed to load last edit mode shortcut profile, reverting to default profile.");
				ShortcutManager.instance.activeProfileId = ShortcutManager.defaultProfileId;
			}
		}
	}
}
