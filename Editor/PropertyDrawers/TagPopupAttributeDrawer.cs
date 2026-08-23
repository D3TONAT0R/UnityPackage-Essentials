using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(TagPopupAttribute))]
	public class TagPopupAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, SerializedPropertyType.String)) return;
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
			if (!property.hasMultipleDifferentValues && string.IsNullOrEmpty(property.stringValue)) property.stringValue = UnityEditorInternal.InternalEditorUtility.tags[0];
			var value = EditorGUI.TagField(position, label, property.stringValue);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				property.stringValue = value;
			}
			EditorGUI.EndProperty();
		}
	}
}