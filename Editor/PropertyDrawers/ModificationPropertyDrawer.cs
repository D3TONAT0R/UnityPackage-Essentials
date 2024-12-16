using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	public abstract class ModificationPropertyDrawer : PropertyDrawer
	{


		protected void DrawProperty(Rect position, SerializedProperty property, GUIContent label)
		{
			//EditorGUI.LabelField(position, GUIContent.none);
			PropertyDrawerUtility.DrawPropertyWithAttributeExcept(position, property, label, attribute.GetType());
		}

		protected float GetBaseHeight(SerializedProperty property, GUIContent label)
		{
			return PropertyDrawerUtility.GetPropertyHeightWithAttributeExcept(property, label, attribute.GetType());
		}

		/*
		public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label);
		}
		*/
	}
}
