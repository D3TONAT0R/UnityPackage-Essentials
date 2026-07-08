using System;
using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(DynamicRangeAttribute), true)]
	public class DynamicRangeAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, SerializedPropertyType.Float,
				    SerializedPropertyType.Integer))
			{
				return;
			}
			var attr = attribute as DynamicRangeAttribute;
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();
			var target = property.GetParentObject();
			var min = GetLimitValue(target, attr.minPropertyName, attr.minValue);
			var max = GetLimitValue(target, attr.maxPropertyName, attr.maxValue);
			if (!attr.softLimits && !Application.isPlaying)
			{
				if(property.floatValue < min ||  property.floatValue > max) 
					property.floatValue = Mathf.Clamp(property.floatValue, min, max);
			}
			//TODO: allow setting values outside of slider range when soft limits is set to true
			if (property.propertyType == SerializedPropertyType.Float)
			{
				var value = property.floatValue;
				value = EditorGUI.Slider(position, label, value, min, max);
				if (EditorGUI.EndChangeCheck()) property.floatValue = value;
			}
			else
			{
				var value = (float)property.intValue;
				value = EditorGUI.Slider(position, label, value, min, max);
				if (EditorGUI.EndChangeCheck()) property.intValue = (int)value;
			}
			EditorGUI.EndProperty();
		}

		private float GetLimitValue(object source, string propertyName, float value)
		{
			if (string.IsNullOrEmpty(propertyName)) return value;
			var member = source.GetType().GetMember(propertyName)[0];
			var v = ReflectionUtility.GetMemberValue(member, source);
			return Convert.ToSingle(v);
		}
	}
}