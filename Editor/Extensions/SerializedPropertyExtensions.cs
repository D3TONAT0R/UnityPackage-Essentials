using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public static class SerializedPropertyExtensions
	{
		/// <summary>
		/// Gets the value object that this SerializedProperty points to.
		/// </summary>
		public static object GetValue(this SerializedProperty prop)
		{
			return PropertyDrawerUtility.GetPropertyValue(prop);
		}

		/// <summary>
		/// Sets the value object that this SerializedProperty points to.
		/// </summary>
		public static void SetValue(this SerializedProperty prop, object value)
		{
			PropertyDrawerUtility.SetPropertyValue(prop, value);
		}

		/// <summary>
		/// Gets the value object that this SerializedProperty points to.
		/// </summary>
		public static T GetValue<T>(this SerializedProperty prop)
		{
			return PropertyDrawerUtility.GetPropertyValue<T>(prop);
		}

		/// <summary>
		/// Gets the parent object containing this SerializedProperty.
		/// </summary>
		public static object GetParentObject(this SerializedProperty prop)
		{
			return PropertyDrawerUtility.GetParentObject(prop);
		}

		/// <summary>
		/// Gets the attribute declared on this SerializedProperty.
		/// </summary>
		public static T GetAttribute<T>(this SerializedProperty prop, bool inherit = true) where T : Attribute
		{
			return PropertyDrawerUtility.GetAttribute<T>(prop, inherit);
		}

		/// <summary>
		/// Gets the attribute declared on this SerializedProperty.
		/// </summary>
		public static bool TryGetAttribute<T>(this SerializedProperty prop, out T attribute) where T : Attribute
		{
			return PropertyDrawerUtility.TryGetAttribute<T>(prop, true, out attribute);
		}

		/// <summary>
		/// Returns the type of the value represented by this property.
		/// </summary>
		public static Type GetValueType(this SerializedProperty prop)
		{
			return PropertyDrawerUtility.GetPropertyType(prop);
		}

		/// <summary>
		/// Gets the attribute declared on this SerializedProperty.
		/// </summary>
		public static bool TryGetAttribute<T>(this SerializedProperty prop, bool inherit, out T attribute) where T : Attribute
		{
			return PropertyDrawerUtility.TryGetAttribute<T>(prop, inherit, out attribute);
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
