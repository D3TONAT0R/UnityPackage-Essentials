using UnityEssentials.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEssentials.Reflection;
using Object = UnityEngine.Object;

namespace UnityEssentialsEditor.PropertyDrawers
{

	[CustomPropertyDrawer(typeof(PolymorphicList<>), true)]
	public class PolymorphicListDrawer : PropertyDrawer
	{
		private static Dictionary<Type, Type[]> polymorphicTypes = new Dictionary<Type, Type[]>();
		private Dictionary<string, ReorderableList> reorderableLists = new Dictionary<string, ReorderableList>();
		private static List<object> listObjects = new List<object>();

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var listProp = property.FindPropertyRelative(nameof(PolymorphicList<object>.list));
			EditorGUI.BeginProperty(position, GUIContent.none, property);
			
			if (listProp == null)
			{
				var v = property.GetValueType();
				var listField = v?.GetField(nameof(PolymorphicList<object>.list), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				var listElementType = listField?.FieldType.GenericTypeArguments[0];
				Debug.LogError($"PolymorphicListDrawer: The base list type ({listElementType?.Name}) must be marked with [System.Serializable]", property.serializedObject.targetObject);
				EditorGUI.EndProperty();
				return;
			}

			object target = property.GetValue();
			var baseType = target.GetType();
			var polymorphicAttr = baseType.GetCustomAttribute<PolymorphicAttribute>();

			// Ensure polymorphic types are cached
			if (!polymorphicTypes.ContainsKey(baseType))
			{
				CachePolymorphicTypes(baseType, polymorphicAttr, property.serializedObject.targetObject);
			}

			// Get or create reorderable list
			string listKey = property.propertyPath;
			if (!reorderableLists.ContainsKey(listKey))
			{
				var reorderableList = new ReorderableList(property.serializedObject, listProp, true, true, true, true);
				SetupReorderableListCallbacks(reorderableList, baseType, listProp, property.serializedObject);
				reorderableLists[listKey] = reorderableList;
			}

			CheckForDuplicates(listProp);
			reorderableLists[listKey].DoList(position);

			EditorGUI.EndProperty();
		}

		private void CachePolymorphicTypes(Type baseType, PolymorphicAttribute polymorphicAttr, Object targetObject)
		{
			var baseGenericType = baseType.BaseType;
			if (baseGenericType == null)
			{
				Debug.LogError("PolymorphicListDrawer: Could not determine base generic type", targetObject);
				polymorphicTypes.Add(baseType, new Type[0]);
				return;
			}

			var valueType = baseGenericType.GetGenericArguments()[0];
			if ((valueType == typeof(object) || valueType == typeof(Object)) && polymorphicAttr?.specificTypes == null)
			{
				Debug.LogError("When creating a PolymorphicList for System.Object or UnityEngine.Object you must specify the subtypes allowed.", targetObject);
				polymorphicTypes.Add(baseType, new Type[0]);
				return;
			}

			Type[] listedTypes;
			if (polymorphicAttr != null && polymorphicAttr.specificTypes != null)
			{
				listedTypes = polymorphicAttr.specificTypes.Where((t) => !t.IsAbstract && !t.IsInterface).ToArray();
			}
			else
			{
				listedTypes = ReflectionUtility.GetClassesOfType(valueType, true).ToArray();
				if (listedTypes.Length > 50)
					Debug.LogWarning($"Excessive number of valid subtypes detected ({listedTypes.Length}), try specifying specific types.", targetObject);
			}
			polymorphicTypes.Add(baseType, listedTypes);
		}

		private void SetupReorderableListCallbacks(ReorderableList list, Type baseType, SerializedProperty listProp, SerializedObject so)
		{
			// Draw header
			list.drawHeaderCallback = rect =>
			{
				EditorGUI.LabelField(rect, fieldInfo.Name, EditorStyles.label);
			};

			// Draw element
			list.drawElementCallback = (rect, index, _, _) =>
			{
				rect.xMin += 10; // Indent
				var element = listProp.GetArrayElementAtIndex(index);
				var elementType = element.GetValue()?.GetType();
				string typeName = ObjectNames.NicifyVariableName(elementType?.Name ?? "(Null)");
				var label = new GUIContent($"Element {index} ({typeName})");
				EditorGUI.PropertyField(rect, element, label, true);
			};

			// Handle add button - show polymorphic type menu
			list.onAddDropdownCallback = (_, _) =>
			{
				var menu = new GenericMenu();
				foreach (var t in polymorphicTypes[baseType])
				{
					var typeToAdd = t;
					menu.AddItem(new GUIContent(GetTypeName(typeToAdd)), false, () =>
					{
						list.serializedProperty.InsertArrayElementAtIndex(list.serializedProperty.arraySize);
						var newElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
						newElement.managedReferenceValue = Activator.CreateInstance(typeToAdd);
						so.ApplyModifiedProperties();
						Undo.RecordObject(so.targetObject, "Add List Element");
					});
				}
				if (menu.GetItemCount() == 0)
					menu.AddDisabledItem(new GUIContent("None"));
				menu.ShowAsContext();
			};

			// Height calculation
			list.elementHeightCallback = index =>
			{
				return EditorGUI.GetPropertyHeight(listProp.GetArrayElementAtIndex(index)) + EditorGUIUtility.standardVerticalSpacing;
			};
		}

		private void CheckForDuplicates(SerializedProperty list)
		{
			if (Application.isPlaying) return;
			listObjects.Clear();
			if (list == null) return;
			for (int i = 0; i < list.arraySize; i++)
			{
				var elem = list.GetArrayElementAtIndex(i);
				var obj = elem.GetValue();
				if (!listObjects.Contains(obj))
				{
					listObjects.Add(obj);
				}
				else
				{
					if (obj == null) continue;
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
			if (typeName.EndsWith("Value")) typeName = typeName.Substring(0, typeName.Length - "Value".Length);
			return typeName;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var listProp = property.FindPropertyRelative(nameof(PolymorphicList<object>.list));
			if (listProp == null) return EditorGUIUtility.singleLineHeight;

			string listKey = property.propertyPath;
			// Only use cached list if it exists and the serialized object is still valid
			if (reorderableLists.ContainsKey(listKey) && property.serializedObject != null)
			{
				try
				{
					return reorderableLists[listKey].GetHeight();
				}
				catch
				{
					// If GetHeight fails (stale reference), remove from cache
					reorderableLists.Remove(listKey);
				}
			}

			return EditorGUIUtility.singleLineHeight;
		}
	}
}