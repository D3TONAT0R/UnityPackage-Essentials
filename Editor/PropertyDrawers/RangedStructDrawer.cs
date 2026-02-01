using System.Linq;
using System.Reflection;
using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(FloatRange)), CustomPropertyDrawer(typeof(IntRange))]
	public class RangedStructDrawer : PropertyDrawer
	{
		private CachedSerializedProperty min;
		private CachedSerializedProperty max;
		private CachedAttribute<MinMaxRangeAttribute> rangeAttribute;
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var minProp = min.Find(property, "min");
			var maxProp = max.Find(property, "max");
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
			position.height = EditorGUIUtility.singleLineHeight;
			bool integer = minProp.propertyType == SerializedPropertyType.Integer;
			if(rangeAttribute.TryGet(fieldInfo, out var attr))
			{
				DrawSlider(position, label, minProp, maxProp, attr, integer);
				position.NextProperty();
			}
			bool hasLabel = !string.IsNullOrEmpty(label.text);
			position.SplitHorizontal(hasLabel ? EditorGUIUtility.labelWidth : 0, out var labelRect, out position);
			if(rangeAttribute.Get(fieldInfo) == null)
			{
				EditorGUI.LabelField(labelRect, label);
			}
			position.SplitHorizontalRelative(0.5f, out var minRect, out var maxRect, 4);
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			EditorGUIUtility.labelWidth = 30;

			DrawField(minProp, maxProp, minRect, rangeAttribute.Get(fieldInfo), integer, false);
			DrawField(maxProp, minProp, maxRect, rangeAttribute.Get(fieldInfo), integer, true);

			EditorGUI.indentLevel = indent;
			EditorGUI.showMixedValue = false;
			EditorGUI.EndProperty();
		}

		private static void DrawField(SerializedProperty prop, SerializedProperty other, Rect minRect, MinMaxRangeAttribute rangeAttribute, bool integer, bool max)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
			EditorGUI.PropertyField(minRect, prop);
			if(EditorGUI.EndChangeCheck())
			{
				if(rangeAttribute != null)
				{
					if(integer)
					{
						int value = prop.intValue;
						value = Mathf.Clamp(value, (int)rangeAttribute.min, (int)rangeAttribute.max);
						value = max ? Mathf.Max(value, other.intValue) : Mathf.Min(value, other.intValue);
						prop.intValue = value;
					}
					else
					{
						float value = prop.floatValue;
						value = Mathf.Clamp(value, rangeAttribute.min, rangeAttribute.max);
						value = max ? Mathf.Max(value, other.floatValue) : Mathf.Min(value, other.floatValue);
						prop.floatValue = value;
					}
				}
			}
		}

		private static void DrawSlider(Rect position, GUIContent label, SerializedProperty min, SerializedProperty max, MinMaxRangeAttribute attr, bool integer)
		{
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = min.hasMultipleDifferentValues || max.hasMultipleDifferentValues;
			var minValue = integer ? min.intValue : min.floatValue;
			var maxValue = integer ? max.intValue : max.floatValue;
			EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, attr.min, attr.max);
			if(EditorGUI.EndChangeCheck())
			{
				minValue = Mathf.Min(minValue, maxValue);
				maxValue = Mathf.Max(minValue, maxValue);
				if(integer)
				{
					min.intValue = (int)minValue;
					max.intValue = (int)maxValue;
				}
				else
				{
					min.floatValue = minValue;
					max.floatValue = maxValue;
				}
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if(rangeAttribute.TryGet(fieldInfo, out _))
			{
				return 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
			else
			{
				return base.GetPropertyHeight(property, label);
			}
		}
	}
}