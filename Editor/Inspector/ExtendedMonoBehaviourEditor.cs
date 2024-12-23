using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor.Inspector
{
	[CustomEditor(typeof(MonoBehaviour), true)]
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
				using(new EditorGUI.DisabledScope(true))
				{
					int staticMembers = 0;
					foreach(var prop in properties)
					{
						bool isStatic = prop.GetGetMethod(true).IsStatic;
						var value = prop.GetValue(isStatic ? null : target);
						string name = ObjectNames.NicifyVariableName(prop.Name);
						if(isStatic)
						{
							name += " *";
							staticMembers++;
						}
						DrawProperty(name, prop.PropertyType, value);
					}
					if(staticMembers > 0)
					{
						EditorGUILayout.Space(5);
						GUILayout.Label("* = Static Members", EditorStyles.miniLabel);
					}
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

		private void DrawProperty(string label, Type type, object value)
		{
			if(type == typeof(bool)) EditorGUILayout.Toggle(label, (bool)value);
			else if(IsIntegerType(type)) EditorGUILayout.LongField(label, Convert.ToInt64(value));
			else if(IsFloatingPointType(type)) EditorGUILayout.DoubleField(label, Convert.ToDouble(value));
			else if(type == typeof(string)) EditorGUILayout.TextField(label, (string)value);
			else if(type == typeof(Vector2)) EditorGUILayout.Vector2Field(label, (Vector2)value);
			else if(type == typeof(Vector3)) EditorGUILayout.Vector3Field(label, (Vector3)value);
			else if(type == typeof(Vector4)) EditorGUILayout.Vector4Field(label, (Vector4)value);
			else if(type == typeof(Quaternion)) EditorGUILayout.Vector4Field(label, new Vector4(((Quaternion)value).x, ((Quaternion)value).y, ((Quaternion)value).z, ((Quaternion)value).w));
			else if(type == typeof(Color)) EditorGUILayout.ColorField(label, (Color)value);
			else if(type == typeof(Color32)) EditorGUILayout.ColorField(label, (Color32)value);
			else if(type == typeof(Rect)) EditorGUILayout.RectField(label, (Rect)value);
			else if(type == typeof(Bounds)) EditorGUILayout.BoundsField(label, (Bounds)value);
			else if(type == typeof(LayerMask)) EditorGUILayout.LayerField(label, (LayerMask)value);
			else if(type == typeof(AnimationCurve)) EditorGUILayout.CurveField(label, (AnimationCurve)value);
			else if(type == typeof(Gradient)) EditorGUILayout.GradientField(label, (Gradient)value);
			else if(type == typeof(RectInt)) EditorGUILayout.RectIntField(label, (RectInt)value);
			else if(typeof(UnityEngine.Object).IsAssignableFrom(type)) EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, type, false);
			else EditorGUILayout.LabelField(name, value?.ToString() ?? "(null)");
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
