using UnityEssentials;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(SceneAttribute))]
	public class SceneAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			if(!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, SerializedPropertyType.String, SerializedPropertyType.Integer)) return;
			
			position.SplitHorizontal(EditorGUIUtility.labelWidth, out var labelRect, out var buttonRect, 2);


			GUIContent text = new GUIContent();
			bool valid;
			if(property.propertyType == SerializedPropertyType.String)
			{
				string sceneName = property.stringValue;
				if(string.IsNullOrWhiteSpace(sceneName))
				{
					text.text = "(None)";
					valid = true;
				}
				else
				{
					int buildIndex = GetSceneBuildIndex(sceneName);
					if(buildIndex >= 0)
					{
						text.text = $"{sceneName} ({buildIndex})";
						valid = true;
					}
					else
					{
						text.text = $"{sceneName} (Invalid)";
						valid = false;
					}
				}
			}
			else
			{
				int buildIndex = property.intValue;
				if(buildIndex < 0)
				{
					text.text = "(None)";
					valid = true;
				}
				else if(buildIndex > SceneManager.sceneCountInBuildSettings)
				{
					text.text = $"{buildIndex}: (Invalid)";
					valid = false;
				}
				else
				{
					text.text = $"{buildIndex}: {GetSceneName(buildIndex)}";
					valid = true;
				}
			}

			if(valid)
			{
				EditorGUI.PrefixLabel(labelRect, label);
			}
			else
			{
				EditorGUIExtras.ErrorLabelField(labelRect, label, GUIContent.none);
			}
			if(EditorGUI.DropdownButton(buttonRect, text, FocusType.Passive))
			{
				bool isString = property.propertyType == SerializedPropertyType.String;
				ShowContextMenu(buttonRect, isString ? (object)property.stringValue : (object)property.intValue, valid, property.serializedObject, property.propertyPath, isString);
			}

			EditorGUI.EndProperty();
		}

		private void ShowContextMenu(Rect position, object value, bool validValue, SerializedObject serializedObject, string propertyPath, bool isStringField)
		{
			var menu = new GenericMenu();
			if(isStringField)
			{
				string stringValue = (string)value;
				if(!validValue)
				{
					menu.AddItem($"({stringValue})", true, true, null);
					menu.AddSeparator("");
				}
				if(SceneManager.sceneCountInBuildSettings == 0)
				{
					menu.AddItem("No Scenes in Build Settings", false, false, null);
				}
				menu.AddItem("(None)", true, string.IsNullOrEmpty(stringValue), () => SetStringValue(serializedObject, propertyPath, ""));
				for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
				{
					var scene = SceneManager.GetSceneByBuildIndex(i);
					var name = GetSceneName(i);
					menu.AddItem($"{name} ({i})", true, name == stringValue, () => SetStringValue(serializedObject, propertyPath, name));
				}
			}
			else
			{
				int intValue = (int)value;
				if(!validValue)
				{
					menu.AddItem($"{intValue}: (Invalid)", true, true, null);
					menu.AddSeparator("");
				}
				if(SceneManager.sceneCountInBuildSettings == 0)
				{
					menu.AddItem("No Scenes in Build Settings", false, false, null);
				}
				menu.AddItem("(None)", true, intValue < 0, () => SetIntValue(serializedObject, propertyPath, -1));
				for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
				{
					var scene = SceneManager.GetSceneByBuildIndex(i);
					var name = GetSceneName(i);
					int j = i;
					menu.AddItem($"{j}: {name}", true, j == intValue, () => SetIntValue(serializedObject, propertyPath, j));
				}
			}
			menu.AddSeparator("");
			menu.AddItem("Build Settings ...", true, false, () => EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow, UnityEditor")));
			menu.DropDown(position);
		}

		private string GetSceneName(int buildIndex)
		{
			return Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(buildIndex));
		}

		private int GetSceneBuildIndex(string sceneName)
		{
			for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				if(GetSceneName(i) == sceneName)
				{
					return i;
				}
			}
			return -1;
		}

		private void SetStringValue(SerializedObject obj, string propertyPath, string value)
		{
			obj.Update();
			obj.FindProperty(propertyPath).stringValue = value;
			obj.ApplyModifiedProperties();
		}

		private void SetIntValue(SerializedObject obj, string propertyPath, int value)
		{
			obj.Update();
			obj.FindProperty(propertyPath).intValue = value;
			obj.ApplyModifiedProperties();
		}
	}
}