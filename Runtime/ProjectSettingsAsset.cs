using System.IO;
using UnityEngine;

namespace UnityEssentials
{
	public abstract class ProjectSettingsAsset : ScriptableObject
	{
		public abstract string ProjectAssetName { get; }

		public string ProjectAssetPath => Path.Combine("ProjectSettings", ProjectAssetName + ".asset");

		public static T CreateSettingsAsset<T>() where T : ProjectSettingsAsset
		{
			//Destroy existing instances
			foreach(var obj in Resources.FindObjectsOfTypeAll<T>())
			{
				DestroyImmediate(obj);
			}
			var asset = CreateInstance<T>();
			try
			{
				asset.Initialize();
				asset.OnInitialize();
			}
			catch(System.Exception e)
			{
				e.LogException("Project settings asset initialization failed.");
			}
			return asset;
		}

		protected virtual void OnCreateNewSettings()
		{

		}

		protected void Initialize()
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
#endif
		}

		protected virtual void OnInitialize()
		{

		}

		protected virtual void Validate()
		{

		}

#if UNITY_EDITOR

		public virtual void EditorLoad(string json)
		{
			UnityEditor.EditorJsonUtility.FromJsonOverwrite(json, this);
		}

		public virtual void EditorSave()
		{
			Validate();
			string json = UnityEditor.EditorJsonUtility.ToJson(this, true);
			File.WriteAllText(ProjectAssetPath, json);
		}

		public virtual void DrawEditorGUI()
		{
			var obj = BeginEditorGUI();
			DrawEditorProperties(obj);
			EndEditorGUI(obj);
		}


		protected UnityEditor.SerializedObject BeginEditorGUI()
		{
			UnityEditor.EditorGUIUtility.labelWidth = 250;
			return new UnityEditor.SerializedObject(this);
		}

		protected void DrawEditorProperties(UnityEditor.SerializedObject obj)
		{
			var prop = obj.GetIterator();
			prop.NextVisible(true);
			obj.Update();
			while(prop.NextVisible(false))
			{
				UnityEditor.EditorGUILayout.PropertyField(prop);
			}
		}

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