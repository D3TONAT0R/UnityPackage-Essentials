using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEssentials;
using UnityEssentials.Collections;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(PolymorphicStack<>), true)]
	public class PolymorphicStackDrawer : PropertyDrawer
	{
		private static bool initialized = false;

		private Dictionary<Type, string> supportedTypes;
		private SerializedObject serializedObject;

		static Dictionary<Type, Dictionary<Type, string>> subtypes = new Dictionary<Type, Dictionary<Type, string>>();

		static GUIStyle headerStyle;

		static string jsonClipboard;
		static Type jsonClipboardType;

		private int lastArraySize;

		private float ItemHeaderHeight => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		private static StackElement renamingElement;
		private static bool needsRenameFocus;
		private static string renameInput;

		private static Dictionary<Type, Type> drawerTypes = new Dictionary<Type, Type>();
		private List<StackElementDrawer> drawerInstances = new List<StackElementDrawer>();

		private static void Init()
		{
			foreach(var t in ReflectionUtility.GetClassesOfType(typeof(StackElementDrawer)))
			{
				if(t == typeof(StackElementDrawer)) continue;
				var attr = t.GetCustomAttribute<CustomElementDrawerAttribute>();
				if(attr != null)
				{
					if(!typeof(StackElement).IsAssignableFrom(attr.targetType))
					{
						Debug.LogError($"Type does not inherit from 'StackElement' ({attr.targetType}).");
					}
					drawerTypes[attr.targetType] = t;
				}
				else
				{
					Debug.LogError($"Custom element drawer '{t.FullName}' does not have a 'CustomElementDrawer' attribute, the drawer will not be used.");
				}
			}
			initialized = true;
		}

		private void GetSupportedTypes(Type elementBaseType)
		{
			if(!subtypes.TryGetValue(elementBaseType, out supportedTypes))
			{
				//Find all valid subtypes for this type
				var types = new List<Type>(ReflectionUtility.GetGameAssembliesIncludingUnity().SelectMany(a => a.GetTypes().Where(t => elementBaseType.IsAssignableFrom(t) && !t.IsAbstract)));
				supportedTypes = new Dictionary<Type, string>();
				foreach(var t in types)
				{
					string menuName = "";
					var attr = t.GetCustomAttribute<AddElementMenuAttribute>(true);
					string customName = ObjectNames.NicifyVariableName(t.Name);
					if(attr != null)
					{
						menuName = attr.group;
						if(!string.IsNullOrWhiteSpace(menuName)) menuName += "/";
						if(!string.IsNullOrWhiteSpace(attr.customName))
						{
							customName = attr.customName;
						}
					}
					menuName += customName;
					supportedTypes.Add(t, menuName);
				}
				subtypes.Add(elementBaseType, supportedTypes);
			}
		}

		public override void OnGUI(Rect position, SerializedProperty stackProp, GUIContent label)
		{
			if(!initialized)
			{
				Init();
			}
			position.height -= EditorGUIUtility.singleLineHeight * 0.5f;
			serializedObject = stackProp.serializedObject;

			var hostProp = stackProp.FindPropertyRelative("hostComponent");
			hostProp.objectReferenceValue = stackProp.serializedObject.targetObject as MonoBehaviour;

			var arrayProp = stackProp.FindPropertyRelative("stack");
			if(supportedTypes == null)
			{
				var rootObj = stackProp.GetValue();
				var stackType = rootObj.GetType().GetField("stack", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(rootObj).GetType();
				var stackElemType = stackType.GenericTypeArguments[0];
				GetSupportedTypes(stackElemType);
			}

			if(headerStyle == null) headerStyle = new GUIStyle(EditorStyles.toolbar) { fixedHeight = 0 };
			GUIStyle boxStyle = "FrameBox";
			GUIStyle topBoxStyle = "Tab onlyOne";

			boxStyle.DrawOnRepaint(position);
			position.height = EditorGUIUtility.singleLineHeight * 1.5f;

			topBoxStyle.DrawOnRepaint(position);
			position.SplitHorizontal(EditorGUIUtility.labelWidth, out _, out var counterRect);

			position.SplitHorizontal(5, out _, out var headerLabelRect);
			position.SplitHorizontalRight(16, out _, out var contextRect);
			contextRect.y += (contextRect.height - 16) / 2;
			contextRect.height = 16;

			GUI.Label(counterRect, $"[{arrayProp.arraySize}]");
			stackProp.isExpanded = EditorGUI.Foldout(position, stackProp.isExpanded, GUIContent.none);
			GUI.Label(headerLabelRect, label, EditorStyles.boldLabel);
			if(GUI.Button(contextRect, "", EditorStyles.foldoutHeaderIcon))
			{
				var menu = new GenericMenu();
				var stackPath = arrayProp.propertyPath;
				menu.AddItem(new GUIContent("Expand All"), false, () => ToggleExpandedStateAll(arrayProp.serializedObject, stackPath, true));
				menu.AddItem(new GUIContent("Collapse All"), false, () => ToggleExpandedStateAll(arrayProp.serializedObject, stackPath, false));
				/*
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Copy Stack"), false, () => CopyObject(obj));
				var paste = new GUIContent("Paste Stack Contents");
				if (jsonClipboard != null && jsonClipboardType == obj.GetType())
				{
					menu.AddItem(paste, false, () => PasteStackContents(obj, stack.serializedObject, stackPath));
				}
				else
				{
					menu.AddDisabledItem(paste, false);
				}
				*/
				menu.ShowAsContext();
			}
			if(stackProp.isExpanded)
			{
				position.xMax -= 1;
				position.NextProperty();
				EditorGUI.indentLevel++;

				DrawExtraProperties(ref position, stackProp.Copy());

				for(int i = 0; i < arrayProp.arraySize; i++)
				{
					var y = DrawItem(position, arrayProp, stackProp, i);
					position.y = y + EditorGUIUtility.standardVerticalSpacing;
				}
				EditorGUI.indentLevel--;
				//position.NextProperty();
				DrawFooter(position, arrayProp, boxStyle);
			}
			if(arrayProp.arraySize > lastArraySize)
			{
				FixDuplicateReferences(arrayProp);
			}
			lastArraySize = arrayProp.arraySize;
		}

		private void DrawExtraProperties(ref Rect position, SerializedProperty property)
		{
			bool hasExtras = false;
			position.xMax -= 4;
			foreach(var e in GetExtraProperties(property))
			{
				hasExtras = true;
				position.height = EditorGUI.GetPropertyHeight(e);
				EditorGUI.PropertyField(position, e);
				position.NextProperty();
			}
			position.xMax += 4;
			if(hasExtras) position.y += 4;
			/*
			if(hasExtras)
			{
				position.y += 4;
				var sep = position;
				sep.height = 1;
				sep = sep.Inset(16, 16, 0, 0);
				GUI.Box(sep, "", "CN Box");
				position.y += 8;
			}
			*/
		}

		private void FixDuplicateReferences(SerializedProperty stack)
		{
			var elements = new List<StackElement>();
			for(int i = 0; i < stack.arraySize; i++)
			{
				var elem = (StackElement)stack.GetArrayElementAtIndex(i).GetValue();
				if(elem != null)
				{
					if(!elements.Contains(elem))
					{
						elements.Add(elem);
					}
					else
					{
						//Duplicate element detected, make a copy of it
						var inst = Activator.CreateInstance(elem.GetType());
						JsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(elem), inst);
						stack.GetArrayElementAtIndex(i).managedReferenceValue = inst;
					}
				}
			}
		}

		private float DrawItem(Rect position, SerializedProperty arrayProp, SerializedProperty stackProp, int i)
		{
			var elem = arrayProp.GetArrayElementAtIndex(i);
			var obj = elem.GetValue() as StackElement;
			if(obj != null) elem.FindPropertyRelative("hostComponent").objectReferenceValue = stackProp.serializedObject.targetObject as MonoBehaviour;
			var drawer = GetDrawerFor(obj?.GetType() ?? typeof(StackElementDrawer));

			position.height = ItemHeaderHeight;

			drawer.OnHeaderGUI(position, i, elem, obj, stackProp, arrayProp);

			position.xMin += 2;
			position.xMax -= 2;
			if(elem.isExpanded && obj != null)
			{
				drawer.OnGUI(ref position, elem, obj, stackProp);
			}
			return position.yMax;
		}

		internal static void DrawItemHeader(Rect position, int i, SerializedProperty prop, StackElement obj, SerializedProperty array)
		{
			EditorGUI.BeginProperty(position, new GUIContent(prop.displayName), prop);
			var title = new GUIContent(obj != null ? obj.HeaderTitle : "null");

			var headerRect = position;
			headerRect.xMin++;
			position.SplitHorizontal(20, out _, out var pos2);
			pos2.SplitHorizontal(20, out var enabledRect, out var labelRect);
			labelRect.width -= 20;

			headerStyle.DrawOnRepaint(headerRect.Outset(0, 0, EditorGUIUtility.standardVerticalSpacing, 0));

			var ep = GetEnabledPropery(prop);
			if(ep != null) ep.boolValue = GUI.Toggle(enabledRect, ep.boolValue, "");

			if(obj != null && obj == renamingElement)
			{
				DrawTitleRenameField(prop, labelRect);
			}
			else
			{
				GUI.Label(labelRect, title, EditorStyles.boldLabel);
			}

			if(obj != null) prop.isExpanded = EditorGUI.Foldout(position, prop.isExpanded, "");

			headerRect.SplitHorizontal(EditorGUIUtility.labelWidth, out _, out var typeLabelRect);
			//GUI.Label(typeLabelRect, typeName, EditorStyles.miniLabel);
			headerRect.SplitHorizontalRight(16, out _, out var contextButtonRect);
			contextButtonRect.height = 16;

			if(GUI.Button(contextButtonRect, GUIContent.none, EditorStyles.foldoutHeaderIcon))
			{
				var menu = new GenericMenu();
				if(obj != null)
				{
					menu.AddItem(new GUIContent("Copy Element"), false, () => CopyObject(obj));
					bool canPaste = !string.IsNullOrEmpty(jsonClipboard) && obj.GetType() == jsonClipboardType;
					var paste = new GUIContent("Paste Element Values");
					if(canPaste)
					{
						menu.AddItem(paste, false, () => EditorJsonUtility.FromJsonOverwrite(jsonClipboard, obj));
					}
					else
					{
						menu.AddDisabledItem(paste, false);
					}
				}
				menu.AddItem(new GUIContent("Remove Element"), false, () =>
				{
					Undo.RecordObject(array.serializedObject.targetObject, "Remove " + title);
					RemoveItem((IList)array.GetValue(), i);
				});

				var up = new GUIContent("Move Up");
				if(i > 0)
				{
					menu.AddItem(up, false, () =>
					{
						Undo.RecordObject(array.serializedObject.targetObject, "Move Element Up");
						SwapItems((IList)array.GetValue(), i, i - 1);
					});
				}
				else
				{
					menu.AddDisabledItem(up);
				}
				var down = new GUIContent("Move Down");
				if(i < array.arraySize - 1)
				{
					menu.AddItem(down, false, () =>
					{
						Undo.RecordObject(array.serializedObject.targetObject, "Move Element Down");
						SwapItems((IList)array.GetValue(), i, i + 1);
					});
				}
				else
				{
					menu.AddDisabledItem(down);
				}

				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Set Custom Name"), false, () =>
				{
					renamingElement = obj;
					needsRenameFocus = true;
					renameInput = obj.CustomName;
				});
				var clear = new GUIContent("Clear Custom Name");
				if(obj.CustomName != null && obj.CustomName.Length > 0)
				{
					menu.AddItem(clear, false, () =>
					{
						Undo.RecordObject(prop.serializedObject.targetObject, "Clar Custom Name");
						obj.CustomName = null;
						prop.serializedObject.Update();
					});
				}
				else
				{
					menu.AddDisabledItem(clear);
				}

				menu.ShowAsContext();
			}
			EditorGUI.EndProperty();
		}

		private static void DrawTitleRenameField(SerializedProperty prop, Rect labelRect)
		{
			bool exit = false;
			if(Event.current.type == EventType.KeyDown && !needsRenameFocus)
			{
				if(Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Escape)
				{
					exit = true;
				}
			}
			GUI.SetNextControlName("customnamefield");
			renameInput = GUI.TextField(labelRect, renameInput);

			if(needsRenameFocus)
			{
				GUI.FocusControl("customnamefield");
				needsRenameFocus = false;
			}

			if(!needsRenameFocus)
			{
				if(GUI.GetNameOfFocusedControl() != "customnamefield")
				{
					exit = true;
				}
			}
			if(exit)
			{
				Undo.RecordObject(prop.serializedObject.targetObject, "Set Custom Name");
				renamingElement.CustomName = renameInput;
				renamingElement = null;
				prop.serializedObject.Update();
			}
		}

		private static SerializedProperty GetEnabledPropery(SerializedProperty prop)
		{
			if(prop == null) return null;
			SerializedProperty e = prop.FindPropertyRelative("enabled");
			if(e == null) e = prop.FindPropertyRelative("m_Enabled");
			return e;
		}

		private void DrawFooter(Rect position, SerializedProperty stack, GUIStyle style)
		{
			var boxRect = position.Outset(0, 1, EditorGUIUtility.standardVerticalSpacing + 1, EditorGUIUtility.standardVerticalSpacing);
			style.DrawOnRepaint(boxRect);
			position.width -= 10;
			position.SplitHorizontalRight(20, out _, out var plusRect);
			if(GUI.Button(plusRect, EditorGUIUtility.IconContent("d_Toolbar Plus More"), "IconButton"))
			{
				var menu = new GenericMenu();
				foreach(var t in supportedTypes)
				{
					menu.AddItem(new GUIContent(t.Value), false, () =>
					{
						Undo.RecordObject(serializedObject.targetObject, "Add " + t.Key.Name);
						AddNewItem(stack, t.Key);
					});
				}
				menu.AddSeparator("");
				var paste = new GUIContent("Paste Element As New");
				if(!string.IsNullOrEmpty(jsonClipboard) && supportedTypes.ContainsKey(jsonClipboardType))
				{
					menu.AddItem(paste, false, () => PasteElementAsNew(stack));
				}
				else
				{
					menu.AddDisabledItem(paste, false);
				}
				menu.ShowAsContext();
			}
		}

		private static object AddNewItem(SerializedProperty stack, Type t)
		{
			var list = (IList)stack.GetValue();
			var inst = Activator.CreateInstance(t);
			list.Add(inst);
			return inst;
		}

		private static void SwapItems(IList list, int iA, int iB)
		{
			var temp = list[iA];
			list[iA] = list[iB];
			list[iB] = temp;
		}

		private static void RemoveItem(IList list, int i)
		{
			list.RemoveAt(i);
		}

		private static void CopyObject(object obj)
		{
			jsonClipboard = EditorJsonUtility.ToJson(obj);
			jsonClipboardType = obj.GetType();
		}

		private static void PasteElementAsNew(SerializedProperty stack)
		{
			var newItem = AddNewItem(stack, jsonClipboardType);
			EditorJsonUtility.FromJsonOverwrite(jsonClipboard, newItem);
		}

		private static void PasteStackContents(object stack, SerializedObject target, string targetPath)
		{
			Debug.Log(jsonClipboard);
			EditorJsonUtility.FromJsonOverwrite(jsonClipboard, stack);
			target.FindProperty(targetPath).managedReferenceValue = stack;
		}

		private static void ToggleExpandedStateAll(SerializedObject obj, string path, bool expand)
		{
			var stack = obj.FindProperty(path);
			for(int i = 0; i < stack.arraySize; i++)
			{
				stack.GetArrayElementAtIndex(i).isExpanded = expand;
			}
		}

		private StackElementDrawer GetDrawerFor(Type elementType)
		{
			while(elementType != null)
			{
				if(elementType == typeof(StackElement))
				{
					return GetOrCreateDrawer(typeof(StackElementDrawer));
				}
				if(drawerTypes.TryGetValue(elementType, out var drawerType))
				{
					return GetOrCreateDrawer(drawerType);
				}
				else
				{
					elementType = elementType.BaseType;
				}
			}
			throw new InvalidOperationException();
		}

		private StackElementDrawer GetOrCreateDrawer(Type drawerType)
		{
			var inst = drawerInstances.FirstOrDefault((d) => d.GetType() == drawerType);
			if(inst == null)
			{
				inst = (StackElementDrawer)Activator.CreateInstance(drawerType);
				drawerInstances.Add(inst);
			}
			return inst;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var list = property.FindPropertyRelative("stack");
			float h = EditorGUIUtility.singleLineHeight * 1.5f;
			if(property.isExpanded)
			{
				bool hasExtras = false;
				foreach(var e in GetExtraProperties(property))
				{
					hasExtras = true;
					h += EditorGUI.GetPropertyHeight(e) + EditorGUIUtility.standardVerticalSpacing;
				}
				//if(hasExtras) h += 12;
				if(hasExtras) h += 4;
				for(int i = 0; i < list.arraySize; i++)
				{
					var elem = list.GetArrayElementAtIndex(i);
					if(elem.isExpanded)
					{
						h += EditorGUI.GetPropertyHeight(elem, true) + ItemHeaderHeight - EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
					}
					else
					{
						h += ItemHeaderHeight;
					}
					h += EditorGUIUtility.standardVerticalSpacing;
				}
				h += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.standardVerticalSpacing;
			}
			h += EditorGUIUtility.singleLineHeight * 0.5f;
			return h;
		}

		private static IEnumerable<SerializedProperty> GetExtraProperties(SerializedProperty parent)
		{
			int dots = parent.propertyPath.Count(c => c == '.');
			foreach(SerializedProperty inner in parent)
			{
				bool isDirectChild = inner.propertyPath.Count(c => c == '.') == dots + 1;
				if(isDirectChild && inner.name != "stack") yield return inner;
			}
		}
	}
}