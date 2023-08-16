using D3T;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(IntPopupAttribute), true)]
	public class IntPopupAttributeDrawer : PropertyDrawer
	{
		const int maxItemCount = 20;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, label, SerializedPropertyType.Integer)) return;
			var choices = PropertyDrawerUtility.GetAttribute<IntPopupAttribute>(property, true).choices;
			if(choices.Length > maxItemCount)
			{
				Debug.LogError($"Excessive number of choices in IntPopupAttribute detected ({choices.Length}), trimming excess items.");
				choices = choices.Take(maxItemCount).ToArray();
			}
			EditorGUI.BeginChangeCheck();
			GUIContent[] choiceStrings = choices.Select((i) => new GUIContent(i.ToString())).ToArray();
			var sel = EditorGUI.IntPopup(position, label, property.intValue, choiceStrings, choices);
			if(!choices.Contains(property.intValue))
			{
				EditorGUI.LabelField(position, " ", $" ({property.intValue})", EditorStyles.boldLabel);
			}
			if(EditorGUI.EndChangeCheck())
			{
				property.intValue = sel;
			}
		}
	}
}
