using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(PropertyDrawerUtility.GetAttribute<ReadOnlyAttribute>(property, true).asFields)
			{
				var lEnabledState = GUI.enabled;
				GUI.enabled = false;
				EditorGUI.PropertyField(position, property);
				GUI.enabled = lEnabledState;
			}
			else
			{
				EditorGUI.LabelField(position, label, new GUIContent(PropertyDrawerUtility.GetTargetObjectOfProperty(property)?.ToString() ?? "(null)"));
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if(PropertyDrawerUtility.GetAttribute<ReadOnlyAttribute>(property, true).asFields)
			{
				return base.GetPropertyHeight(property, label);
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}
	}
}
