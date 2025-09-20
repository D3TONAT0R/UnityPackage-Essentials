using UnityEngine;
using UnityEssentials;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(IDrawInlined), true)]
	public class InlinedDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			position.xMin += 15 * indent;

			position.height = EditorGUIUtility.singleLineHeight;

			if(label != null && !string.IsNullOrEmpty(label.text))
			{
				EditorGUI.LabelField(position, label);
				position.xMin += EditorGUIUtility.labelWidth + 3;
			}

			int childCount = GetDirectChildren(property).Count();
			var rects = position.DivideHorizontal(childCount, 2);
			int i = 0;
			while(property.NextVisible(i == 0) && i < childCount)
			{
				EditorGUI.PropertyField(rects[i], property, GUIContent.none);
				//PropertyDrawerUtility.DrawPropertyField(rects[i], prop, GUIContent.none);
				i++;
			}

			EditorGUI.indentLevel = indent;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
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
