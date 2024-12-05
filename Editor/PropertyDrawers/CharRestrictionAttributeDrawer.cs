using UnityEssentials;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(CharRestrictionAttribute))]
	public class CharRestrictionAttributeDrawer : PropertyDrawer
	{
		private static Texture infoIcon;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent content)
		{

			var attr = fieldInfo.GetCustomAttribute<CharRestrictionAttribute>(true);
			DrawRestrictedTextField(position, property, content, true, attr.allowedChars, attr.forcedCase, attr.replacementChar);
		}

		public static void DrawRestrictedTextField(Rect position, SerializedProperty property, GUIContent content, bool showInfoIcon, string allowedChars, bool? forcedCase, char replacementChar)
		{
			try
			{
				if(!PropertyDrawerUtility.ValidatePropertyTypeForAttribute(position, property, content, SerializedPropertyType.String)) return;
				bool restricted = allowedChars != null;
				if(infoIcon == null)
				{
					infoIcon = EditorGUIUtility.IconContent("d_Text Icon").image;
				}
				EditorGUI.BeginProperty(position, content, property);
				Rect infoRect = Rect.zero;
				if(restricted && showInfoIcon)
				{
					position.SplitHorizontalRight(16, out position, out infoRect, 2);
				}
				var value = property.stringValue;
				EditorGUI.BeginChangeCheck();
				EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
				value = EditorGUI.TextField(position, content, value, EditorGUIExtras.GetMonospaceTextField(property));
				if(restricted)
				{
					GUI.color = GUI.color.MultiplyAlpha(0.25f);
					GUIContent info = new GUIContent();
					info.tooltip = "The following characters are allowed:\n" + allowedChars;
					if(forcedCase.HasValue)
					{
						info.tooltip += "\n" + (forcedCase.Value ? "Uppercase" : "Lowercase") + " only.";
					}
					GUI.DrawTexture(infoRect, infoIcon, ScaleMode.ScaleToFit);
					GUI.Label(infoRect, info);
					GUI.color = GUI.color.MultiplyAlpha(4.0f);
					value = ApplyRestrictions(value, allowedChars, forcedCase, replacementChar);
				}
				if(EditorGUI.EndChangeCheck())
				{
					property.stringValue = value;
				}
				EditorGUI.showMixedValue = false;
				EditorGUI.EndProperty();
			}
			catch(Exception e)
			{
				e.LogException("Failed to draw property");
			}
		}

		private static string ApplyRestrictions(string value, string allowedChars, bool? forcedCase, char replacementChar)
		{
			if(forcedCase != null)
			{
				if(forcedCase.Value)
				{
					value = value.ToUpper();
				}
				else
				{
					value = value.ToLower();
				}
			}
			if(allowedChars != null)
			{
				var arr = value.ToCharArray();
				for(int i = 0; i < arr.Length; i++)
				{
					if(allowedChars.IndexOf(arr[i]) < 0)
					{
						arr[i] = replacementChar;
					}
				}
				value = new string(arr);
			}
			return value;
		}
	}
}