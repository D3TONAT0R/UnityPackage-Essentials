using UnityEssentials;
using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(EnumButtonGroupAttribute))]
	public class EnumButtonsAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			if(!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, SerializedPropertyType.Enum)) return;
			var enumType = PropertyDrawerUtility.GetTypeOfProperty(property);
			if(!property.hasMultipleDifferentValues)
			{
				Enum input = (Enum)Enum.ToObject(enumType, property.intValue);
				Enum output = EditorGUIExtras.EnumButtons(position, label, input, enumType);
				property.intValue = (int)Convert.ChangeType(output, typeof(int));
			}
			else
			{
				Enum output = EditorGUIExtras.EnumButtons(position, label, null, enumType);
				if(output != null)
				{
					property.intValue = (int)Convert.ChangeType(output, typeof(int));
				}
			}
			EditorGUI.EndProperty();
		}
	}
}