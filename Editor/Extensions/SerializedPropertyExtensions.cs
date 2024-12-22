using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public static class SerializedPropertyExtensions
	{
		/// <summary>
		/// Gets the value object that this SerializedProperty points to.
		/// </summary>
		public static object GetValueObject(this SerializedProperty prop)
		{
			return PropertyDrawerUtility.GetTargetObjectOfProperty(prop);
		}

		/// <summary>
		/// Gets the value object that this SerializedProperty points to.
		/// </summary>
		public static T GetValueObject<T>(this SerializedProperty prop)
		{
			return PropertyDrawerUtility.GetTargetObjectOfProperty<T>(prop);
		}

		/// <summary>
		/// Draws this property using <c>EditorGUI.PropertyField</c>
		/// </summary>
		public static void Draw(this SerializedProperty prop, Rect position)
		{
			EditorGUI.PropertyField(position, prop);
		}

		/// <summary>
		/// Draws this property using <c>EditorGUI.PropertyField</c>
		/// </summary>
		public static void Draw(this SerializedProperty prop, Rect position, bool includeChildren)
		{
			EditorGUI.PropertyField(position, prop, includeChildren);
		}

		/// <summary>
		/// Draws this property using <c>EditorGUI.PropertyField</c>
		/// </summary>
		public static void Draw(this SerializedProperty prop, Rect position, GUIContent label)
		{
			EditorGUI.PropertyField(position, prop, label);
		}

		/// <summary>
		/// Draws this property using <c>EditorGUI.PropertyField</c>
		/// </summary>
		public static void Draw(this SerializedProperty prop, Rect position, GUIContent label, bool includeChildren)
		{
			EditorGUI.PropertyField(position, prop, label, includeChildren);
		}

		/// <summary>
		/// Draws this property using <c>EditorGUILayout.PropertyField</c>
		/// </summary>
		public static void Draw(this SerializedProperty prop, params GUILayoutOption[] options)
		{
			EditorGUILayout.PropertyField(prop, options);
		}

		/// <summary>
		/// Draws this property using <c>EditorGUILayout.PropertyField</c>
		/// </summary>
		public static void Draw(this SerializedProperty prop, bool includeChildren, params GUILayoutOption[] options)
		{
			EditorGUILayout.PropertyField(prop, includeChildren, options);
		}

		/// <summary>
		/// Draws this property using <c>EditorGUILayout.PropertyField</c>
		/// </summary>
		public static void Draw(this SerializedProperty prop, GUIContent label, params GUILayoutOption[] options)
		{
			EditorGUILayout.PropertyField(prop, label, options);
		}

		/// <summary>
		/// Draws this property using <c>EditorGUILayout.PropertyField</c>
		/// </summary>
		public static void Draw(this SerializedProperty prop, GUIContent label, bool includeChildren, params GUILayoutOption[] options)
		{
			EditorGUILayout.PropertyField(prop, label, includeChildren, options);
		}
	}
}
