using D3T;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	[CustomPropertyDrawer(typeof(LayerPopupAttribute))]
	public class LayerPopupAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			property.intValue = EditorGUI.LayerField(position, label, property.intValue);
			EditorGUI.EndProperty();
		}
	}
}
