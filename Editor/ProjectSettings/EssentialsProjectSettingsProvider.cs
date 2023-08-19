using UnityEngine;
using D3T;
using UnityEditor;
using System;

namespace D3TEditor
{
	internal static class EssentialsProjectSettingsProvider
	{
		[SettingsProvider]
		internal static SettingsProvider Register()
		{
			var provider = new SettingsProvider("Project/Essentials", SettingsScope.Project)
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
		}

		private static void OnClose()
		{
			EssentialsProjectSettings.Instance.SaveModifiedProperties();
		}
	}
}
