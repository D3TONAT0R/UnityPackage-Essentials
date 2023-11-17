using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(CurveUsageAttribute))]
	public class CurveUsageAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, SerializedPropertyType.AnimationCurve)) return;
			var usageAttribute = (CurveUsageAttribute)attribute;
			using(new EditorGUI.PropertyScope(position, label, property))
			{
				property.animationCurveValue = EditorGUI.CurveField(position, label, property.animationCurveValue, usageAttribute.color, usageAttribute.ranges);
			}
		}
	} 
}
