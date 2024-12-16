using UnityEssentials;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(Null))]
	internal class NullStructDrawer : PropertyDrawer
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
