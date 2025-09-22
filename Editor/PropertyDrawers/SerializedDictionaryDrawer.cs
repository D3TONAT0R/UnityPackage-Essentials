using UnityEssentials;
using UnityEssentials.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(SerializedDictionary<,>), true)]
	public class SerializedDictionaryDrawer : PropertyDrawer
	{
		private const string KEYS_FIELD_NAME = "serializedKeys";
		private const string VALUES_FIELD_NAME = "serializedValues";

		private static Color errorColor = new Color(1, 0.3f, 0.2f);

		private static Dictionary<Type, Type[]> polymorphicTypes = new Dictionary<Type, Type[]>();

		private static GUIStyle indexStyle;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if(property.hasMultipleDifferentValues)
			{
				//If the property has multiple different values, we cannot draw it properly
				EditorGUI.BeginProperty(position, label, property);
				EditorGUI.LabelField(position, label, new GUIContent("—"));
				EditorGUI.EndProperty();
				return;
			}
			if(indexStyle == null)
			{
				indexStyle = new GUIStyle(EditorStyles.miniLabel)
				{
					alignment = TextAnchor.MiddleRight,
					fontSize = 9
				};
				indexStyle.normal.textColor = indexStyle.normal.textColor.MultiplyAlpha(0.4f);
			}
			EditorGUI.BeginProperty(position, GUIContent.none, property);
			var target = property.GetValue();
			var dictionary = (ISerializedDictionary)target;
			bool preferMonospaceKeys = (bool)target.GetType().GetProperty(nameof(SerializedDictionary<Null, Null>.UseMonospaceKeyLabels)).GetValue(target);
			var dictionaryType = target.GetType();
			var polymorphicAttr = dictionaryType.GetCustomAttribute<PolymorphicAttribute>();
			bool polymorphic = polymorphicAttr != null;
			if(polymorphic)
			{
				if(!polymorphicTypes.ContainsKey(dictionaryType))
				{
					var valueType = ((ISerializedDictionary)target).ValueType;
					if((valueType == typeof(object) || valueType == typeof(UnityEngine.Object)) && polymorphicAttr.specificTypes == null)
					{
						Debug.LogError("When creating a polymorphic dictionary for System.Object or UnityEngine.Object you must specify the subtypes allowed in the dictionary.", property.serializedObject.targetObject);
						polymorphicTypes.Add(dictionaryType, Array.Empty<Type>());
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
			var exception = dictionary.SerializationException;
			bool valid = exception == null;
			position.height = EditorGUIUtility.singleLineHeight;
			var lColor = GUI.color;
			if(!valid) GUI.color = errorColor;
			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
			GUI.color = lColor;
			var headerPos = position;
			if(property.isExpanded)
			{
				EditorGUI.indentLevel++;
				int indentLevel = EditorGUI.indentLevel;
				var so = property.serializedObject;
				var keys = property.FindPropertyRelative(KEYS_FIELD_NAME);
				var duplicates = PropertyDrawerUtility.GetPropertyValue<System.Collections.IList>(keys)
					.Cast<object>()
					.GroupBy(x => x)
					.Where(x => x.Count() > 1)
					.Select(x => x.Key)
					.ToList();
				var values = property.FindPropertyRelative(VALUES_FIELD_NAME);

				if(keys.arraySize < values.arraySize) keys.arraySize = values.arraySize;
				if(values.arraySize < keys.arraySize) values.arraySize = keys.arraySize;
				int length = keys.arraySize;
				for(int i = 0; i < length; i++)
				{
					var k = keys.GetArrayElementAtIndex(i);
					var v = values.GetArrayElementAtIndex(i);
					position.NextProperty();
					DrawElement(position, property, preferMonospaceKeys, dictionaryType, polymorphic, indentLevel, so, keys, duplicates, i, k, v);
				}
				position.NextProperty();
				position.height = 16;
				DrawAddButton(position, dictionary, dictionaryType, polymorphic, property);
				EditorGUI.indentLevel--;
			}
			//This needs to be done after the foldout to prevent losing focus on text fields
			headerPos.SplitHorizontalRight(16, out headerPos, out var contextBtnPos);
			contextBtnPos.height = 16;
			var headerLabelPos = headerPos;
			headerLabelPos.xMin += EditorGUIUtility.labelWidth + 2;
			if(exception != null)
			{
				//Show error message
				EditorGUI.HelpBox(headerLabelPos, exception.Message, MessageType.None);
				//Tooltip
				EditorGUI.LabelField(headerLabelPos, new GUIContent("", exception.GetType().Name + "\n" + exception.Message));
			}
			else
			{
				//Show information about the dictionary
				var keyType = dictionary.KeyType;
				var valueType = dictionary.ValueType;
				GUI.Label(headerLabelPos, $"{keyType.Name} / {valueType.Name} ({dictionary.Count} Elements)", EditorStyles.miniLabel);
			}
			if(GUI.Button(contextBtnPos, GUIContent.none, EditorStyles.foldoutHeaderIcon))
			{
				var menu = new GenericMenu();
				var so = property.serializedObject;
				var path = property.propertyPath;
				menu.AddItem("Clear Dictionary", true, false, () =>
				{
					ClearDictionary(so.FindProperty(path));
				});
				menu.ShowAsContext();
			}
			EditorGUI.EndProperty();
		}

		private static void DrawAddButton(Rect position, ISerializedDictionary dictionary, Type dictionaryType, bool polymorphic, SerializedProperty prop)
		{
			var so = prop.serializedObject;
			var path = prop.propertyPath;
			position.SplitHorizontalRight(16, out _, out var addRect);
			if(polymorphic)
			{
				if(GUI.Button(addRect, EditorGUIUtility.IconContent("d_Toolbar Plus More"), "IconButton"))
				{
					var menu = new GenericMenu();
					foreach(var t in polymorphicTypes[dictionaryType])
					{
						menu.AddItem(new GUIContent(GetTypeName(t)), false,
						() => {
							AddItem(so.FindProperty(path), t);
						});
					}
					if(menu.GetItemCount() == 0) menu.AddDisabledItem(new GUIContent("None"));
					menu.ShowAsContext();
				}
			}
			else
			{
				if(GUI.Button(addRect, EditorGUIUtility.IconContent("d_Toolbar Plus"), "IconButton"))
				{
					var valueType = dictionary.ValueType;
					so.Update();
					prop = so.FindProperty(path);
					AddItem(prop, valueType);
					so.ApplyModifiedProperties();
				}
			}
		}

		private static void DrawElement(Rect position, SerializedProperty property, bool preferMonospaceKeys, Type dictionaryType, bool polymorphic, int indentLevel, SerializedObject so, SerializedProperty keys, List<object> duplicates, int i, SerializedProperty k, SerializedProperty v)
		{
			position.SplitHorizontal(EditorGUIUtility.labelWidth, out var keyRect, out var valueRect, 0);
			//Add 4 pixels of space to allow for dragging of values
			valueRect.xMin += 4;
			valueRect.SplitHorizontalRight(16, out valueRect, out var btnRect, 0);
			valueRect.width -= 4;

			var lColor = GUI.color;
			if(duplicates.Contains(k)) GUI.color = errorColor;

			var indexRect = keyRect;
			indexRect = EditorGUI.IndentedRect(indexRect);
			indexRect.xMin -= 27;
			indexRect.width = 25;
			GUI.Label(indexRect, i.ToString(), indexStyle);

#if UNITY_2022_1_OR_NEWER
			if(k.propertyType == SerializedPropertyType.ManagedReference || k.propertyType == SerializedPropertyType.Generic)
#else
			if(k.propertyType == SerializedPropertyType.ManagedReference)
#endif
			{
				EditorGUI.PropertyField(keyRect, k, GUIContent.none, false);
			}
			else
			{
				PropertyDrawerUtility.DrawPropertyDirect(keyRect, GUIContent.none, k, preferMonospaceKeys);
			}

			GUI.color = lColor;

			EditorGUI.indentLevel = 0;
			//TODO: currently only supports the first visible property
			v.isExpanded = true;
			if(v.hasVisibleChildren && v.propertyType == SerializedPropertyType.ManagedReference) v.NextVisible(true);
			if(v.propertyType == SerializedPropertyType.ManagedReference && v.GetValue() == null)
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

			btnRect.height = 16;
			if(GUI.Button(btnRect, GUIContent.none, EditorStyles.foldoutHeaderIcon))
			{
				int index = i;
				var menu = new GenericMenu();
				string path = property.propertyPath;
				menu.AddItem("Move Up", index > 0, false, () =>
				{
					MoveItem(so.FindProperty(path), index, -1);
				});
				menu.AddItem("Move Down", index < keys.arraySize - 1, false, () =>
				{
					MoveItem(so.FindProperty(path), index, 1);
				});
				if(polymorphic)
				{
					string root = "Change Element Type/";
					foreach(var t in polymorphicTypes[dictionaryType])
					{
						var currentType = v.GetValue()?.GetType();
						menu.AddItem(root + GetTypeName(t), true, currentType == t, () =>
						{
							ReplaceValue(so.FindProperty(path), index, Activator.CreateInstance(t));
						});
					}
				}
				menu.AddItem("Delete Element", true, false, () => 
				{
					DeleteItem(so.FindProperty(path), i);
				});
				menu.ShowAsContext();
			}
		}

		private static void AddItem(SerializedProperty prop, Type valueType)
		{
			Undo.RecordObject(prop.serializedObject.targetObject, "Add Dictionary Element");
			prop.serializedObject.Update();
			var k = prop.FindPropertyRelative(KEYS_FIELD_NAME);
			var v = prop.FindPropertyRelative(VALUES_FIELD_NAME);
			k.arraySize++;
			v.arraySize++;
			prop.serializedObject.ApplyModifiedProperties();
			var key = k.GetArrayElementAtIndex(k.arraySize - 1);
			var keyValue = key.GetValue();
			var isEnum = PropertyDrawerUtility.GetPropertyType(key).IsEnum;
			var iList = (IList)k.GetValue();
			//Skip unique key generation for enums since they are near impossible to generate unique keys for
			var uniqueKey = (k.arraySize > 1 && !isEnum) ? GetUniqueKey(keyValue, iList) : keyValue;
			PropertyDrawerUtility.SetPropertyValue(key, uniqueKey);
			var value = v.GetArrayElementAtIndex(v.arraySize - 1);
			var valueValue = CreateValueIfNeeded(valueType);
			PropertyDrawerUtility.SetPropertyValue(value, valueValue);
			prop.serializedObject.ApplyModifiedProperties();
		}

		private static object CreateValueIfNeeded(Type type)
		{
			if(type == typeof(string))
			{
				return "";
			}
			if(typeof(UnityEngine.Object).IsAssignableFrom(type))
			{
				return null;
			}
			else
			{
				return Activator.CreateInstance(type);
			}
		}

		private static void DeleteItem(SerializedProperty prop, int index)
		{
			prop.serializedObject.Update();
			Undo.RecordObject(prop.serializedObject.targetObject, "Delete Dictionary Element");
			var k = prop.FindPropertyRelative(KEYS_FIELD_NAME);
			var v = prop.FindPropertyRelative(VALUES_FIELD_NAME);
			DeleteArrayElement(k, index);
			DeleteArrayElement(v, index);
			prop.serializedObject.ApplyModifiedProperties();
		}

		private static void DeleteArrayElement(SerializedProperty array, int index)
		{
			var elem = array.GetArrayElementAtIndex(index);
			if(elem.propertyType == SerializedPropertyType.ObjectReference) elem.objectReferenceValue = null;
			array.DeleteArrayElementAtIndex(index);
		}

		private static void ClearDictionary(SerializedProperty prop)
		{
			prop.serializedObject.Update();
			Undo.RecordObject(prop.serializedObject.targetObject, "Clear Dictionary");
			var k = prop.FindPropertyRelative(KEYS_FIELD_NAME);
			var v = prop.FindPropertyRelative(VALUES_FIELD_NAME);
			k.ClearArray();
			v.ClearArray();
			prop.serializedObject.ApplyModifiedProperties();
		}

		private static void ReplaceValue(SerializedProperty prop, int index, object newValue)
		{
			prop.serializedObject.Update();
			Undo.RecordObject(prop.serializedObject.targetObject, "Replace Dictionary Value");
			var v = prop.FindPropertyRelative(VALUES_FIELD_NAME);
			PropertyDrawerUtility.SetPropertyValue(v.GetArrayElementAtIndex(index), newValue);
			prop.serializedObject.ApplyModifiedProperties();
		}

		private static void MoveItem(SerializedProperty prop, int index, int move)
		{
			Undo.RecordObject(prop.serializedObject.targetObject, "Move Dictionary Value");
			prop.serializedObject.Update();
			prop.FindPropertyRelative(KEYS_FIELD_NAME).MoveArrayElement(index, index + move);
			prop.FindPropertyRelative(VALUES_FIELD_NAME).MoveArrayElement(index, index + move);
			prop.serializedObject.ApplyModifiedProperties();
		}

		private static string GetTypeName(Type t)
		{
			string typeName = t.Name;
			if(typeName.EndsWith("Value")) typeName = typeName.Substring(0, typeName.Length - "Value".Length);
			return typeName;
		}

		private static object GetUniqueKey(object key, IList _keys)
		{
			if(_keys.Count == 0) return key;
			if(key is string s)
			{
				return ObjectNames.GetUniqueName(((List<string>)_keys).ToArray(), s);
			}
			else if(key is int i)
			{
				var ints = (List<int>)_keys;
				while(ints.Contains(i)) i++;
				return i;
			}
			else if(key is float f)
			{
				var floats = (List<float>)_keys;
				while(floats.Contains(f)) f++;
				return f;
			}
			else
			{
				//Unable to create unique key for this type
				return key;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if(property.isExpanded && !property.hasMultipleDifferentValues)
			{
				int count = property.FindPropertyRelative(KEYS_FIELD_NAME).arraySize;
				return (count + 2) * EditorGUIUtility.singleLineHeight + (count + 1) * EditorGUIUtility.standardVerticalSpacing;
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}
	}
}