using D3T;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(FloatRange))]
	public class FloatRangeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
			position.height = EditorGUIUtility.singleLineHeight;
			var min = property.FindPropertyRelative("min");
			var max = property.FindPropertyRelative("max");
			var attr = GetRangeAttribute(property);
			if(attr != null)
			{
				var minValue = min.floatValue;
				var maxValue = max.floatValue;
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = min.hasMultipleDifferentValues || max.hasMultipleDifferentValues;
				EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, attr.min, attr.max);
				if(EditorGUI.EndChangeCheck())
				{
					minValue = Mathf.Min(minValue, maxValue);
					maxValue = Mathf.Max(minValue, maxValue);
					min.floatValue = minValue;
					max.floatValue = maxValue;
				}
				position.NextProperty();
			}
			bool hasLabel = !string.IsNullOrEmpty(label.text);
			position.SplitHorizontal(hasLabel ? EditorGUIUtility.labelWidth : 0, out var labelRect, out position);
			if(attr == null)
			{
				EditorGUI.LabelField(labelRect, label);
			}
			position.SplitHorizontalRelative(0.5f, out var minRect, out var maxRect, 4);
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			EditorGUIUtility.labelWidth = 30;

			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = min.hasMultipleDifferentValues;
			EditorGUI.PropertyField(minRect, min);
			if(EditorGUI.EndChangeCheck())
			{
				min.floatValue = Mathf.Min(min.floatValue, max.floatValue);
				if(attr != null) min.floatValue = Mathf.Clamp(min.floatValue, attr.min, attr.max);
			}

			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = max.hasMultipleDifferentValues;
			EditorGUI.PropertyField(maxRect, max);
			if(EditorGUI.EndChangeCheck())
			{
				max.floatValue = Mathf.Max(min.floatValue, max.floatValue);
				if(attr != null) max.floatValue = Mathf.Clamp(max.floatValue, attr.min, attr.max);
			}

			EditorGUI.indentLevel = indent;
			EditorGUI.showMixedValue = false;
			EditorGUI.EndProperty();
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