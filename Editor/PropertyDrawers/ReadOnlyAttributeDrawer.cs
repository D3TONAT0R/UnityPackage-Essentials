using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyAttributeDrawer : PropertyDrawer
	{
		private GUIContent content = new GUIContent();
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attr = (ReadOnlyAttribute)attribute;
			if(attr.drawAsFields)
			{
				var lEnabledState = GUI.enabled;
				GUI.enabled = false;
				EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
				PropertyDrawerUtility.DrawPropertyWithAttributeExcept(position, property, label, typeof(ReadOnlyAttribute), attribute.order);
				EditorGUI.showMixedValue = false;
				GUI.enabled = lEnabledState;
			}
			else
			{
				if(property.hasMultipleDifferentValues) content.text = "—";
				else content.text = property.GetValue()?.ToString() ?? "(null)";
				EditorGUI.LabelField(position, label, content);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var attr = (ReadOnlyAttribute)attribute;
			if(attr.drawAsFields)
			{
				return PropertyDrawerUtility.GetPropertyHeightWithAttributeExcept(property, label, attribute.GetType(), attribute.order);
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}
	}
}
