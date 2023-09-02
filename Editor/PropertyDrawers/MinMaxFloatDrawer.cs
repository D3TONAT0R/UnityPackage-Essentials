using D3T;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(FloatRange))]
	public class MinMaxFloatDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = EditorGUIUtility.singleLineHeight;
			var min = property.FindPropertyRelative("min");
			var max = property.FindPropertyRelative("max");
			var attr = GetRangeAttribute(property);
			if(attr != null)
			{
				var minValue = min.floatValue;
				var maxValue = max.floatValue;
				EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, attr.min, attr.max);
				min.floatValue = minValue;
				max.floatValue = maxValue;
				position.NextProperty();
			}
			position.SplitHorizontal(EditorGUIUtility.labelWidth, out var labelRect, out position);
			if(attr == null)
			{
				EditorGUI.LabelField(labelRect, label);
			}
			position.SplitHorizontalRelative(0.5f, out var minRect, out var maxRect, 4);
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			EditorGUIUtility.labelWidth = 30;
			EditorGUI.PropertyField(minRect, min);
			EditorGUI.PropertyField(maxRect, max);
			EditorGUI.indentLevel = indent;
			if(attr != null)
			{
				min.floatValue = Mathf.Clamp(min.floatValue, attr.min, attr.max);
				max.floatValue = Mathf.Clamp(max.floatValue, attr.min, attr.max);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if(GetRangeAttribute(property) != null)
			{
				return 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
			else
			{
				return base.GetPropertyHeight(property, label);
			}
		}

		private MinMaxRangeAttribute GetRangeAttribute(SerializedProperty property)
		{
			return PropertyDrawerUtility.GetAttribute<MinMaxRangeAttribute>(property, true);
		}
	}
}