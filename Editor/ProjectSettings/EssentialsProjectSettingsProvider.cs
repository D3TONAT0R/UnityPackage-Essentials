using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
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
			EditorGUI.BeginChangeCheck();
			EssentialsProjectSettings.Instance.DrawEditorGUI();
			if(EditorGUI.EndChangeCheck())
			{
				//Sync namespace setting with editor settings
				EditorSettings.projectGenerationRootNamespace = EssentialsProjectSettings.Instance.GetScriptRootNamespace() ?? "";
			}
			GUILayout.Space(10);
			provider.Repaint();
		}

		private static void OnClose()
		{
			EssentialsProjectSettings.Instance.EditorSave();
		}
	}
}
