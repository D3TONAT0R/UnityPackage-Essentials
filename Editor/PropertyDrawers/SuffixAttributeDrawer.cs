using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(SuffixAttribute))]
	public class UnitAttributeDrawer : PropertyDrawer
	{
		float fieldWidth;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attr = PropertyDrawerUtility.GetAttribute<SuffixAttribute>(property, true);
			var suffixWidth = attr.fixedWidth;
			if(suffixWidth <= 0)
			{
				var requiredWidth = Mathf.Ceil((EditorStyles.label.CalcSize(attr.suffix).x + 5) / 10f) * 10f; //Round to 10 pixel increments
				suffixWidth = Mathf.Max(requiredWidth, 30);
			}
			if(Event.current.type == EventType.Repaint)
			{
				fieldWidth = position.width - EditorGUIUtility.labelWidth;
			}
			if(fieldWidth < suffixWidth * 2)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}
			position.SplitHorizontalRight(suffixWidth, out position, out var unitRect, 2);
			EditorGUI.PropertyField(position, property, label);
			GUI.Label(unitRect, attr.suffix);
		}
	}
}
