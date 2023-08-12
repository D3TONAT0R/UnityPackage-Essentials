using D3T;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	public static class EditorGUIExtras
	{
		public static Font MonospaceFont { get; private set; }
		public static Font MonospaceBoldFont { get; private set; }

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

		public static GUIStyle MonospaceBoldTextField
		{
			get
			{
				if(_monospaceBoldTextField == null)
				{
					_monospaceBoldTextField = new GUIStyle(_monospaceTextField)
					{
						font = MonospaceBoldFont
					};
				}
				return _monospaceBoldTextField;
			}
		}

		private static GUIStyle _monospaceTextField;
		private static GUIStyle _monospaceBoldTextField;

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
			string root = "Packages/com.github.d3tonat0r.core/Resources/";
			MonospaceFont = AssetDatabase.LoadAssetAtPath<Font>(root + "Consolas.ttf");
			MonospaceBoldFont = AssetDatabase.LoadAssetAtPath<Font>(root + "Consolas Bold.ttf");
		}

		public static GUIStyle GetMonospaceTextField(SerializedProperty prop)
		{
			return prop.prefabOverride ? MonospaceBoldTextField : MonospaceTextField;
		}

		public static Enum EnumButtons(Rect position, GUIContent label, Enum value, Type enumType)
		{
			int[] values = (int[])Enum.GetValues(enumType);
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
			int input = (int)(object)value;
			int output = IntButtonField(position, label, input, values, names);
			return (Enum)Enum.ToObject(enumType, output);
		}

		public static Enum EnumButtons(GUIContent label, Enum value, Type enumType)
		{
			var position = EditorGUILayout.GetControlRect(true);
			return EnumButtons(position, label, value, enumType);
		}

		public static E EnumButtons<E>(Rect position, GUIContent label, E value) where E : Enum
		{
			return (E)EnumButtons(position, label, value, typeof(E));
		}

		public static E EnumButtons<E>(GUIContent label, E value) where E : Enum
		{
			var position = EditorGUILayout.GetControlRect(true);
			return EnumButtons(position, label, value);
		}

		public static int IntButtonField(Rect position, GUIContent label, int selection, params string[] options)
		{
			return IntButtonField(position, label, selection, Enumerable.Range(0, options.Length).ToArray(), options);
		}

		public static int IntButtonField(GUIContent label, int selection, params string[] options)
		{
			var position = EditorGUILayout.GetControlRect(true);
			return IntButtonField(position, label, selection, options);
		}

		//TODO: tooltip gets incorrect positioning (shows up next to the buttons)
		private static int IntButtonField(Rect position, GUIContent label, int value, int[] values, string[] names)
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

		public static void SeparatorLine()
		{
			var position = EditorGUILayout.GetControlRect();
			SeparatorLine(position);
		}

		public static void SeparatorLine(int height)
		{
			var position = EditorGUILayout.GetControlRect(true, height);
			SeparatorLine(position);
		}
	}
}
