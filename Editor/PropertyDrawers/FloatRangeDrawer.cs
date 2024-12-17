using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(FloatRange))]
	public class FloatRangeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
			position.height = EditorGUIUtility.singleLineHeight;
			var min = property.FindPropertyRelative(nameof(FloatRange.min));
			var max = property.FindPropertyRelative(nameof(FloatRange.max));
			var rangeAttribute = GetRangeAttribute(property);
			if(rangeAttribute != null)
			{
				DrawSlider(position, label, min, max, rangeAttribute);
				position.NextProperty();
			}
			bool hasLabel = !string.IsNullOrEmpty(label.text);
			position.SplitHorizontal(hasLabel ? EditorGUIUtility.labelWidth : 0, out var labelRect, out position);
			if(rangeAttribute == null)
			{
				EditorGUI.LabelField(labelRect, label);
			}
			position.SplitHorizontalRelative(0.5f, out var minRect, out var maxRect, 4);
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			EditorGUIUtility.labelWidth = 30;

			DrawField(min, max, minRect, rangeAttribute, false);
			DrawField(max, min, maxRect, rangeAttribute, true);

			EditorGUI.indentLevel = indent;
			EditorGUI.showMixedValue = false;
			EditorGUI.EndProperty();
		}

		private static void DrawField(SerializedProperty prop, SerializedProperty other, Rect minRect, MinMaxRangeAttribute rangeAttribute, bool max)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
			EditorGUI.PropertyField(minRect, prop);
			if(EditorGUI.EndChangeCheck())
			{
				if(rangeAttribute != null)
				{
					float value = prop.floatValue;
					value = Mathf.Clamp(value, rangeAttribute.min, rangeAttribute.max);
					value = max ? Mathf.Max(value, other.floatValue) : Mathf.Min(value, other.floatValue);
					prop.floatValue = value;
				}
			}
		}

		private static void DrawSlider(Rect position, GUIContent label, SerializedProperty min, SerializedProperty max,
			MinMaxRangeAttribute attr)
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