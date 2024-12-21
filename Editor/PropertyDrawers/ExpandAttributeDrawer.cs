using UnityEngine;
using UnityEssentials;
using UnityEditor;
using System.Collections.Generic;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(ExpandAttribute), true)]
	public class ExpandAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attribute = PropertyDrawerUtility.GetAttribute<ExpandAttribute>(property, true);
			if(attribute.drawBox) GUI.Box(position, GUIContent.none);
			property.isExpanded = true;
			position.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField(position, label, EditorStyles.boldLabel);
			foreach(var child in GetDirectChildren(property))
			{
				//TODO: Get the correct height for the property
				var height = EditorGUIUtility.singleLineHeight;
				position.NextProperty(height);
				EditorGUI.PropertyField(position, child);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			property.isExpanded = true;
			return EditorGUI.GetPropertyHeight(property, true);
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
