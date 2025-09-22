using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(EnabledIfAttribute), true)]
	public class EnabledIfAttributeDrawer : ModificationPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var lastState = GUI.enabled;
			GUI.enabled &= IsEnabled(property);
			DrawProperty(position, property, label);
			GUI.enabled = lastState;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return GetBaseHeight(property, label);
		}

		private bool IsEnabled(SerializedProperty property)
		{
			var attr = PropertyDrawerUtility.GetAttribute<EnabledIfAttribute>(property, true);
			return attr?.IsEnabled(PropertyDrawerUtility.GetParentObject(property)) ?? true;
		}
	}
}
