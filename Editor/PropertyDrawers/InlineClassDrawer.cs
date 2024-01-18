using UnityEngine;
using UnityEssentials;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(InlineClass), true)]
	public class InlineClassDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.LabelField(position, label);
			position.xMin += EditorGUIUtility.labelWidth + 3;

			int childCount = GetDirectChildren(property).Count();
			var rects = position.DivideHorizontal(childCount, 2);
			int i = 0;
			while(property.NextVisible(i == 0) && i < childCount)
			{
				PropertyDrawerUtility.DrawPropertyField(rects[i], property, GUIContent.none);
				i++;
			}
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
