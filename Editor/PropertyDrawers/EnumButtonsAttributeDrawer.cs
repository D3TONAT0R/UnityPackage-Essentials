using D3T;
using System;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(EnumButtonsAttribute))]
	public class EnumButtonsAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			if(!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, SerializedPropertyType.Enum)) return;
			var enumType = PropertyDrawerUtility.GetTypeOfProperty(property);
			Enum input = (Enum)Enum.ToObject(enumType, property.intValue);
			Enum output = EditorGUIExtras.EnumButtons(position, label, input, enumType);
			property.intValue = (int)Convert.ChangeType(output, typeof(int));
			EditorGUI.EndProperty();
		}
	}
}