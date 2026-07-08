using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(PowRangeAttribute), true)]
	public class PowRangeAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attr = attribute as PowRangeAttribute;
			EditorGUI.BeginProperty(position, label, property);
			float value = Mathf.InverseLerp(attr.min, attr.max, property.floatValue);
			value = Mathf.Pow(value, 1f / attr.exponent);
			position.SplitHorizontal(EditorGUIUtility.labelWidth, out var labelRect, out position, 2);
			GUI.Label(labelRect, label);
			position.SplitHorizontalRight(50, out position, out var valueRect, 5);
			value = GUI.HorizontalSlider(position, value, 0, 1);
			value = Mathf.Pow(value, attr.exponent);
			value = Mathf.Lerp(attr.min, attr.max, value);
			if(EditorGUI.EndChangeCheck())
			{
				property.floatValue = value;
				GUI.FocusControl(null);
			}
			value = EditorGUI.FloatField(valueRect, value);
			if(EditorGUI.EndChangeCheck()) property.floatValue = value;
			EditorGUI.EndProperty();
		}
	}
}