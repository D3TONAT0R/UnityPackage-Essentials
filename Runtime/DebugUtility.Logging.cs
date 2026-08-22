using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEssentials.PlayerLoop;

namespace UnityEssentials
{
	/// <summary>
	/// Utility functions for debugging.
	/// </summary>
	public static partial class DebugUtility
	{
		private static StringBuilder stringBuilder = new StringBuilder();

		/// <summary>
		/// Logs an array's content to the console.
		/// </summary>
		public static void LogArray<T>(string message, IList<T> array, Func<T, string> elementFunc = null)
		{
			elementFunc ??= t => t.ToString();
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
			stringBuilder.Clear();
			message ??= t.name;
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
			if(obj == null) 
			{
				Debug.Log($"{message}: (null)");
				return;
			}
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
						if(obsoleteAttr != null) continue;
						stringBuilder.AppendLine($"- {property.Name} ({property.PropertyType.Name}): {property.GetValue(obj)?.ToString() ?? "(null)"}");
					}
				}
			}
			Debug.Log(stringBuilder.ToString());
		}
	}
}