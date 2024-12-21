using UnityEssentials;
using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(CreateAssetButtonAttribute))]
	public class CreateAssetButtonAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var type = PropertyDrawerUtility.GetTypeOfProperty(property);
			if(IsTypeSupported(type, out string ext))
			{
				position.width -= 45;
				EditorGUI.PropertyField(position, property);
				position.x += position.width + 5;
				position.width = 40;
				if(GUI.Button(position, "New"))
				{
					CreateNewAssetDialog(property.serializedObject, property.propertyPath, type, ext);
					GUIUtility.ExitGUI();
				}
			}
			else
			{
				EditorGUIExtras.ErrorLabelField(position, label, new GUIContent("(Asset creation not supported)"));
			}
		}

		private void CreateNewAssetDialog(SerializedObject propRoot, string propPath, Type type, string ext)
		{
			string name = $"New {type.Name}";
			var defaultPath = ((CreateAssetButtonAttribute)attribute).defaultPath;
			var path = EditorUtility.SaveFilePanelInProject($"Create new {type.Name}", name, ext, "", "Assets/" + defaultPath);
			if(!string.IsNullOrWhiteSpace(path))
			{
				var obj = CreateAsset(type);
				AssetDatabase.CreateAsset(obj, path);
				propRoot.FindProperty(propPath).objectReferenceValue = obj;
				propRoot.ApplyModifiedProperties();
			}
		}

		private bool IsTypeSupported(Type type, out string extension)
		{
			if(typeof(ScriptableObject).IsAssignableFrom(type))
			{
				extension = "asset";
				return true;
			}
			extension = null;
			if(type == typeof(AnimationClip)) extension = "anim";

			return extension != null;
		}

		private UnityEngine.Object CreateAsset(Type type)
		{
			if(typeof(ScriptableObject).IsAssignableFrom(type)) return ScriptableObject.CreateInstance(type);
			if(type == typeof(AnimationClip)) return new AnimationClip();

			throw new NotImplementedException();
		}
	}
}
