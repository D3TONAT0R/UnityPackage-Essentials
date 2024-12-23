using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(PropertyView))]
	public class PropertyViewDrawer : PropertyDrawer
	{
		private class ExposedProperty
		{
			public readonly PropertyInfo property;
			public readonly ShowInInspectorAttribute attribute;

			public ExposedProperty(PropertyInfo prop)
			{
				property = prop;
				attribute = prop.GetCustomAttribute<ShowInInspectorAttribute>();
			}
		}

		private static Dictionary<Type, ExposedProperty[]> exposedProperties = new Dictionary<Type, ExposedProperty[]>();

		private static List<ExposedProperty> propertiesToShow = new List<ExposedProperty>();

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GatherPropertiesToShow(property);
			position.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField(position, label, EditorStyles.boldLabel);
			var parent = property.GetParentObject();
			foreach (var prop in propertiesToShow)
			{
				position.NextProperty();
				DrawExposedPropertyField(position, prop, parent);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			int lines = 1;
			GatherPropertiesToShow(property);
			foreach(var prop in propertiesToShow)
			{
				lines += 1;
			}
			return lines * EditorGUIUtility.singleLineHeight + (lines - 1) * EditorGUIUtility.standardVerticalSpacing;
		}

		private static void DrawExposedPropertyField(Rect position, ExposedProperty prop, object parent)
		{
			bool isStatic = prop.property.GetGetMethod(true).IsStatic;
			var value = prop.property.GetValue(isStatic ? null : parent);
			string name = prop.attribute.customLabel ?? ObjectNames.NicifyVariableName(prop.property.Name);
			if(isStatic)
			{
				name += " *";
			}
			GUI.enabled = true;
			position.SplitHorizontal(EditorGUIUtility.labelWidth, out var labelPos, out var fieldPos, 4);
			EditorGUI.LabelField(labelPos, name);

			bool editable = prop.attribute.editableAtRuntime && EditorApplication.isPlaying && prop.property.SetMethod != null;
			GUI.enabled = editable;
			EditorGUI.BeginChangeCheck();
			value = DrawProperty(fieldPos, prop.property.PropertyType, value);
			if(EditorGUI.EndChangeCheck())
			{
				prop.property.SetValue(isStatic ? null : parent, value);
			}
			GUI.enabled = true;
		}

		private static void GatherPropertiesToShow(SerializedProperty property)
		{
			var name = property.name;
			var parent = property.GetParentObject();
			propertiesToShow.Clear();
			foreach(var prop in GetExposedProperties(parent))
			{
				if(string.IsNullOrEmpty(prop.attribute.category) || prop.attribute.category == name)
				{
					propertiesToShow.Add(prop);
				}
			}
		}

		private static ExposedProperty[] GetExposedProperties(object target)
		{
			var type = target.GetType();
			if(exposedProperties.TryGetValue(type, out var properties))
			{
				return properties;
			}
			else
			{
				var props = type.GetProperties(ReflectionUtility.allInclusiveBindingFlags)
					.Where(p => p.GetCustomAttribute<ShowInInspectorAttribute>() != null && p.GetGetMethod(true) != null)
					.Select(p => new ExposedProperty(p))
					.ToArray();
				exposedProperties[type] = props;
				return props;
			}
		}

		private static object DrawProperty(Rect pos, Type type, object value)
		{
			if(type == typeof(bool)) return EditorGUI.Toggle(pos, (bool)value);
			else if(IsIntegerType(type)) return EditorGUI.LongField(pos, Convert.ToInt64(value));
			else if(IsFloatingPointType(type)) return EditorGUI.DoubleField(pos, Convert.ToDouble(value));
			else if(type == typeof(string)) return EditorGUI.TextField(pos, (string)value);
			else if(type == typeof(Vector2)) return EditorGUI.Vector2Field(pos, GUIContent.none, (Vector2)value);
			else if(type == typeof(Vector3)) return EditorGUI.Vector3Field(pos, GUIContent.none, (Vector3)value);
			else if(type == typeof(Vector4)) return EditorGUI.Vector4Field(pos, GUIContent.none, (Vector4)value);
			else if(type == typeof(Quaternion)) return EditorGUI.Vector4Field(pos, GUIContent.none, new Vector4(((Quaternion)value).x, ((Quaternion)value).y, ((Quaternion)value).z, ((Quaternion)value).w));
			else if(type == typeof(Color)) return EditorGUI.ColorField(pos, (Color)value);
			else if(type == typeof(Color32)) return EditorGUI.ColorField(pos, (Color32)value);
			else if(type == typeof(Rect)) return EditorGUI.RectField(pos, (Rect)value);
			else if(type == typeof(Bounds)) return EditorGUI.BoundsField(pos, (Bounds)value);
			else if(type == typeof(LayerMask)) return EditorGUI.LayerField(pos, (LayerMask)value);
			else if(type == typeof(AnimationCurve)) return EditorGUI.CurveField(pos, (AnimationCurve)value);
			else if(type == typeof(Gradient)) return EditorGUI.GradientField(pos, (Gradient)value);
			else if(type == typeof(RectInt)) return EditorGUI.RectIntField(pos, (RectInt)value);
			else if(typeof(UnityEngine.Object).IsAssignableFrom(type))
			{
				bool allowSceneObjects = typeof(Component).IsAssignableFrom(type) || type == typeof(GameObject);
				return EditorGUI.ObjectField(pos, (UnityEngine.Object)value, type, allowSceneObjects);
			}
			else
			{
				EditorGUI.LabelField(pos, value?.ToString() ?? "(null)");
				return value;
			}
		}

		private static bool IsIntegerType(Type t)
		{
			return t == typeof(byte)
				|| t == typeof(sbyte)
				|| t == typeof(short)
				|| t == typeof(ushort)
				|| t == typeof(int)
				|| t == typeof(uint)
				|| t == typeof(long)
				|| t == typeof(ulong);
		}

		private static bool IsFloatingPointType(Type t)
		{
			return t == typeof(float)
				|| t == typeof(double)
				|| t == typeof(decimal);
		}
	}
}