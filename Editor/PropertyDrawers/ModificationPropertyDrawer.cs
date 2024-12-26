using System;
using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor.PropertyDrawers
{
	public abstract class ModificationPropertyDrawer : PropertyDrawer
	{


		protected void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
		{
			try
			{
				PropertyDrawerUtility.DrawPropertyWithAttributeExcept(position, property, label, attribute.GetType(), attribute.order);
			}
			catch(Exception e)
			{
				e.LogException();
			}
		}

		protected float GetBaseHeight(SerializedProperty property, GUIContent label)
		{
			return PropertyDrawerUtility.GetPropertyHeightWithAttributeExcept(property, label, attribute.GetType(), attribute.order);
		}
	}
}
