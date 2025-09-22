using UnityEssentials;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ButtonAttribute), true)]
	public class ButtonAttributeDrawer : ModificationPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var attr = PropertyDrawerUtility.GetAttribute<ButtonAttribute>(property, true);
			if(!attr.Below)
			{
				position.SplitVertical(EditorGUIUtility.singleLineHeight, out var top, out var bottom, EditorGUIUtility.standardVerticalSpacing);
				DrawButtons(property, attr, top);
				DrawProperty(bottom, property, label);
			}
			else
			{
				position.SplitVerticalBottom(EditorGUIUtility.singleLineHeight, out var top, out var bottom, EditorGUIUtility.standardVerticalSpacing);
				DrawProperty(top, property, label);
				DrawButtons(property, attr, bottom);
			}
		}

		private static void DrawButtons(SerializedProperty property, ButtonAttribute attribute, Rect position)
		{
			bool enabled = true;
			switch(attribute.EnabledIn)
			{
				case ButtonAttribute.Usage.Never: enabled = false; break;
				case ButtonAttribute.Usage.EditMode: enabled = !Application.isPlaying; break;
				case ButtonAttribute.Usage.PlayMode: enabled = Application.isPlaying; break;
				case ButtonAttribute.Usage.Both: enabled = true; break;
			}

			using(new EnabledScope(enabled))
			{
				var rects = position.DivideHorizontal(attribute.methodNames.Length, 4);
				for(int i = 0; i < rects.Length; i++)
				{
					string name = attribute.labels[i] ?? ObjectNames.NicifyVariableName(attribute.methodNames[i]);
					if(GUI.Button(rects[i], name))
					{
						Invoke(property, attribute.methodNames[i], attribute.arguments[i]);
					}
				}
			}
		}

		private static void Invoke(SerializedProperty property, string methodName, string[] args)
		{
			object target;
			var parentProp = PropertyDrawerUtility.GetParentProperty(property);
			if(parentProp != null) target = parentProp.GetValue();
			else target = property.serializedObject.targetObject;

			var method = target.GetType().GetMethod(methodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if(method == null)
			{
				Debug.LogError($"Could not find method to invoke: '{methodName}'");
				return;
			}
			var parameters = ParseParameters(method, args);
			method.Invoke(target, parameters);

			var onValidate = target.GetType().GetMethod("OnValidate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			onValidate?.Invoke(target, Array.Empty<object>());
		}


		private static object[] ParseParameters(MethodInfo target, string[] args)
		{
			var targetParameters = target.GetParameters();
			var parameters = new object[targetParameters.Length];
			if(args.Length > targetParameters.Length)
			{
				Debug.LogError("Too many parameters for method " + target.Name);
				return null;
			}
			for(int i = 0; i < parameters.Length; i++)
			{
				if(i >= args.Length)
				{
					if(targetParameters[i].IsOptional)
					{
						parameters[i] = targetParameters[i].DefaultValue;
						continue;
					}
					else
					{
						Debug.LogError("Not enough parameters for method " + target.Name);
						return null;
					}
				}

				var type = targetParameters[i].ParameterType;
				if(type == typeof(int))
				{
					if(int.TryParse(args[i], out int result))
					{
						parameters[i] = result;
					}
					else
					{
						Debug.LogError("Failed to parse int parameter for method " + target.Name + ": " + args[i]);
						parameters[i] = 0;
					}
				}
				else if(type == typeof(float))
				{
					if(args[i].ToLower().EndsWith("f")) args[i] = args[i].Substring(0, args[i].Length - 1);
					if(float.TryParse(args[i], out float result))
					{
						parameters[i] = result;
					}
					else
					{
						Debug.LogError("Failed to parse float parameter for method " + target.Name + ": " + args[i]);
						parameters[i] = 0f;
					}
				}
				else if(type == typeof(double))
				{
					if(args[i].ToLower().EndsWith("d")) args[i] = args[i].Substring(0, args[i].Length - 1);
					if(double.TryParse(args[i], out double result))
					{
						parameters[i] = result;
					}
					else
					{
						Debug.LogError("Failed to parse double parameter for method " + target.Name + ": " + args[i]);
						parameters[i] = 0d;
					}
				}
				else if(type == typeof(string))
				{
					parameters[i] = args[i];
				}
				else if(type == typeof(bool))
				{
					if(bool.TryParse(args[i], out bool result))
					{
						parameters[i] = result;
					}
					else
					{
						Debug.LogError("Failed to parse bool parameter for method " + target.Name + ": " + args[i]);
						parameters[i] = false;
					}
				}
				else
				{
					Debug.LogError("Unsupported parameter type for method " + target.Name + ": " + type.Name);
					parameters[i] = 0;
				}
			}
			return parameters;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + GetBaseHeight(property, label);
		}
	}
}