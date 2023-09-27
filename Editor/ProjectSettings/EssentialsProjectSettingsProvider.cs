using UnityEngine;
using D3T;
using UnityEditor;
using System;
using D3TEditor.TimeTracking;

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
			};
			provider.deactivateHandler += OnClose;
			return provider;
		}

		private static void OnGUI(string search)
		{
			EditorGUIUtility.labelWidth = 250;
			var so = new SerializedObject(EssentialsProjectSettings.Instance);
			var prop = so.GetIterator();
			prop.NextVisible(true);
			so.Update();
			while(prop.NextVisible(false))
			{
				EditorGUILayout.PropertyField(prop);
			}
			if(so.ApplyModifiedProperties())
			{
				EssentialsProjectSettings.Instance.SaveModifiedProperties();
			}
			GUILayout.Space(20);
			EditorTimeTrackingGUI.DrawGUI("Tracked Editor Times");
			provider.Repaint();
		}

		private static void OnClose()
		{
			EssentialsProjectSettings.Instance.SaveModifiedProperties();
		}
	}
}
