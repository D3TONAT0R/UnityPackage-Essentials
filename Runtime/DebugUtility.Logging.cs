using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEssentials
{
	/// <summary>
	/// Utility functions for debugging.
	/// </summary>
	public static partial class DebugUtility
	{
		[ThreadStatic]
		private static StringBuilder stringBuilder;

		/// <summary>
		/// Logs an array's content to the console.
		/// </summary>
#if UNITY_2021_3_OR_NEWER
		[HideInCallstack]
#endif
		public static void LogArray<T>(string message, IList<T> array, Func<T, string> elementFunc = null)
		{
			elementFunc = elementFunc ?? (t => t.ToString());
			stringBuilder = stringBuilder ?? new StringBuilder();
			stringBuilder.Clear();
			if (!string.IsNullOrEmpty(message)) stringBuilder.Append(message + " ");
			if (array != null)
			{
				stringBuilder.AppendLine($"{typeof(T)}[{array.Count}]");
				int i = 0;
				foreach (var elem in array)
				{
					stringBuilder.AppendLine($"{i}: {(elem != null ? elementFunc(elem) : "(null)")}");
					i++;
				}
			}
			else
			{
				stringBuilder.AppendLine("(null)");
			}
			Debug.Log(stringBuilder.ToString());
		}

		/// <summary>
		/// Logs a transform's position, rotation and scale to the console
		/// </summary>
		public static void LogTransform(string message, Transform t, bool oneLine = false, bool position = true, bool rotation = true,
			bool scale = true)
		{
			stringBuilder = stringBuilder ?? new StringBuilder();
			stringBuilder.Clear();
			message = message ?? t.name;
			if (t)
			{
				if (!oneLine)
				{
					stringBuilder.AppendLine(message + ":");
					if (position) stringBuilder.AppendLine("- Position: " + t.position);
					if (rotation) stringBuilder.AppendLine("- Rotation: " + t.eulerAngles);
					if (scale) stringBuilder.AppendLine("- Scale (Local): " + t.localScale);
				}
				else
				{
					stringBuilder.Append(message + ":");
					if (position) stringBuilder.Append(" Pos: " + t.position);
					if (rotation) stringBuilder.Append(" Rot: " + t.eulerAngles);
					if (scale) stringBuilder.Append(" Scale (Local): " + t.localScale);
				}
			}
			else
			{
				stringBuilder.Append(" (null)");
			}
			Debug.Log(stringBuilder.ToString());
		}

		/// <summary>
		/// Logs an object's data to the console.
		/// </summary>
		public static void LogObject(string message, object obj, bool fields = true, bool properties = true, bool includePrivate = true)
		{
			if (obj == null)
			{
				Debug.Log($"{message}: (null)");
				return;
			}
			stringBuilder = stringBuilder ?? new StringBuilder();
			stringBuilder.Clear();
			var type = obj.GetType();
			var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
			if (includePrivate) bindingFlags |= BindingFlags.NonPublic;
			stringBuilder.AppendLine($"{message}: ({type.Name})");
			if (fields)
			{
				foreach (var field in type.GetFields(bindingFlags))
				{
					stringBuilder.AppendLine($"- {field.Name} ({field.FieldType.Name}): {field.GetValue(obj)?.ToString() ?? "(null)"}");
				}
			}
			if (properties)
			{
				foreach (var property in type.GetProperties(bindingFlags))
				{
					if (property.CanRead)
					{
						var obsoleteAttr = property.GetCustomAttribute<ObsoleteAttribute>();
						if (obsoleteAttr != null) continue;
						stringBuilder.AppendLine(
							$"- {property.Name} ({property.PropertyType.Name}): {property.GetValue(obj)?.ToString() ?? "(null)"}");
					}
				}
			}
			Debug.Log(stringBuilder.ToString());
		}

		/// <summary>
		/// Logs a verbose message to the console, with a boolean flag to enable or disable the message from being logged.
		/// </summary>
		public static void Verbose(string message, bool enabled, Object context = null)
		{
			if(!enabled) return;
			Debug.Log($"<alpha=#80>{message}<alpha=#FF>", context);
		}

		/// <summary>
		/// Logs a verbose message to the console.
		/// </summary>
		public static void Verbose(string message, Object context = null) => Verbose(message, true, context);
		
		/// <summary>
		/// Logs a critical error message to the console, in red color.
		/// </summary>
		public static void Critical(string message, Object context = null, bool displayDialog = true)
		{
			Debug.LogError($"<b><color=#FF4040>{message}</color></b>", context);
			if (displayDialog)
			{
#if UNITY_EDITOR
				if (UnityEditor.EditorApplication.isPlaying)
				{
					if (DisplayEditorChoiceDialog("Critical Error Message", message, "Exit Play Mode", "Continue"))
					{
						UnityEditor.EditorApplication.isPlaying = false;
					}
				}
				else
				{
					DisplayEditorDialog("Critical Error Message", message, "OK");
				}
#endif
			}
		}

		public static void DisplayEditorDialog(string title, string message, string okButton = "OK")
		{
#if UNITY_EDITOR
			UnityEditor.EditorUtility.DisplayDialog(title, message, okButton);
#endif
		}

		public static bool DisplayEditorChoiceDialog(string title, string message, string okButton = "OK", string cancelButton = "Cancel")
		{
#if UNITY_EDITOR
			return UnityEditor.EditorUtility.DisplayDialog(title, message, okButton, cancelButton);
#endif
		}
	}
}