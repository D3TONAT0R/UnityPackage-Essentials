using UnityEssentials;
using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(CreateAssetButtonAttribute))]
	public class CreateAssetButtonAttributeDrawer : PropertyDrawer
	{
		private static readonly GUIContent buttonContent = new GUIContent("New", "Create a new asset and assign it to this field.");
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (fieldInfo == null)
			{
				EditorGUI.PropertyField(position, property, label); 
				return;
			}
			var type = PropertyDrawerUtility.GetElementType(fieldInfo.FieldType, out _);
			if(IsTypeSupported(type, out string ext))
			{
				using (new EditorGUI.PropertyScope(position, label, property))
				{
					position.width -= 45;
					EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
					EditorGUI.BeginChangeCheck();
					var obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, type, false);
					if (EditorGUI.EndChangeCheck()) property.objectReferenceValue = obj;
					EditorGUI.showMixedValue = false;
					position.x += position.width + 5;
					position.width = 40;
					if (GUI.Button(position, buttonContent))
					{
						CreateNewAssetDialog(property.serializedObject, property.propertyPath, type, ext);
						GUIUtility.ExitGUI();
					}
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
