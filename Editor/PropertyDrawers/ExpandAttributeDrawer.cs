using UnityEngine;
using UnityEssentials;
using UnityEditor;
using System.Collections.Generic;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(ExpandAttribute), true)]
	public class ExpandAttributeDrawer : PropertyDrawer
	{
		private const float BOX_PADDING = 4;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var exAttribute = (ExpandAttribute)attribute;
			if(exAttribute.drawBox)
			{
				var boxPos = position;
				boxPos.xMin -= BOX_PADDING;
				boxPos.xMax += BOX_PADDING;
				GUI.Box(boxPos, GUIContent.none, EditorStyles.helpBox);
				position.yMin += BOX_PADDING;
				position.yMax -= BOX_PADDING;
			}
			property.isExpanded = true;
			position.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField(position, label, EditorStyles.boldLabel);
			Random.InitState(0);
			foreach(var child in GetDirectChildren(property))
			{
				float height = EditorGUI.GetPropertyHeight(child);
				position.NextProperty(height);
				EditorGUI.PropertyField(position, child);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var exAttribute = (ExpandAttribute)attribute;
			property.isExpanded = true;
			float h = EditorGUI.GetPropertyHeight(property, true);
			if(exAttribute.drawBox) h += BOX_PADDING * 2;
			return h;
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
