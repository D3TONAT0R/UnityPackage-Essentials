using D3T;
using D3T.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static D3T.Utility.ReflectionUtility;

namespace D3TEditor
{
	public static class PropertyDrawerUtility
	{

		const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		//Object property drawer method from AdvancedObjectSelector package
		static MethodInfo advancedObjectPropertyDrawer;

		static Dictionary<Type, Type> propertyDrawerTypes = new Dictionary<Type, Type>();
		static Dictionary<Type, PropertyDrawer> propertyDrawersCache = new Dictionary<Type, PropertyDrawer>();

		[System.Diagnostics.DebuggerHidden]
		[InitializeOnLoadMethod]
		public static void Init()
		{
#if D3T_ADVSELECTOR
			var a = Assembly.Load("D3T.AdvancedObjectSelector");
			advancedObjectPropertyDrawer = a.GetType("AdvancedObjectSelector.ObjectPropertyDrawer").GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Static);
#endif
			propertyDrawerTypes = new Dictionary<Type, Type>();
			foreach(var pdType in GetClassesOfType(typeof(PropertyDrawer), true))
			{
				var attr = pdType.GetCustomAttribute<CustomPropertyDrawer>(true);
				if(attr != null)
				{
					Type targetType = (Type)attr.GetType().GetField("m_Type", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(attr);
					propertyDrawerTypes.Add(targetType, pdType);
				}
			}
		}

		/// <summary>
		/// Gets the object the property represents.
		/// </summary>
		public static object GetTargetObjectOfProperty(SerializedProperty prop)
		{
			if(prop == null) return null;

			var path = prop.propertyPath.Replace(".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split('.');
			foreach(var element in elements)
			{
				if(element.Contains("["))
				{
					var elementName = element.Substring(0, element.IndexOf("["));
					if(elementName.Length > 0)
					{
						obj = GetMemberValueByName(obj, elementName);
					}
					var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
					obj = GetElementAtIndex(obj, index);
				}
				else
				{
					obj = GetMemberValueByName(obj, element);
				}
			}
			return obj;
		}

