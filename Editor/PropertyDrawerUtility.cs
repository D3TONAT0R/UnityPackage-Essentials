﻿using UnityEssentials;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEssentials.ReflectionUtility;

namespace UnityEssentialsEditor
{
	public static class PropertyDrawerUtility
	{
		private struct FromToType : IEquatable<FromToType>
		{
			public Type from;
			public Type to;

			public FromToType(Type from, Type to)
			{
				this.from = from;
				this.to = to;
			}

			public bool Equals(FromToType other)
			{
				return GetHashCode() == other.GetHashCode();
			}

			public override int GetHashCode()
			{
				return unchecked(from.GetHashCode() * 17 + to.GetHashCode());
			}
		}

		//Object property drawer method from AdvancedObjectSelector package
		private static MethodInfo advancedObjectPropertyDrawer;

		private static Dictionary<Type, Type> propertyDrawerTypes = new Dictionary<Type, Type>();
		private static Dictionary<Type, PropertyDrawer> propertyDrawersCache = new Dictionary<Type, PropertyDrawer>();

		private static Dictionary<FromToType, bool> assignabilityToGenericTypeCache = new Dictionary<FromToType, bool>();

		[System.Diagnostics.DebuggerHidden]
		[InitializeOnLoadMethod]
		public static void Init()
		{
			if(!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				try
				{
					var assembly = Assembly.Load("D3T.AdvancedObjectSelector");
					if(assembly != null)
					{
						advancedObjectPropertyDrawer = assembly.GetType("AdvancedObjectSelector.ObjectPropertyDrawer")
							.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Static);
					}
				}
				catch
				{
					
				}

				propertyDrawerTypes = new Dictionary<Type, Type>();
				foreach(var propertyDrawerType in GetClassesOfType(typeof(PropertyDrawer), true))
				{
					var attributes = propertyDrawerType.GetCustomAttributes<CustomPropertyDrawer>(false);
					if(attributes == null || attributes.Count() == 0)
					{
						//Extend search to include ancestors
						attributes = propertyDrawerType.GetCustomAttributes<CustomPropertyDrawer>(true);
					}
					if(attributes != null && attributes.Count() > 0)
					{
						var attribute = attributes.First();
						Type targetType = (Type)attribute.GetType().GetField("m_Type", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(attribute);
						if(!propertyDrawerTypes.ContainsKey(targetType))
						{
							propertyDrawerTypes.Add(targetType, propertyDrawerType);
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the object the property represents.
		/// </summary>
		public static object GetTargetObjectOfProperty(SerializedProperty prop)
		{
			if(prop == null) return null;

#if UNITY_2022_1_OR_NEWER
			if(prop.isArray) return GetPropertyValueViaReflection(prop);
			else return prop.boxedValue;
#else
			return GetPropertyValueViaReflection(prop);
#endif
		}

		public static object GetPropertyValueViaReflection(SerializedProperty prop)
		{
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

		private static PropertyInfo GetIndexer(Type type)
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
			var list = (IList)collection;
			return list[index];
			/*
			var enumerable = collection as System.Collections.IEnumerable;
			if(enumerable == null) return null;
			var enumerator = enumerable.GetEnumerator();
			for(int i = 0; i <= index; i++)
			{
				if(!enumerator.MoveNext()) return null;
			}
			return enumerator.Current;
			*/
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

		private static bool IsAssignableToGenericType(Type givenType, Type genericType)
		{
			var fromTo = new FromToType(givenType, genericType);

			if(assignabilityToGenericTypeCache.TryGetValue(fromTo, out var result))
			{
				return result;
			}

			var interfaceTypes = givenType.GetInterfacesNonAlloc();

			foreach(var it in interfaceTypes)
			{
				if(it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
					return true;
			}

			if(givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
				return true;

			Type baseType = givenType.BaseType;
			if(baseType == null) return false;

			result = IsAssignableToGenericType(baseType, genericType);
			assignabilityToGenericTypeCache[new FromToType(baseType, genericType)] = result;
			return result;
		}

		private static Type GetPropertyDrawerType(Type objectOrAttributeType)
		{
			foreach(var kv in propertyDrawerTypes)
			{
				if(kv.Key == objectOrAttributeType)
				{
					return kv.Value;
				}
			}
			//TODO: check if property drawer is allowed for child classes
			foreach(var kv in propertyDrawerTypes)
			{
				if(kv.Key.IsGenericType)
				{
					if(IsAssignableToGenericType(objectOrAttributeType, kv.Key)) return kv.Value;
				}
				else if(kv.Key.IsAssignableFrom(objectOrAttributeType))
				{
					return kv.Value;
				}
			}
			return null;
		}

		public static PropertyDrawer GetPropertyDrawerFromType(Type objectOrAttributeType)
		{
			Type drawerType = GetPropertyDrawerType(objectOrAttributeType);

			if(drawerType != null)
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

		public static void DrawPropertyWithAttributeExcept(Rect rect, SerializedProperty property, GUIContent label, Type exceptType, int minimumOrder)
		{
			var drawer = GetDecoratedPropertyDrawerExcept(property, exceptType, minimumOrder);
			if(drawer != null)
			{
				drawer.OnGUI(rect, property, label);
			}
			else
			{
				DrawPropertyField(rect, property, label);
			}
		}

		public static float GetPropertyHeightWithAttributeExcept(SerializedProperty property, GUIContent label, Type exceptType, int minimumOrder)
		{
			var drawer = GetDecoratedPropertyDrawerExcept(property, exceptType, minimumOrder) ?? GetPropertyDrawerFromType(GetTypeOfProperty(property));
			if(drawer != null)
			{
				return drawer.GetPropertyHeight(property, label);
			}
			else
			{
				return EditorGUI.GetPropertyHeight(property);
			}
		}

		private static PropertyDrawer GetDecoratedPropertyDrawerExcept(SerializedProperty property, Type exceptType, int minimumOrder)
		{
			var attrs = GetPropertyAttributes(property, out var fieldInfo).Where(attr =>
			{
				var t = attr.GetType();
				return !typeof(DecoratorAttribute).IsAssignableFrom(t)
				&& attr.order >= minimumOrder
				&& t != exceptType
				&& t != typeof(TooltipAttribute);
			}).OrderBy(a => a.order).ToArray();
			if(attrs.Length > 0)
			{
				if(attrs.Length > 1)
				{
					Debug.LogWarning($"More than one PropertyAttribute detected on '{property.name}': [{string.Join(", ", attrs.Select(a => a.GetType().Name))}]");
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
			var elementType = GetElementType(fieldType, out _);
			if(typeof(UnityEngine.Object).IsAssignableFrom(elementType) && advancedObjectPropertyDrawer != null)
			{
				advancedObjectPropertyDrawer.Invoke(null, new object[] { rect, property, label, elementType });
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
				case SerializedPropertyType.Enum: prop.intValue = Convert.ToInt32(value); break;
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
				case SerializedPropertyType.Integer:
					HandleDirectPropertyDraw(
						() => EditorGUI.IntField(position, label, prop.intValue, textFieldStyle),
						i => prop.intValue = i);
					break;
				case SerializedPropertyType.Boolean:
					HandleDirectPropertyDraw(
						() => EditorGUI.Toggle(position, label, prop.boolValue, textFieldStyle),
						b => prop.boolValue = b);
					break;
				case SerializedPropertyType.Float:
					HandleDirectPropertyDraw(
						() => EditorGUI.FloatField(position, label, prop.floatValue, textFieldStyle),
						f => prop.floatValue = f);
					break;
				case SerializedPropertyType.String:
					HandleDirectPropertyDraw(
						() => EditorGUI.TextField(position, label, prop.stringValue, textFieldStyle),
						s => prop.stringValue = s);
					break;
				case SerializedPropertyType.Color:
					HandleDirectPropertyDraw(
						() => EditorGUI.ColorField(position, label, prop.colorValue),
						c => prop.colorValue = c);
					break;
				case SerializedPropertyType.ObjectReference:
					HandleDirectPropertyDraw(
						() => EditorGUI.ObjectField(position, label, prop.objectReferenceValue, GetTypeOfProperty(prop), true),
						o => prop.objectReferenceValue = o);
					break;
				case SerializedPropertyType.LayerMask:
					HandleDirectPropertyDraw(
						() => EditorGUI.LayerField(position, label, prop.intValue),
						l => prop.intValue = l);
					break;
				case SerializedPropertyType.Enum:
					HandleDirectPropertyDraw(
						() => EditorGUI.EnumPopup(position, label, (Enum)Enum.ToObject(prop.GetManagedType(), prop.intValue)),
						e => prop.intValue = Convert.ToInt32(e));
					break;
				case SerializedPropertyType.Vector2:
					HandleDirectPropertyDraw(
						() => EditorGUI.Vector2Field(position, label, prop.vector2Value),
						v => prop.vector2Value = v);
					break;
				case SerializedPropertyType.Vector3:
					HandleDirectPropertyDraw(
						() => EditorGUI.Vector3Field(position, label, prop.vector3Value),
						v => prop.vector3Value = v);
					break;
				case SerializedPropertyType.Vector4:
					HandleDirectPropertyDraw(
						() => EditorGUI.Vector4Field(position, label, prop.vector4Value),
						v => prop.vector4Value = v);
					break;
				case SerializedPropertyType.Rect:
					HandleDirectPropertyDraw(
						() => EditorGUI.RectField(position, label, prop.rectValue),
						r => prop.rectValue = r);
					break;
				case SerializedPropertyType.ArraySize:
					HandleDirectPropertyDraw(
						() => EditorGUI.IntField(position, label, prop.arraySize, textFieldStyle),
						s => prop.arraySize = s);
					break;
				case SerializedPropertyType.Character:
					HandleDirectPropertyDraw(
						() => EditorGUI.TextField(position, label, prop.stringValue, textFieldStyle),
						s => prop.stringValue = s);
					break;
				case SerializedPropertyType.AnimationCurve:
					HandleDirectPropertyDraw(
						() => EditorGUI.CurveField(position, label, prop.animationCurveValue),
						c => prop.animationCurveValue = c);
					break;
				case SerializedPropertyType.Bounds:
					HandleDirectPropertyDraw(
						() => EditorGUI.BoundsField(position, label, prop.boundsValue),
						b => prop.boundsValue = b);
					break;
				case SerializedPropertyType.Gradient:
					HandleDirectPropertyDraw(
						() => EditorGUI.GradientField(position, label, GetGradientValue(prop)),
						g => SetGradientValue(prop, g));
					break;
				case SerializedPropertyType.Quaternion:
					HandleDirectPropertyDraw(
						() => EditorGUI.Vector4Field(position, label, prop.quaternionValue.ToVector4()),
						v => prop.quaternionValue = v.ToQuaternion());
					break;
				case SerializedPropertyType.ExposedReference:
					HandleDirectPropertyDraw(
						() => EditorGUI.ObjectField(position, label, prop.exposedReferenceValue, GetTypeOfProperty(prop), true),
						o => prop.exposedReferenceValue = o);
					break;
				case SerializedPropertyType.FixedBufferSize:
					throw new InvalidOperationException("Read only.");
				case SerializedPropertyType.Vector2Int:
					HandleDirectPropertyDraw(
						() => EditorGUI.Vector2IntField(position, label, prop.vector2IntValue),
						v => prop.vector2IntValue = v);
					break;
				case SerializedPropertyType.Vector3Int:
					HandleDirectPropertyDraw(
						() => EditorGUI.Vector3IntField(position, label, prop.vector3IntValue),
						v => prop.vector3IntValue = v);
					break;
				case SerializedPropertyType.RectInt:
					HandleDirectPropertyDraw(
						() => EditorGUI.RectIntField(position, label, prop.rectIntValue),
						r => prop.rectIntValue = r);
					break;
				case SerializedPropertyType.BoundsInt:
					HandleDirectPropertyDraw(
						() => EditorGUI.BoundsIntField(position, label, prop.boundsIntValue),
						b => prop.boundsIntValue = b);
					break;
				case SerializedPropertyType.ManagedReference:
					throw new NotImplementedException();
					break;
				default: throw new NotImplementedException();
			}
		}

		private static void HandleDirectPropertyDraw<T>(Func<T> drawer, Action<T> applier)
		{
			EditorGUI.BeginChangeCheck();
			T newValue = drawer();
			if(EditorGUI.EndChangeCheck())
			{
				applier(newValue);
			}
		}

		public static float DrawChildProperties(Rect position, SerializedProperty parent)
		{
			float yOffset = 0;
			foreach(SerializedProperty child in parent)
			{
				position.height = EditorGUI.GetPropertyHeight(child);
				EditorGUI.PropertyField(position, child, new GUIContent(child.displayName));
				float off = position.height + EditorGUIUtility.standardVerticalSpacing;
				position.y += off;
				yOffset += off;
			}
			return yOffset;
		}

		public static void DrawChildProperties(SerializedProperty parent)
		{
			foreach(SerializedProperty child in parent)
			{
				EditorGUILayout.PropertyField(child, new GUIContent(child.displayName));
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

		public static bool ValidatePropertyTypeForAttribute(Rect position, SerializedProperty property, GUIContent label, params SerializedPropertyType[] types)
		{
			var type = property.propertyType;
			foreach(var t in types)
			{
				if(type == t)
				{
					return true;
				}
			}
			EditorGUIExtras.ErrorLabelField(position, label, new GUIContent("(Invalid Attribute Usage)"));
			return false;
		}

		public static bool ValidatePropertyTypeForAttribute(Rect position, SerializedProperty property, GUIContent label, params Type[] types)
		{
			var type = GetTypeOfProperty(property);
			foreach(var t in types)
			{
				if(t.IsAssignableFrom(type))
				{
					return true;
				}
			}
			EditorGUIExtras.ErrorLabelField(position, label, new GUIContent("(Invalid Attribute Usage)"));
			return false;
		}

		public static Type GetElementType(Type type, out bool isArrayOrList)
		{
			if(type.IsArray)
			{
				isArrayOrList = true;
				return type.GetElementType();
			}
			else if(typeof(IList).IsAssignableFrom(type) && type.IsGenericType)
			{
				isArrayOrList = true;
				return type.GetGenericArguments()[0];
			}
			isArrayOrList = false;
			return type;
		}
	}
}
