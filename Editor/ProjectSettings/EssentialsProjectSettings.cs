using UnityEngine;
using UnityEssentials;
using UnityEditor;
using System.IO;

namespace UnityEssentialsEditor
{
	public class EssentialsProjectSettings : ScriptableObject
	{
		private static string ProjectRootPath => Directory.GetParent(Application.dataPath).ToString();
		private static string SettingsAssetPath => Path.Combine(ProjectRootPath, "ProjectSettings", "EssentialsProjectSettings.asset");

		public static EssentialsProjectSettings Instance
		{
			get
			{
				if(instance == null) InitSettings();
				return instance;
			}
		}
		private static EssentialsProjectSettings instance;

		public bool removeDefaultScriptMenu = true;
		public string defaultScriptNamespace = "MyNamespace";
		#if UNITY_2020_2_OR_NEWER
		[NonReorderable]
		#endif
		public string[] additionalDefaultUsings = new string[0];

		[Space(20)]
		public bool enableEditorTimeTracking = true;

		private static void InitSettings()
		{
			if(File.Exists(SettingsAssetPath))
			{
				instance = CreateInstance<EssentialsProjectSettings>();
				string json = File.ReadAllText(SettingsAssetPath);
				JsonUtility.FromJsonOverwrite(json, instance);
			}
			else
			{
				Debug.Log("Essentials Package: Creating new project settings ...");
				instance = CreateInstance<EssentialsProjectSettings>();
			}
		}

		public void Validate()
		{
			
		}

		public void SaveModifiedProperties()
		{
			Validate();
			string json = JsonUtility.ToJson(this, true);
			File.WriteAllText(SettingsAssetPath, json);
		}
	}
}
