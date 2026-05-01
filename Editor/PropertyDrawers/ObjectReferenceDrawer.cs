using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObjectReference<>))]
	public class ObjectReferenceDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var scriptRefProp = property.FindPropertyRelative("objectRef");
			var type = property.GetValueType().GetGenericArguments()[0];
			var newValue = EditorGUI.ObjectField(position, label, scriptRefProp.objectReferenceValue, type, true);
			if (newValue != scriptRefProp.objectReferenceValue)
			{
				scriptRefProp.objectReferenceValue = newValue;
			}
			EditorGUI.EndProperty();
		}
	}
}