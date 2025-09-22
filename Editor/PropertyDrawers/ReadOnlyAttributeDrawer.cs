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
			if(property.GetAttribute<ReadOnlyAttribute>().drawAsFields)
			{
				var lEnabledState = GUI.enabled;
				GUI.enabled = false;
				PropertyDrawerUtility.DrawPropertyWithAttributeExcept(position, property, label, typeof(ReadOnlyAttribute), attribute.order);
				GUI.enabled = lEnabledState;
			}
			else
			{
				EditorGUI.LabelField(position, label, new GUIContent(property.GetValue()?.ToString() ?? "(null)"));
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if(PropertyDrawerUtility.GetAttribute<ReadOnlyAttribute>(property, true).drawAsFields)
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
