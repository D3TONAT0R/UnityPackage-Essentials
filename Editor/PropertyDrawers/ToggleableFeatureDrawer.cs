using UnityEssentials;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ToggleableFeature), true)]
	public class ToggleableFeatureDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var obj = PropertyDrawerUtility.GetTargetObjectOfProperty<ToggleableFeature>(property);
			DrawBgBox(position);
			position.height = EditorGUIUtility.singleLineHeight;
			bool enabled = DrawCheckbox(position, property);
			GUI.backgroundColor = Color.clear;
			string displayName = !string.IsNullOrWhiteSpace(obj.CustomName) ? obj.CustomName : property.displayName;
			property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, property.isExpanded, displayName);
			EditorGUI.EndFoldoutHeaderGroup();
			GUI.backgroundColor = Color.white;
			EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
			GUI.enabled = enabled;
			if(property.isExpanded)
			{
				position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				foreach(SerializedProperty p in GetChildren(property))
				{
					var h = EditorGUI.GetPropertyHeight(p);
					position.height = h;
					EditorGUI.PropertyField(position, p, true);
					position.y += h + EditorGUIUtility.standardVerticalSpacing;
				}
			}
			GUI.enabled = true;
			EditorGUI.EndProperty();
		}

		private void DrawBgBox(Rect position)
		{
			position.xMin -= 15;
			position.xMax += 2;
			GUI.Box(position, "", EditorStyles.helpBox);
		}

		private bool DrawCheckbox(Rect position, SerializedProperty property)
		{
			var checkPos = position;
			checkPos.x += EditorGUIUtility.labelWidth + 2;
			checkPos.y += 2;
			checkPos.width = 20;
			var prop = property.FindPropertyRelative("enabled");
			EditorGUI.PropertyField(checkPos, prop, GUIContent.none);
			return prop.boolValue;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, true) + 5;
		}

		public static IEnumerable<SerializedProperty> GetChildren(SerializedProperty property)
		{
			property = property.Copy();
			var nextElement = property.Copy();
			bool hasNextElement = nextElement.NextVisible(false);
			if(!hasNextElement)
			{
				nextElement = null;
			}

			property.NextVisible(true);
			while(true)
			{
				if((SerializedProperty.EqualContents(property, nextElement)))
				{
					yield break;
				}

				yield return property;

				bool hasNext = property.NextVisible(false);
				if(!hasNext)
				{
					break;
				}
			}
		}
	}
}
