using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityEssentials
{
	public abstract class ProjectSettingsAsset<T> : ScriptableObject where T : ProjectSettingsAsset<T>
	{
		public static T Instance
		{
			get
			{
				if(!instance) instance = CreateInstance<T>();
				return instance;
			}
		}
		private static T instance;
		
		/// <summary>
		/// The name of the asset file in the ProjectSettings folder without the .asset extension. This is used to determine the path to the asset file.
		/// </summary>
		public virtual string ProjectAssetName => typeof(T).Name;

		/// <summary>
		/// The full path to the asset file in the ProjectSettings folder, including the .asset extension.
		/// </summary>
		public string ProjectAssetPath => Path.Combine("ProjectSettings", ProjectAssetName + ".asset");

		/// <summary>
		/// Called when the settings asset is initialized.
		/// </summary>
		protected void Awake()
		{
#if UNITY_EDITOR
			if(!File.Exists(ProjectAssetPath))
			{
				OnCreateNewSettings();
				EditorSave();
			}
			else
			{
				EditorLoad(File.ReadAllText(ProjectAssetPath));
			}
			Initialize();
#endif
		}

		/// <summary>
		/// Called when the settings asset was just created for the first time.
		/// </summary>
		protected virtual void OnCreateNewSettings()
		{
		}

		/// <summary>
		/// Called when the settings asset is initialized.
		/// </summary>
		protected virtual void Initialize()
		{
		}

#if UNITY_EDITOR
		/// <summary>
		/// Loads the settings from the specified JSON string. Called when the settings asset is loaded from disk.
		/// </summary>
		public virtual void EditorLoad(string json)
		{
			UnityEditor.EditorJsonUtility.FromJsonOverwrite(json, this);
		}

		/// <summary>
		/// Called before the settings asset is serialized to JSON.
		/// </summary>
		protected virtual void OnBeforeEditorSerialize()
		{
		}

		/// <summary>
		/// Saves the settings asset to disk as a JSON file. Called when the settings asset is modified in the editor.
		/// </summary>
		public virtual void EditorSave()
		{
			OnBeforeEditorSerialize();
			string json = UnityEditor.EditorJsonUtility.ToJson(this, true);
			File.WriteAllText(ProjectAssetPath, json);
		}

		/// <summary>
		/// Draws the editor GUI for the settings asset.
		/// </summary>
		public virtual void DrawEditorGUI()
		{
			var obj = BeginEditorGUI();
			DrawEditorProperties(obj);
			EndEditorGUI(obj);
		}

		/// <summary>
		/// Begins the editor GUI for the settings asset and returns a SerializedObject for the asset.
		/// </summary>
		/// <returns></returns>
		protected UnityEditor.SerializedObject BeginEditorGUI()
		{
			UnityEditor.EditorGUIUtility.labelWidth = 250;
			return new UnityEditor.SerializedObject(this);
		}

		/// <summary>
		/// Draws the properties of the settings asset in the editor GUI using the specified SerializedObject.
		/// </summary>
		/// <param name="obj"></param>
		protected void DrawEditorProperties(UnityEditor.SerializedObject obj)
		{
			var prop = obj.GetIterator();
			obj.Update();
			prop.NextVisible(true);
			while(prop.NextVisible(false))
			{
				UnityEditor.EditorGUILayout.PropertyField(prop);
			}
		}

		/// <summary>
		/// Ends the editor GUI for the settings asset and applies any modified properties to the asset. If any properties were modified, the asset is saved to disk.
		/// </summary>
		protected void EndEditorGUI(UnityEditor.SerializedObject obj)
		{
			if(obj.ApplyModifiedProperties())
			{
				EditorSave();
			}
		}
#endif
	}
}