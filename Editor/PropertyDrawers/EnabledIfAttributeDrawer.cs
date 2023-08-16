using D3T;
using D3TEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(EnabledIfAttribute))]
	public class EnabledIfAttributeDrawer : ModificationPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var lastState = GUI.enabled;
			GUI.enabled = IsEnabled(property);
			DrawProperty(position, property, label);
			GUI.enabled = lastState;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label);
		}

		private bool IsEnabled(SerializedProperty property)
		{
			var attr = PropertyDrawerUtility.GetAttribute<EnabledIfAttribute>(property, true);
			return attr?.IsEnabled(PropertyDrawerUtility.GetParent(property)) ?? true;
		}
	} 
}
