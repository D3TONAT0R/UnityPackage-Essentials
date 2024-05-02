using UnityEssentials;
using UnityEssentials.Collections;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEssentials.Utility;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(UnityDictionary<,>), true)]
	public class UnityDictionaryDrawer : PropertyDrawer
	{
		private static Color errorColor = new Color(1, 0.3f, 0.2f);

		private static Dictionary<Type, Type[]> polymorphicTypes = new Dictionary<Type, Type[]>();

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, GUIContent.none, property);
			var target = PropertyDrawerUtility.GetTargetObjectOfProperty(property);
			bool preferMonospaceKeys = (bool)target.GetType().GetProperty(nameof(UnityDictionary<Null, Null>.UseMonospaceKeyLabels)).GetValue(target);
			var dictionaryType = target.GetType();
			var polymorphicAttr = dictionaryType.GetCustomAttribute<PolymorphicAttribute>();
			bool polymorphic = polymorphicAttr != null;
			if(polymorphic)
			{
				if(!polymorphicTypes.ContainsKey(dictionaryType))
				{
					var valueType = ((IUnityDictionary)target).ValueType;
					if((valueType == typeof(object) || valueType == typeof(UnityEngine.Object)) && polymorphicAttr.specificTypes == null)
					{
						Debug.LogError("When creating a polymorphic dictionary for System.Object or UnityEngine.Object you must specify the subtypes allowed in the dictionary.", property.serializedObject.targetObject);
						polymorphicTypes.Add(dictionaryType, new Type[0]);
						return;
					}
					else
					{
						Type[] listedTypes;
						if(polymorphicAttr.specificTypes != null)
						{
							listedTypes = polymorphicAttr.specificTypes.Where((t) => !t.IsAbstract && !t.IsInterface).ToArray();
						}
						else
						{
							listedTypes = ReflectionUtility.GetClassesOfType(valueType, true).ToArray();
							if(listedTypes.Length > 50) Debug.LogWarning($"Excessive number of valid subtypes detected ({listedTypes.Length}), try specifying specific types to be allowed in the dictionary.", property.serializedObject.targetObject);
						}
						polymorphicTypes.Add(dictionaryType, listedTypes);
					}
				}
			}
			var exception = ((IUnityDictionary)target).SerializationException;
			bool valid = exception == null;
			position.height = EditorGUIUtility.singleLineHeight;
			var lColor = GUI.color;
			if(!valid) GUI.color = errorColor;
			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
			GUI.color = lColor;
			if(exception != null)
			{
				var p2 = position;
				p2.xMin += EditorGUIUtility.labelWidth + 2;
				EditorGUI.HelpBox(p2, exception.Message, MessageType.None);
				//Tooltip
				EditorGUI.LabelField(p2, new GUIContent("", exception.Message));
			}
			if(property.isExpanded)
			{
				EditorGUI.indentLevel++;
				int indentLevel = EditorGUI.indentLevel;
				var so = property.serializedObject;
				var keys = property.FindPropertyRelative("_keys");
				var duplicates = PropertyDrawerUtility.GetTargetObjectOfProperty<System.Collections.IList>(keys)
					.Cast<object>()
					.GroupBy(x => x)
					.Where(x => x.Count() > 1)
					.Select(x => x.Key)
					.ToList();
				var values = property.FindPropertyRelative("_values");

				if(keys.arraySize < values.arraySize) keys.arraySize = values.arraySize;
				if(values.arraySize < keys.arraySize) values.arraySize = keys.arraySize;
				int length = keys.arraySize;
				for(int i = 0; i < length; i++)
				{
					var k = keys.GetArrayElementAtIndex(i);
					var kObj = PropertyDrawerUtility.GetTargetObjectOfProperty(k);
					var v = values.GetArrayElementAtIndex(i);
					position.NextProperty();
					lColor = DrawElement(position, property, target, preferMonospaceKeys, dictionaryType, polymorphic, indentLevel, so, keys, duplicates, i, k, kObj, v);
				}
				position.NextProperty();
				DrawAddButton(position, target, dictionaryType, polymorphic, so);
				EditorGUI.indentLevel--;
			}
			EditorGUI.EndProperty();
		}

		private static void DrawAddButton(Rect position, object target, Type dictionaryType, bool polymorphic, SerializedObject so)
		{
			position.SplitHorizontalRight(16, out _, out var addRect);
			if(polymorphic)
			{
				if(GUI.Button(addRect, EditorGUIUtility.IconContent("d_Toolbar Plus More"), "IconButton"))
				{
					var menu = new GenericMenu();
					foreach(var t in polymorphicTypes[dictionaryType])
					{
						menu.AddItem(new GUIContent(GetTypeName(t)), false,
						() =>
						{
							AddItem(target, so, t);
						}
						);
					}
					if(menu.GetItemCount() == 0) menu.AddDisabledItem(new GUIContent("None"));
					menu.ShowAsContext();
				}
				addRect.x -= addRect.width;
				if(GUI.Button(addRect, GUIContent.none, EditorStyles.foldoutHeaderIcon))
				{
					var menu = new GenericMenu();
					menu.AddItem("Clear Dictionary", true, false, () =>
					{
						target.GetType().GetMethod("Clear").Invoke(target, new object[] { });
					});
					menu.ShowAsContext();
				}
			}
			else
			{
				if(GUI.Button(addRect, EditorGUIUtility.IconContent("d_Toolbar Plus"), "IconButton"))
				{
					var valueType = ((IUnityDictionary)target).ValueType;
					EditorApplication.delayCall += () => AddItem(target, so, valueType);
				}
			}
		}

		private static Color DrawElement(Rect position, SerializedProperty property, object target, bool preferMonospaceKeys, Type dictionaryType, bool polymorphic, int indentLevel, SerializedObject so, SerializedProperty keys, List<object> duplicates, int i, SerializedProperty k, object kObj, SerializedProperty v)
		{
			Color lColor;
			position.SplitHorizontal(EditorGUIUtility.labelWidth, out var keyRect, out var valueRect, 0);
			valueRect.xMin += 4;
			valueRect.SplitHorizontalRight(16, out valueRect, out var btnRect, 0);
			valueRect.width -= 4;

			lColor = GUI.color;
			if(duplicates.Contains(k)) GUI.color = errorColor;
			PropertyDrawerUtility.DrawPropertyDirect(keyRect, GUIContent.none, k, preferMonospaceKeys);
			//EditorGUI.PropertyField(keyRect, k, GUIContent.none);
			GUI.color = lColor;

			EditorGUI.indentLevel = 0;
			//TODO: currently only supports the first visible property
			v.isExpanded = true;
			if(v.hasVisibleChildren && v.propertyType == SerializedPropertyType.ManagedReference) v.NextVisible(true);
			if(v.propertyType == SerializedPropertyType.ManagedReference && PropertyDrawerUtility.GetTargetObjectOfProperty(v) == null)
			{
				GUI.Label(valueRect, "(null)");
			}
			else
			{
				valueRect.xMin -= 4;
				var lLW = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 4;
				var content = new GUIContent(" ");
				if(v.type == "Vector4")
				{
					v.vector4Value = EditorGUI.Vector4Field(valueRect, content, v.vector4Value);
				}
				else
				{
					EditorGUI.PropertyField(valueRect, v, content, false);
				}
				EditorGUIUtility.labelWidth = lLW;
			}
			EditorGUI.indentLevel = indentLevel;

			if(GUI.Button(btnRect, GUIContent.none, EditorStyles.foldoutHeaderIcon))
			{
				int index = i;
				var menu = new GenericMenu();
				string path = property.propertyPath;
				menu.AddItem("Move Up", index > 0, false, () =>
				{
					so.Update();
					so.FindProperty(path + "._keys").MoveArrayElement(index, index - 1);
					so.FindProperty(path + "._values").MoveArrayElement(index, index - 1);
					so.ApplyModifiedProperties();
				});
				menu.AddItem("Move Down", index < keys.arraySize - 1, false, () =>
				{
					so.Update();
					so.FindProperty(path + "._keys").MoveArrayElement(index, index + 1);
					so.FindProperty(path + "._values").MoveArrayElement(index, index + 1);
					so.ApplyModifiedProperties();
				});
				if(polymorphic)
				{
					string root = "Change Element Type/";
					foreach(var t in polymorphicTypes[dictionaryType])
					{
						var currentType = PropertyDrawerUtility.GetTargetObjectOfProperty(v)?.GetType();
						menu.AddItem(root + GetTypeName(t), true, currentType == t, () =>
						{
							Undo.RecordObject(so.targetObject, "Change Element Type");
							target.GetType().GetProperties().First(x => x.GetIndexParameters().Length > 0)
								.SetValue(target, Activator.CreateInstance(t), new object[] { kObj });
						});
					}
				}
				menu.AddItem("Delete Element", true, false, () => DeleteItem(target, property, kObj, index));
				menu.ShowAsContext();
			}

			return lColor;
		}

		private static void AddItem(object target, SerializedObject so, Type valueType)
		{
			var adder = target.GetType().GetMethod(nameof(UnityDictionary<Null, Null>.Editor_Add));
			Undo.RecordObject(so.targetObject, "Add Dictionary Element");
			adder.Invoke(target, new object[] { valueType });
		}

		private static void DeleteItem(object target, SerializedProperty prop, object key, int index)
		{
			var remover = target.GetType().GetMethod(nameof(UnityDictionary<Null, Null>.Remove));
			Undo.RecordObject(prop.serializedObject.targetObject, "Delete Dictionary Element");
			remover.Invoke(target, new object[] { key });
			
		}

		private static string GetTypeName(Type t)
		{
			string typeName = t.Name;
			if(typeName.EndsWith("Value")) typeName = typeName.Substring(0, typeName.Length - "Value".Length);
			return typeName;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if(property.isExpanded)
			{
				int count = property.FindPropertyRelative("_keys").arraySize;
				return (count + 2) * EditorGUIUtility.singleLineHeight + (count + 1) * EditorGUIUtility.standardVerticalSpacing;
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}
	}
}