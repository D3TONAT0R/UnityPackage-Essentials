using D3T;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
	public class HelpBoxDecoratorDrawer : DecoratorDrawer
	{

		public override void OnGUI(Rect position)
		{
			position.height -= EditorGUIUtility.standardVerticalSpacing;
			position.x += EditorGUI.indentLevel * 15;
			var attr = (HelpBoxAttribute)attribute;
			EditorGUI.HelpBox(position, attr.message, attr.MessageType);
		}

		public override float GetHeight()
		{
			var attr = (HelpBoxAttribute)attribute;
			var width = EditorGUIUtility.currentViewWidth - EditorGUI.indentLevel * 15;
			return Mathf.Max(20, EditorStyles.helpBox.CalcHeight(new GUIContent(attr.message), width - 30)) + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}