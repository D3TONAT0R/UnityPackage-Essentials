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
			var usageAttribute = (CurveUsageAttribute)attribute;
			using(new EditorGUI.PropertyScope(position, label, property))
			{
				property.animationCurveValue = EditorGUI.CurveField(position, label, property.animationCurveValue, usageAttribute.color, usageAttribute.ranges);
			}
		}
	} 
}
