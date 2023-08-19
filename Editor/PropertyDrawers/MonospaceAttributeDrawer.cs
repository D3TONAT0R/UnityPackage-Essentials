using D3T;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(MonospaceAttribute))]
    public class MonospaceAttributeDrawer : PropertyDrawer
    {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, SerializedPropertyType.String, SerializedPropertyType.Float, SerializedPropertyType.Integer)) return;
			EditorGUI.BeginProperty(position, label, property);
			if(property.propertyType == SerializedPropertyType.String)
			{
				property.stringValue = EditorGUI.TextField(position, label, property.stringValue, EditorGUIExtras.GetMonospaceTextField(property));
			}
			else if(property.propertyType == SerializedPropertyType.Float)
			{
				property.floatValue = EditorGUI.FloatField(position, label, property.floatValue, EditorGUIExtras.GetMonospaceTextField(property));
			}
			else if(property.propertyType == SerializedPropertyType.Integer)
			{
				property.intValue = EditorGUI.IntField(position, label, property.intValue, EditorGUIExtras.GetMonospaceTextField(property));
			}
			EditorGUI.EndProperty();
		}
	} 
}
