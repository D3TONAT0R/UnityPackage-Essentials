﻿using D3TEditor.TimeTracking;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	internal static class EssentialsProjectSettingsProvider
	{
		private static SettingsProvider provider;

		[SettingsProvider]
		internal static SettingsProvider Register()
		{
			provider = new SettingsProvider("Project/Essentials", SettingsScope.Project)
			{
				guiHandler = OnGUI,
				deactivateHandler = OnClose
			};
			return provider;
		}

		private static void OnGUI(string search)
		{
			EssentialsProjectSettings.Instance.DrawEditorGUI();
			GUILayout.Space(20);
			EditorTimeTrackingGUI.DrawGUI("Tracked Editor Times");
			provider.Repaint();
		}

		private static void OnClose()
		{
			EssentialsProjectSettings.Instance.EditorSave();
		}
	}
}
