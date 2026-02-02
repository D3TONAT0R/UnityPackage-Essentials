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
			var attr = (ReadOnlyAttribute)attribute;
			if(attr.drawAsFields)
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
			var attr = (ReadOnlyAttribute)attribute;
			if(attr.drawAsFields)
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
