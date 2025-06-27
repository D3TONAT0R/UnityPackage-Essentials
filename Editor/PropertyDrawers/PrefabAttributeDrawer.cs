using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(PrefabAttribute))]
	public class PrefabAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, SerializedPropertyType.ObjectReference)
				|| !PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, typeof(GameObject)))
			{
				return;
			}
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();
			var newObject = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(GameObject), false);
			if(EditorGUI.EndChangeCheck())
			{
				property.objectReferenceValue = newObject;
			}
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label);
		}
	}
}