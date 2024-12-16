using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(MonospaceAttribute))]
	public class MonospaceAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, SerializedPropertyType.String, SerializedPropertyType.Float, SerializedPropertyType.Integer)) return;
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
			if(property.propertyType == SerializedPropertyType.String)
			{
				EditorGUI.BeginChangeCheck();
				var newValue = EditorGUI.TextField(position, label, property.stringValue, EditorGUIExtras.GetMonospaceTextField(property));
				if(EditorGUI.EndChangeCheck())
				{
					property.stringValue = newValue;
				}
			}
			else if(property.propertyType == SerializedPropertyType.Float)
			{
				EditorGUI.BeginChangeCheck();
				var newValue = EditorGUI.FloatField(position, label, property.floatValue, EditorGUIExtras.GetMonospaceTextField(property));
				if(EditorGUI.EndChangeCheck())
				{
					property.floatValue = newValue;
				}
			}
			else if(property.propertyType == SerializedPropertyType.Integer)
			{
				EditorGUI.BeginChangeCheck();
				var newValue = EditorGUI.IntField(position, label, property.intValue, EditorGUIExtras.GetMonospaceTextField(property));
				if(EditorGUI.EndChangeCheck())
				{
					property.intValue = newValue;
				}
			}
			EditorGUI.showMixedValue = false;
			EditorGUI.EndProperty();
		}
	}
}
