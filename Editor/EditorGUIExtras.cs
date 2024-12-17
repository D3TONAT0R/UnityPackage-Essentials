using UnityEngine;
﻿using UnityEssentials;
using UnityEssentialsEditor.Tools;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.EditorTools;
#if !UNITY_2020_2_OR_NEWER
using ToolManager = UnityEditor.EditorTools.EditorTools;
#endif


namespace UnityEssentialsEditor
{
	public static class EditorGUIExtras
	{
		/// <summary>
		/// A monospace font for use in editor GUI elements.
		/// </summary>
		public static Font MonospaceFont { get; private set; }
		/// <summary>
		/// A bold monospace font for use in editor GUI elements.
		/// </summary>
		public static Font MonospaceBoldFont { get; private set; }

		/// <summary>
		/// A text field style that uses a monospace font.
		/// </summary>
		public static GUIStyle MonospaceTextField
		{
			get
			{
				if(_monospaceTextField == null)
				{
					_monospaceTextField = new GUIStyle(EditorStyles.textField)
					{
						font = MonospaceFont,
						fontSize = 13,
						alignment = TextAnchor.MiddleLeft
					};
				}
				return _monospaceTextField;
			}
		}

		/// <summary>
		/// A bold label style that uses a monospace font.
		/// </summary>
		public static GUIStyle MonospaceBoldTextField
		{
			get
			{
				if(_monospaceBoldTextField == null)
				{
					_monospaceBoldTextField = new GUIStyle(MonospaceTextField)
					{
						font = MonospaceBoldFont
					};
				}
				return _monospaceBoldTextField;
			}
		}

		private static GUIStyle _monospaceTextField;
		private static GUIStyle _monospaceBoldTextField;

		/// <summary>
		/// A label style that uses a monospace font.
		/// </summary>
		public static GUIStyle MonospaceLabel
		{
			get
			{
				if(_monospaceLabel == null)
				{
					_monospaceLabel = new GUIStyle(EditorStyles.label)
					{
						font = MonospaceFont,
						fontSize = 13,
						alignment = TextAnchor.MiddleLeft
					};
				}
				return _monospaceLabel;
			}
		}
		private static GUIStyle _monospaceLabel;

		[InitializeOnLoadMethod]
		private static void Init()
		{
			string root = "Packages/com.github.d3tonat0r.essentials/Editor/EditorAssets/";
			MonospaceFont = AssetDatabase.LoadAssetAtPath<Font>(root + "Consolas.ttf");
			MonospaceBoldFont = AssetDatabase.LoadAssetAtPath<Font>(root + "Consolas Bold.ttf");
		}

		/// <summary>
		/// Returns the proper monospace text field style for the current state of the given property.
		/// </summary>
		public static GUIStyle GetMonospaceTextField(SerializedProperty prop)
		{
			return prop.prefabOverride ? MonospaceBoldTextField : MonospaceTextField;
		}

