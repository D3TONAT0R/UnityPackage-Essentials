using UnityEngine;
using D3T;
using UnityEditor;

namespace D3TEditor
{
	[CustomPropertyDrawer(typeof(InlineClass), true)]
	public class AlwaysExpandedAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.LabelField(position, label);
			position.xMin += EditorGUIUtility.labelWidth + 3;

			int childCount = property.Copy().CountInProperty() - 1;
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
			return base.GetPropertyHeight(property, label);
		}
	}
}
