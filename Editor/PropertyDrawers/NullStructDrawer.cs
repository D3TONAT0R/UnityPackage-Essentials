﻿using D3T;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	[CustomPropertyDrawer(typeof(Null))]
	public class NullStructDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return -EditorGUIUtility.standardVerticalSpacing;
		}
	} 
}
