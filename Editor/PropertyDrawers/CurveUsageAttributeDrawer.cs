using D3T;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
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
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
				var newCurve = EditorGUI.CurveField(position, label, property.animationCurveValue, usageAttribute.color, usageAttribute.ranges);
				EditorGUI.showMixedValue = false;
				if(EditorGUI.EndChangeCheck())
				{
					property.animationCurveValue = newCurve;
				}
			}
		}
	}
}
