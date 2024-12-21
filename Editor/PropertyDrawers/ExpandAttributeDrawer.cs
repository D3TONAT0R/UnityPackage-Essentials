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

		//TODO: check for invalid usage of ExpandAttribute
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var expandAttribute = (ExpandAttribute)attribute;
			if(property.propertyType == SerializedPropertyType.ObjectReference)
			{
				if(property.isExpanded && expandAttribute.drawBox)
				{
					var boxPos = position;
					boxPos.xMin -= BOX_PADDING - 15;
					boxPos.xMax += BOX_PADDING;
					boxPos.yMin += EditorGUIUtility.singleLineHeight + 2;
					GUI.Box(boxPos, GUIContent.none, EditorStyles.helpBox);
				}
				position.height = EditorGUIUtility.singleLineHeight;
				position.SplitHorizontal(EditorGUIUtility.labelWidth, out var labelPos, out var fieldPos, 4);
				property.isExpanded = EditorGUI.Foldout(labelPos, property.isExpanded, label);
				EditorGUI.PropertyField(fieldPos, property, GUIContent.none);
				var obj = property.objectReferenceValue;
				if(property.isExpanded && obj != null)
				{
					if(expandAttribute.drawBox) position.y += BOX_PADDING;
					EditorGUI.indentLevel++;
					var so = new SerializedObject(obj);
					var prop = so.GetIterator();
					prop.NextVisible(true);
					while(prop.NextVisible(false))
					{
						if(!IsVisible(prop)) continue;
						position.NextProperty(EditorGUI.GetPropertyHeight(prop));
						EditorGUI.PropertyField(position, prop);
					}
					so.ApplyModifiedProperties();
					EditorGUI.indentLevel--;
				}
			}
			else
			{
				if(expandAttribute.drawBox)
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
				foreach(var child in GetDirectChildren(property))
				{
					//Skip properties that are hidden by ShowIfAttribute
					if(!IsVisible(child)) continue;
					float height = EditorGUI.GetPropertyHeight(child);
					position.NextProperty(height);
					EditorGUI.PropertyField(position, child);
				}
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var expandAttribute = (ExpandAttribute)attribute;
			float height;
			if(property.propertyType != SerializedPropertyType.ObjectReference)
			{
				property.isExpanded = true;
				height = EditorGUI.GetPropertyHeight(property, true);
				if(expandAttribute.drawBox) height += BOX_PADDING * 2;
			}
			else
			{
				var obj = property.objectReferenceValue;
				if(obj == null || !property.isExpanded)
				{
					height = EditorGUIUtility.singleLineHeight;
				}
				else
				{
					height = EditorGUIUtility.singleLineHeight;
					var so = new SerializedObject(obj);
					var prop = so.GetIterator();
					prop.NextVisible(true);
					while(prop.NextVisible(false))
					{
						if(!IsVisible(prop)) continue;
						height += EditorGUIUtility.standardVerticalSpacing;
						height += EditorGUI.GetPropertyHeight(prop);
					}
					if(expandAttribute.drawBox) height += BOX_PADDING * 2;
				}
			}
			return height;
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

		private static bool IsVisible(SerializedProperty property)
		{
			if(PropertyDrawerUtility.TryGetAttribute<ShowIfAttribute>(property, true, out var showIf))
			{
				return showIf.ShouldDraw(PropertyDrawerUtility.GetParent(property));
			}
			return true;
		}
	}
}