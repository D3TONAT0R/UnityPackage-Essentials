using D3T;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	[CustomPropertyDrawer(typeof(InlineClass), true)]
	public class InlineClassDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var target = PropertyDrawerUtility.GetTargetObjectOfProperty<InlineClass>(property);
			if(target.InheritInlinedLayout)
			{
				position.height = EditorGUIUtility.singleLineHeight;

				EditorGUI.LabelField(position, label);
				position.xMin += EditorGUIUtility.labelWidth + 3;

				int childCount = GetDirectChildren(property).Count();
				var rects = position.DivideHorizontal(childCount, 2);
				int i = 0;
				while(property.NextVisible(i == 0) && i < childCount)
				{
					EditorGUI.PropertyField(rects[i], property, GUIContent.none);
					//PropertyDrawerUtility.DrawPropertyField(rects[i], prop, GUIContent.none);
					i++;
				}
			}
			else
			{
				EditorGUI.PropertyField(position, property, label, true);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var target = PropertyDrawerUtility.GetTargetObjectOfProperty<InlineClass>(property);
			if(target.InheritInlinedLayout)
			{
				return EditorGUIUtility.singleLineHeight;
			}
			else
			{
				return EditorGUI.GetPropertyHeight(property, label, true);
			}
		}

		private static IEnumerable<SerializedProperty> GetDirectChildren(SerializedProperty parent)
		{
			var copy = parent.Copy();
			int rootDepth = copy.depth;
			foreach(SerializedProperty inner in copy)
			{
				if(inner.depth == rootDepth + 1) yield return inner;
			}
		}

	}
}
