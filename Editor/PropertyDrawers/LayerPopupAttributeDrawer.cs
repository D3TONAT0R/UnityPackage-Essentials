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
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
			var value = EditorGUI.LayerField(position, label, property.intValue);
			EditorGUI.showMixedValue = false;
			if(EditorGUI.EndChangeCheck())
			{
				property.intValue = value;
			}
			EditorGUI.EndProperty();
		}
	}
}
