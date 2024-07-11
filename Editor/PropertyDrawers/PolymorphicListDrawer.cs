using UnityEssentials;
using UnityEssentials.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{

	//TODO: cleanup required
	[CustomPropertyDrawer(typeof(PolymorphicList<>), true)]
	public class PolymorphicListDrawer : PropertyDrawer
	{
		private static Dictionary<Type, Type[]> polymorphicTypes = new Dictionary<Type, Type[]>();

		private static List<object> listObjects = new List<object>();

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var listProp = property.FindPropertyRelative(nameof(PolymorphicList<object>.list));
			CheckForDuplicates(listProp);
			EditorGUI.BeginProperty(position, GUIContent.none, property);
			var target = PropertyDrawerUtility.GetTargetObjectOfProperty(property);
			var baseType = target.GetType();
			var polymorphicAttr = baseType.GetCustomAttribute<PolymorphicAttribute>();
			bool polymorphic = polymorphicAttr != null;
			if(!polymorphicTypes.ContainsKey(baseType))
			{
				var valueType = target.GetType().BaseType.GetGenericArguments()[0];
				if((valueType == typeof(object) || valueType == typeof(UnityEngine.Object)) && polymorphicAttr.specificTypes == null)
				{
					Debug.LogError("When creating a PolymorphicList for System.Object or UnityEngine.Object you must specify the subtypes allowed in the dictionary.", property.serializedObject.targetObject);
					polymorphicTypes.Add(baseType, new Type[0]);
					return;
				}
				else
				{
					Type[] listedTypes;
					if(polymorphicAttr != null && polymorphicAttr.specificTypes != null)
					{
						listedTypes = polymorphicAttr.specificTypes.Where((t) => !t.IsAbstract && !t.IsInterface).ToArray();
					}
					else
					{
						listedTypes = ReflectionUtility.GetClassesOfType(valueType, true).ToArray();
						if(listedTypes.Length > 50) Debug.LogWarning($"Excessive number of valid subtypes detected ({listedTypes.Length}), try specifying specific types to be allowed in the dictionary.", property.serializedObject.targetObject);
					}
					polymorphicTypes.Add(baseType, listedTypes);
				}
			}
			position.height = EditorGUIUtility.singleLineHeight;
			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
			if(property.isExpanded)
			{
				EditorGUI.indentLevel++;
				for(int i = 0; i < listProp.arraySize; i++)
				{
					var item = listProp.GetArrayElementAtIndex(i);
					position.NextProperty(EditorGUI.GetPropertyHeight(item));
					var type = PropertyDrawerUtility.GetTargetObjectOfProperty(item)?.GetType();
					string typeName = ObjectNames.NicifyVariableName(type?.Name ?? "(Null)");
					//GUI.Box(position, GUIContent.none);
					EditorGUI.PropertyField(position, item, new GUIContent($"Element {i} ({typeName})"), true);
					var pos2 = position;
					pos2.height = 16;
					pos2.xMin = pos2.xMax - 16;
					if(GUI.Button(pos2, EditorGUIUtility.IconContent("d_Toolbar Minus"), "IconButton"))
					{
						listProp.DeleteArrayElementAtIndex(i);
						i--;
					}
				}
				position.NextProperty();
				DrawAddButton(position, target, baseType, polymorphic, property.serializedObject, property.propertyPath);
				EditorGUI.indentLevel--;
			}
			EditorGUI.EndProperty();
		}

		private static void DrawAddButton(Rect position, object target, Type dictionaryType, bool polymorphic, SerializedObject so, string path)
		{
			position.SplitHorizontalRight(16, out _, out var addRect);
			if(GUI.Button(addRect, EditorGUIUtility.IconContent("d_Toolbar Plus More"), "IconButton"))
			{
				var menu = new GenericMenu();
				foreach(var t in polymorphicTypes[dictionaryType])
				{
					menu.AddItem(new GUIContent(GetTypeName(t)), false,
					() =>
					{
						AddItem(so, path, t);
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
				menu.AddItem("Clear", true, false, () =>
				{
					so.FindProperty(path + "." + nameof(PolymorphicList<object>.list)).ClearArray();
					so.ApplyModifiedProperties();
				});
				menu.ShowAsContext();
			}
		}

		private static void AddItem(SerializedObject so, string path, Type valueType)
		{
			var list = so.FindProperty(path + "." + nameof(PolymorphicList<object>.list));
			list.InsertArrayElementAtIndex(list.arraySize);
			list.GetArrayElementAtIndex(list.arraySize - 1).managedReferenceValue = Activator.CreateInstance(valueType);
			so.ApplyModifiedProperties();
			Undo.RecordObject(so.targetObject, "Add List Element");
		}

		private void CheckForDuplicates(SerializedProperty list)
		{
			if(Application.isPlaying) return;
			listObjects.Clear();
			if(list == null) return;
			for(int i = 0; i < list.arraySize; i++)
			{
				var elem = list.GetArrayElementAtIndex(i);
				var obj = PropertyDrawerUtility.GetTargetObjectOfProperty(elem);
				if(!listObjects.Contains(obj))
				{
					listObjects.Add(obj);
				}
				else
				{
					if(obj == null) continue;
					string json = JsonUtility.ToJson(obj);
					var clone = JsonUtility.FromJson(json, obj.GetType());
					elem.managedReferenceValue = clone;
					listObjects.Add(clone);
				}
			}
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
				float h = 2 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
				var listProp = property.FindPropertyRelative(nameof(PolymorphicList<object>.list));
				for(int i = 0; i < listProp.arraySize; i++)
				{
					h += EditorGUI.GetPropertyHeight(listProp.GetArrayElementAtIndex(i)) + EditorGUIUtility.standardVerticalSpacing;
				}
				return h;
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}
	}
}