		/// <summary>
		/// Draws an enum selection as horizontal buttons.
		/// </summary>
		public static Enum EnumButtons(Rect position, GUIContent label, Enum value, Type enumType)
		{
			//TODO: could use some caching
			var valuesArray = Enum.GetValues(enumType);
			int[] values = new int[valuesArray.Length];
			for(int i = 0; i < valuesArray.Length; i++)
			{
				values[i] = Convert.ToInt32(valuesArray.GetValue(i));
			}
			string[] names = Enum.GetNames(enumType);
			for(int i = 0; i < values.Length; i++)
			{
				var member = enumType.GetMember(names[i]).FirstOrDefault(m => m.DeclaringType == enumType);
				var attr = member.GetCustomAttribute<InspectorNameAttribute>();
				if(attr != null)
				{
					names[i] = attr.displayName;
				}
				else
				{
					names[i] = ObjectNames.NicifyVariableName(Enum.GetName(enumType, values[i]));
				}
			}
			int input = value != null ? Convert.ToInt32(value) : -1;
			int output = HorizontalButtonGroup(position, label, input, values, names);
			if(output >= 0)
			{
				return (Enum)Enum.ToObject(enumType, output);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Draws an enum selection as horizontal buttons.
		/// </summary>
		public static Enum EnumButtons(GUIContent label, Enum value, Type enumType)
		{
			var position = EditorGUILayout.GetControlRect(true);
			return EnumButtons(position, label, value, enumType);
		}

		/// <summary>
		/// Draws an enum selection as horizontal buttons.
		/// </summary>
		public static E EnumButtons<E>(Rect position, GUIContent label, E value) where E : Enum
		{
			return (E)EnumButtons(position, label, value, typeof(E));
		}

		/// <summary>
		/// Draws an enum selection as horizontal buttons.
		/// </summary>
		public static E EnumButtons<E>(GUIContent label, E value) where E : Enum
		{
			var position = EditorGUILayout.GetControlRect(true);
			return EnumButtons(position, label, value);
		}

		/// <summary>
		/// Draws a horizontal button group.
		/// </summary>
		public static int HorizontalButtonGroup(Rect position, GUIContent label, int selection, params string[] options)
		{
			return HorizontalButtonGroup(position, label, selection, Enumerable.Range(0, options.Length).ToArray(), options);
		}

		/// <summary>
		/// Draws a horizontal button group.
		/// </summary>
		public static int HorizontalButtonGroup(GUIContent label, int selection, params string[] options)
		{
			var position = EditorGUILayout.GetControlRect(true);
			return HorizontalButtonGroup(position, label, selection, options);
		}

		/// <summary>
		/// Draws a horizontal button group.
		/// </summary>
		//TODO: tooltip gets incorrect positioning (shows up next to the buttons)
		private static int HorizontalButtonGroup(Rect position, GUIContent label, int value, int[] values, string[] names)
		{
			if(label != null)
			{
				EditorGUI.LabelField(position, label);
				position.xMin += EditorGUIUtility.labelWidth;
				label.tooltip = null;
			}
			if(names == null)
			{
				names = new string[values.Length];
				for(int i = 0; i < values.Length; i++)
				{
					names[i] = values[i].ToString();
				}
			}
			if(values.Length != names.Length)
			{
				throw new ArgumentException("Value and Name array lengths do not match.");
			}
			var rects = position.DivideHorizontal(values.Length);
			for(int i = 0; i < values.Length; i++)
			{
				GUIStyle style = i == 0 ? EditorStyles.miniButtonLeft : i == values.Length - 1 ? EditorStyles.miniButtonRight : EditorStyles.miniButtonMid;
				bool b = values[i] == value;
				var b2 = GUI.Toggle(rects[i], b, names[i], style);
				if(b2 && !b)
				{
					value = values[i];
				}
			}
			return value;
		}

		/// <summary>
		/// Draws a horizontal separator line.
		/// </summary>
		public static void SeparatorLine(Rect position)
		{
			position.xMin += EditorGUI.indentLevel * 15;
			position.yMin += Mathf.CeilToInt(position.height / 2f);
			position.height = 1;
			position.xMin += 2;
			position.xMax -= 2;
			Color32 color = EditorGUIUtility.isProSkin ? new Color32(196, 196, 196, 128) : new Color32(22, 22, 22, 128);
			EditorGUI.DrawRect(position, color);
		}

		/// <summary>
		/// Draws a horizontal separator line.
		/// </summary>
		public static void SeparatorLine()
		{
			var position = EditorGUILayout.GetControlRect();
			SeparatorLine(position);
		}

		/// <summary>
		/// Draws a horizontal separator line.
		/// </summary>
		public static void SeparatorLine(int height)
		{
			var position = EditorGUILayout.GetControlRect(true, height);
			SeparatorLine(position);
		}

		/// <summary>
		/// Draws a red label indicating an error.
		/// </summary>
		public static void ErrorLabelField(Rect position, GUIContent label, GUIContent label2)
		{
			using(new ColorScope(new Color(1, 0.3f, 0.3f)))
			{
				EditorGUI.LabelField(position, label, label2);
			}
		}

		/// <summary>
		/// Draws a tool button.
		/// </summary>
		public static bool ToolButton(Rect rect, string label, GUIContent icon, bool state)
		{
			var buttonRect = new Rect(rect.x, rect.y, 30, 20);
			if(label != null)
			{
				GUI.Label(rect, label);
				buttonRect.x += EditorGUIUtility.labelWidth;
			}
			return GUI.Toggle(buttonRect, state, icon, GUI.skin.button);
		}

		/// <summary>
		/// Draws a tool button.
		/// </summary>
		public static bool ToolButton(string label, GUIContent icon, bool state)
		{
			var position = EditorGUILayout.GetControlRect(true, 20);
			var buttonRect = new Rect(0, 0, 30, 20);
			if(label != null)
			{
				GUI.Label(position, label);
				buttonRect.x += EditorGUIUtility.labelWidth;
			}
			return GUI.Toggle(buttonRect, state, BoundsDefinitionTool.Icon, GUI.skin.button);
		}

		/// <summary>
		/// Draws a tool button that activates the given editor tool when pressed.
		/// </summary>
		public static void ToolButton<T>(string label, GUIContent icon) where T : EditorTool
		{
			bool state = ToolManager.activeToolType == typeof(T);
			bool newState = ToolButton(label, icon, state);
			if(newState != state)
			{
				if(newState) ToolManager.SetActiveTool<T>();
				else ToolManager.RestorePreviousTool();
			}
		}
	}
}
