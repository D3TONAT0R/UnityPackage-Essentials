using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor.Inspector
{
	//CustomEditor(typeof(MonoBehaviour), true)]
	public class ExtendedMonoBehaviourEditor : Editor
	{
		private static Dictionary<Type, PropertyInfo[]> exposedProperties = new Dictionary<Type, PropertyInfo[]>();

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var properties = GetExposedProperties(target);
			if(properties.Length > 0)
			{
				EditorGUILayout.Space(10);
				EditorGUILayout.LabelField("Exposed Properties", EditorStyles.boldLabel);
				int staticMembers = 0;
				foreach(var prop in properties)
				{
					var attribute = prop.GetCustomAttribute<ShowInInspectorAttribute>();
					bool isStatic = prop.GetGetMethod(true).IsStatic;
					var rect = EditorGUILayout.GetControlRect();
					var value = prop.GetValue(isStatic ? null : target);
					string name = attribute.customLabel ?? ObjectNames.NicifyVariableName(prop.Name);
					if(isStatic)
					{
						name += " *";
						staticMembers++;
					}
					GUI.enabled = true;
					rect.SplitHorizontal(EditorGUIUtility.labelWidth, out var labelPos, out var fieldPos, 4);
					EditorGUI.LabelField(labelPos, name);

					bool editable = attribute.editableAtRuntime && EditorApplication.isPlaying && prop.SetMethod != null;
					GUI.enabled = editable;
					EditorGUI.BeginChangeCheck();
					value = DrawProperty(fieldPos, prop.PropertyType, value);
					if(EditorGUI.EndChangeCheck())
					{
						prop.SetValue(isStatic ? null : target, value);
					}
					GUI.enabled = true;
				}
				if(staticMembers > 0)
				{
					EditorGUILayout.Space(2);
					GUILayout.Label("* : Static Members", EditorStyles.miniLabel);
				}
			}
		}

		private PropertyInfo[] GetExposedProperties(object target)
		{
			var type = target.GetType();
			if(exposedProperties.TryGetValue(type, out var properties))
			{
				return properties;
			}
			else
			{
				var props = type.GetProperties(ReflectionUtility.allInclusiveBindingFlags).Where(p => p.GetCustomAttribute<ShowInInspectorAttribute>() != null && p.GetGetMethod(true) != null).ToArray();
				exposedProperties[type] = props;
				return props;
			}
		}

		private object DrawProperty(Rect pos, Type type, object value)
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

		private bool IsIntegerType(Type t)
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

		private bool IsFloatingPointType(Type t)
		{
			return t == typeof(float)
				|| t == typeof(double)
				|| t == typeof(decimal);
		}
	}
}
