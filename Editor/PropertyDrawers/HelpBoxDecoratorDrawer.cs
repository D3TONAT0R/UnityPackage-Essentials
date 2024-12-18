﻿using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
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
			float width = 200;
			if(Event.current != null)
			{
				try
				{
					width = EditorGUIUtility.currentViewWidth;
				}
				catch(System.ArgumentException e)
				{
					e.LogException();
				}
			}
			return Mathf.Max(20, EditorStyles.helpBox.CalcHeight(new GUIContent(attr.message), width - 30)) + EditorGUIUtility.standardVerticalSpacing;
		}
	}
}