		/// <summary>
		/// Gets the type of the given property.
		/// </summary>
		public static Type GetTypeOfProperty(SerializedProperty property)
		{
			var path = property.propertyPath;
			System.Type parentType = property.serializedObject.targetObject.GetType();
			object obj = property.serializedObject.targetObject;
			while(path.Contains("."))
			{
				string root = path.Split('.')[0];
				if(path.StartsWith("Array.data["))
				{
					path = path.Substring("Array.data[".Length);
					string indexString = "";
					while(path[0] != ']')
					{
						indexString += path[0];
						path = path.Substring(1);
					}
					int index = int.Parse(indexString);
					path = path.Substring(1);
					if(path.Length > 0 && path[0] == '.') path = path.Substring(1);
					if(parentType.IsArray)
					{
						//It's a regular array
						var arr = obj as System.Array;
						if(index >= 0 && index < arr.Length)
						{
							obj = arr.GetValue(index);
						}
						else
						{
							obj = null;
						}
						parentType = parentType.GetElementType();
					}
					else
					{
						//It's a List
						var indexer = GetIndexer(obj.GetType());
						obj = indexer.GetGetMethod().Invoke(obj, new object[] { index });
						parentType = parentType.GenericTypeArguments[0];
					}
					if(path.Length == 0) return parentType;
				}
				else
				{
					obj = parentType.GetField(root, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(obj);
					path = path.Substring(root.Length + 1);
				}
				parentType = obj.GetType();
			}
			var fieldInfo = (FieldInfo)FindMemberInType(parentType, path, true);
			return fieldInfo.FieldType;
		}

		static PropertyInfo GetIndexer(Type type)
		{
			foreach(PropertyInfo pi in type.GetProperties())
			{
				if(pi.GetIndexParameters().Length > 0) return pi;
			}
			return null;
		}

		/// <summary>
		/// Gets the object the property represents.
		/// </summary>
		public static T GetTargetObjectOfProperty<T>(SerializedProperty prop)
		{
			return (T)GetTargetObjectOfProperty(prop);
		}

		private static object GetElementAtIndex(object collection, int index)
		{
			var enumerable = collection as System.Collections.IEnumerable;
			if(enumerable == null) return null;
			var enm = enumerable.GetEnumerator();
			//while (index-- >= 0)
			//    enm.MoveNext();
			//return enm.Current;

			for(int i = 0; i <= index; i++)
			{
				if(!enm.MoveNext()) return null;
			}
			return enm.Current;
		}

		public static SerializedProperty GetParentProperty(SerializedProperty prop)
		{
			var path = prop.propertyPath.Replace(".Array.data[", "[");
			var split = path.Split('.').ToList();
			split.RemoveAt(split.Count - 1);
			path = string.Join(".", split);
			return prop.serializedObject.FindProperty(path);
		}

		public static object GetParent(SerializedProperty prop)
		{
			var path = prop.propertyPath.Replace(".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split('.');
			foreach(var element in elements.Take(elements.Length - 1))
			{
				if(element.Contains("["))
				{
					var elementName = element.Substring(0, element.IndexOf("["));
					var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
					obj = GetValue(obj, elementName, index);
				}
				else
				{
					obj = GetValue(obj, element);
				}
			}
			return obj;
		}

		public static object GetValue(object source, string name)
		{
			if(source == null)
				return null;
			var type = source.GetType();
			var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if(f == null)
			{
				var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if(p == null)
					return null;
				return p.GetValue(source, null);
			}
			return f.GetValue(source);
		}

		public static object GetValue(object source, string name, int index)
		{
			var enumerable = GetValue(source, name) as IEnumerable;
			var enm = enumerable.GetEnumerator();
			while(index-- >= 0)
				enm.MoveNext();
			return enm.Current;
		}

		public static T GetAttribute<T>(SerializedProperty prop, bool inherit) where T : Attribute
		{
			var m = GetMemberInfoOfProperty(prop, out var obj);
			return m?.GetCustomAttribute<T>(inherit);
		}

		public static IEnumerable<PropertyAttribute> GetPropertyAttributes(SerializedProperty prop, out FieldInfo fieldInfo)
		{
			var m = GetMemberInfoOfProperty(prop, out var obj);
			fieldInfo = m as FieldInfo;
			return m?.GetCustomAttributes<PropertyAttribute>();
		}

		public static MemberInfo GetMemberInfoOfProperty(SerializedProperty prop, out object obj)
		{
			if(prop == null)
			{
				obj = null;
				return null;
			}

			obj = prop.serializedObject.targetObject;

			MemberInfo m;
			var arrayMatch = System.Text.RegularExpressions.Regex.Match(prop.propertyPath, @"\.Array\.data\[([0-9]+)\]$");
			if(arrayMatch.Success)
			{
				m = GetMember(ref obj, prop.propertyPath.Substring(0, prop.propertyPath.Length - arrayMatch.Length));
			}
			else
			{
				m = GetMember(ref obj, prop.propertyPath);
			}
			return m;
		}

		private static MemberInfo GetMember(ref object obj, string path)
		{
			path = path.Replace("Array.data[", "[");
			var names = new List<string>(path.Split('.'));
			MemberInfo member = obj.GetType();
			while(names.Count > 0)
			{
				if(names[0].StartsWith("["))
				{
					int index = Convert.ToInt32(names[0].Replace("[", "").Replace("]", ""));
					obj = GetElementAtIndex(obj, index);
					names.RemoveAt(0);
				}

				if(obj == null) return null;

				member = FindMemberInType(obj.GetType(), names[0]);
				if(member != null)
				{
					obj = GetMemberValue(member, obj);
				}
				else
				{
					throw new InvalidOperationException($"Failed to get member '{path}'.");
				}
				names.RemoveAt(0);
			}
			return member;
		}

		public static bool TryGetAttribute<T>(SerializedProperty prop, bool inherit, out T attribute) where T : Attribute
		{
			try
			{
				attribute = GetAttribute<T>(prop, inherit);
				return attribute != null;
			}
			catch
			{
				attribute = null;
				return false;
			}
		}

		public static PropertyDrawer GetPropertyDrawerFromType(Type objectOrAttributeType)
		{
			if(propertyDrawerTypes.TryGetValue(objectOrAttributeType, out var drawerType))
			{
				if(propertyDrawersCache.TryGetValue(drawerType, out var pd))
				{
					return pd;
				}
				else
				{
					pd = (PropertyDrawer)Activator.CreateInstance(drawerType);
					propertyDrawersCache.Add(drawerType, pd);
					return pd;
				}
			}
			else
			{
				return null;
			}
		}


		public static void DrawPropertyWithAttributeExcept(Rect rect, SerializedProperty property, GUIContent label, Type exceptType)
		{
			var drawer = GetDecoratedPropertyDrawerExcept(property, exceptType);
			if(drawer != null)
			{
				drawer.OnGUI(rect, property, label);
			}
			else
			{
				DrawPropertyField(rect, property, label);
			}
		}

		public static float GetPropertyHeightWithAttributeExcept(SerializedProperty property, GUIContent label, Type exceptType)
		{
			var drawer = GetDecoratedPropertyDrawerExcept(property, exceptType) ?? GetPropertyDrawerFromType(GetTypeOfProperty(property));
			if(drawer != null)
			{
				return drawer.GetPropertyHeight(property, label);
			}
			else
			{
				return EditorGUI.GetPropertyHeight(property);
			}
		}

		private static PropertyDrawer GetDecoratedPropertyDrawerExcept(SerializedProperty property, Type exceptType)
		{
			var attrs = GetPropertyAttributes(property, out var fieldInfo).Where(attr => {
				var t = attr.GetType();
				return t != exceptType && t != typeof(TooltipAttribute);
			}).ToArray();
			if(attrs.Length > 0)
			{
				if(attrs.Length > 1)
				{
					Debug.LogWarning("More than one PropertyAttribute detected.");
				}

				var drawer = GetPropertyDrawerFromType(attrs[0].GetType());
				if(drawer != null)
				{
					typeof(PropertyDrawer).GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(drawer, attrs[0]);
					typeof(PropertyDrawer).GetField("m_FieldInfo", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(drawer, fieldInfo);
					return drawer;
				}
			}
			return null;
		}

		public static void DrawPropertyField(Rect rect, SerializedProperty property, GUIContent label, Type fieldType)
		{
			if(typeof(UnityEngine.Object).IsAssignableFrom(fieldType) && advancedObjectPropertyDrawer != null)
			{
				advancedObjectPropertyDrawer.Invoke(null, new object[] { rect, property, label, fieldType });
			}
			else
			{
				var drawer = GetPropertyDrawerFromType(GetTypeOfProperty(property));
				if(drawer != null)
				{
					drawer.OnGUI(rect, property, label);
				}
				else
				{
					EditorGUI.PropertyField(rect, property, label, true);
				}
			}
		}

		public static void DrawPropertyField(Rect rect, SerializedProperty property, GUIContent label)
		{
			DrawPropertyField(rect, property, label, GetTypeOfProperty(property));
		}

		public static void CopyPropertyValue(SerializedProperty from, SerializedProperty to)
		{
			switch(from.propertyType)
			{
				case SerializedPropertyType.Integer: to.intValue = from.intValue; break;
				case SerializedPropertyType.Boolean: to.boolValue = from.boolValue; break;
				case SerializedPropertyType.Float: to.floatValue = from.floatValue; break;
				case SerializedPropertyType.String: to.stringValue = from.stringValue; break;
				case SerializedPropertyType.Color: to.colorValue = from.colorValue; break;
				case SerializedPropertyType.ObjectReference: to.objectReferenceValue = from.objectReferenceValue; break;
				case SerializedPropertyType.LayerMask: to.intValue = from.intValue; break;
				case SerializedPropertyType.Enum: to.intValue = from.intValue; break;
				case SerializedPropertyType.Vector2: to.vector2Value = from.vector2Value; break;
				case SerializedPropertyType.Vector3: to.vector3Value = from.vector3Value; break;
				case SerializedPropertyType.Vector4: to.vector4Value = from.vector4Value; break;
				case SerializedPropertyType.Rect: to.rectValue = from.rectValue; break;
				case SerializedPropertyType.ArraySize: to.arraySize = from.arraySize; break;
				case SerializedPropertyType.Character: to.stringValue = from.stringValue; break;
				case SerializedPropertyType.AnimationCurve: to.animationCurveValue = from.animationCurveValue; break;
				case SerializedPropertyType.Bounds: to.boundsValue = from.boundsValue; break;
				case SerializedPropertyType.Gradient: SetGradientValue(to, GetGradientValue(from)); break;
				case SerializedPropertyType.Quaternion: to.quaternionValue = from.quaternionValue; break;
				case SerializedPropertyType.ExposedReference: to.exposedReferenceValue = from.exposedReferenceValue; break;
				case SerializedPropertyType.FixedBufferSize: throw new InvalidOperationException("Read only.");
				case SerializedPropertyType.Vector2Int: to.vector2IntValue = from.vector2IntValue; break;
				case SerializedPropertyType.Vector3Int: to.vector3IntValue = from.vector3IntValue; break;
				case SerializedPropertyType.RectInt: to.rectIntValue = from.rectIntValue; break;
				case SerializedPropertyType.BoundsInt: to.boundsIntValue = from.boundsIntValue; break;
				case SerializedPropertyType.ManagedReference: to.managedReferenceValue = GetTargetObjectOfProperty(from); break;
				default: throw new NotImplementedException();
			}
		}

		public static void SetValue(SerializedProperty prop, object value)
		{
			switch(prop.propertyType)
			{
				case SerializedPropertyType.Integer: prop.intValue = (int)value; break;
				case SerializedPropertyType.Boolean: prop.boolValue = (bool)value; break;
				case SerializedPropertyType.Float: prop.floatValue = (float)value; break;
				case SerializedPropertyType.String: prop.stringValue = (string)value; break;
				case SerializedPropertyType.Color: prop.colorValue = (Color)value; break;
				case SerializedPropertyType.ObjectReference: prop.objectReferenceValue = (UnityEngine.Object)value; break;
				case SerializedPropertyType.LayerMask: prop.intValue = (int)value; break;
				case SerializedPropertyType.Enum: prop.intValue = (int)value; break;
				case SerializedPropertyType.Vector2: prop.vector2Value = (Vector2)value; break;
				case SerializedPropertyType.Vector3: prop.vector3Value = (Vector3)value; break;
				case SerializedPropertyType.Vector4: prop.vector4Value = (Vector4)value; break;
				case SerializedPropertyType.Rect: prop.rectValue = (Rect)value; break;
				case SerializedPropertyType.ArraySize: prop.arraySize = (int)value; break;
				case SerializedPropertyType.Character: prop.stringValue = (string)value; break;
				case SerializedPropertyType.AnimationCurve: prop.animationCurveValue = (AnimationCurve)value; break;
				case SerializedPropertyType.Bounds: prop.boundsValue = (Bounds)value; break;
				case SerializedPropertyType.Gradient: SetGradientValue(prop, (Gradient)value); break;
				case SerializedPropertyType.Quaternion: prop.quaternionValue = (Quaternion)value; break;
				case SerializedPropertyType.ExposedReference: prop.exposedReferenceValue = (UnityEngine.Object)value; break;
				case SerializedPropertyType.FixedBufferSize: throw new InvalidOperationException("Read only.");
				case SerializedPropertyType.Vector2Int: prop.vector2IntValue = (Vector2Int)value; break;
				case SerializedPropertyType.Vector3Int: prop.vector3IntValue = (Vector3Int)value; break;
				case SerializedPropertyType.RectInt: prop.rectIntValue = (RectInt)value; break;
				case SerializedPropertyType.BoundsInt: prop.boundsIntValue = (BoundsInt)value; break;
				case SerializedPropertyType.ManagedReference: prop.managedReferenceValue = value; break;
				default: throw new NotImplementedException();
			}
		}

		public static void DrawPropertyDirect(Rect position, SerializedProperty prop)
		{
			DrawPropertyDirect(position, GUIContent.none, prop);
		}

		public static void DrawPropertyDirect(Rect position, GUIContent label, SerializedProperty prop, bool monospaceTextFields = false)
		{
			var textFieldStyle = monospaceTextFields ? EditorGUIExtras.GetMonospaceTextField(prop) : GUI.skin.textField;
			switch(prop.propertyType)
			{
				case SerializedPropertyType.Integer: prop.intValue = EditorGUI.IntField(position, label, prop.intValue, textFieldStyle); break;
				case SerializedPropertyType.Boolean: prop.boolValue = EditorGUI.Toggle(position, label, prop.boolValue, textFieldStyle); break;
				case SerializedPropertyType.Float: prop.floatValue = EditorGUI.FloatField(position, label, prop.floatValue, textFieldStyle); break;
				case SerializedPropertyType.String: prop.stringValue = EditorGUI.TextField(position, label, prop.stringValue, textFieldStyle); break;
				case SerializedPropertyType.Color: prop.colorValue = EditorGUI.ColorField(position, label, prop.colorValue); break;
				case SerializedPropertyType.ObjectReference: prop.objectReferenceValue = EditorGUI.ObjectField(position, label, prop.objectReferenceValue, GetTypeOfProperty(prop), true); break;
				case SerializedPropertyType.LayerMask: prop.intValue = EditorGUI.LayerField(position, label, prop.intValue); break;
				case SerializedPropertyType.Enum: prop.intValue = (int)(object)EditorGUI.EnumPopup(position, label, (Enum)GetTargetObjectOfProperty(prop), textFieldStyle); break;
				case SerializedPropertyType.Vector2: prop.vector2Value = EditorGUI.Vector2Field(position, label, prop.vector2Value); break;
				case SerializedPropertyType.Vector3: prop.vector3Value = EditorGUI.Vector3Field(position, label, prop.vector3Value); break;
				case SerializedPropertyType.Vector4: prop.vector4Value = EditorGUI.Vector4Field(position, label, prop.vector4Value); break;
				case SerializedPropertyType.Rect: prop.rectValue = EditorGUI.RectField(position, label, prop.rectValue); break;
				case SerializedPropertyType.ArraySize: prop.arraySize = EditorGUI.IntField(position, label, prop.intValue, textFieldStyle); break;
				case SerializedPropertyType.Character: prop.stringValue = EditorGUI.TextField(position, label, prop.stringValue, textFieldStyle); break;
				case SerializedPropertyType.AnimationCurve: prop.animationCurveValue = EditorGUI.CurveField(position, label, prop.animationCurveValue); break;
				case SerializedPropertyType.Bounds: prop.boundsValue = EditorGUI.BoundsField(position, label, prop.boundsValue); break;
				case SerializedPropertyType.Gradient: SetGradientValue(prop, EditorGUI.GradientField(position, label, GetGradientValue(prop))); break;
				case SerializedPropertyType.Quaternion: prop.quaternionValue = EditorGUI.Vector4Field(position, label, prop.quaternionValue.ToVector4()).ToQuaternion(); break;
				case SerializedPropertyType.ExposedReference: throw new NotImplementedException();
				case SerializedPropertyType.FixedBufferSize: throw new InvalidOperationException();
				case SerializedPropertyType.Vector2Int: prop.vector2IntValue = EditorGUI.Vector2IntField(position, label, prop.vector2IntValue); break;
				case SerializedPropertyType.Vector3Int: prop.vector3IntValue = EditorGUI.Vector3IntField(position, label, prop.vector3IntValue); break;
				case SerializedPropertyType.RectInt: prop.rectIntValue = EditorGUI.RectIntField(position, prop.rectIntValue); break;
				case SerializedPropertyType.BoundsInt: prop.boundsIntValue = EditorGUI.BoundsIntField(position, label, prop.boundsIntValue); break;
				case SerializedPropertyType.ManagedReference: throw new NotImplementedException();
				default: throw new NotImplementedException();
			}
		}

		private static Gradient GetGradientValue(SerializedProperty prop)
		{
#if UNITY_2022_1_OR_NEWER
			return prop.gradientValue;
#else
			return (Gradient)typeof(SerializedProperty).GetProperty("gradientValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(prop);
#endif
		}

		private static void SetGradientValue(SerializedProperty prop, Gradient value)
		{
#if UNITY_2022_1_OR_NEWER
			prop.gradientValue = value;
#else
			typeof(SerializedProperty).GetProperty("gradientValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(prop, value);
#endif
		}
	}
}
