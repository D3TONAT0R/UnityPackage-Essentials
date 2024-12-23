using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{

	/// <summary>
	/// Draws a clickable button above or below a field in the inspector.
	/// </summary>
	//TODO: Crashes the editor when combined with ShowIf or HideIf
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ButtonAttribute : PropertyAttribute
	{
		/// <summary>
		/// Determines in which context the button is enabled.
		/// </summary>
		[Flags]
		public enum Usage
		{
			Never = 0,
			EditMode = 1,
			PlayMode = 2,
			Both = 3
		}

		public readonly string[] labels;
		public readonly string[] methodNames;
		public readonly string[][] arguments;

		public bool Below { get; set; }

		public virtual Usage EnabledIn => Usage.Both;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="buttons">The button(s) that should be drawn in the inspector, in any of the following formats:
		/// <c>"TargetMethodName"</c>,
		/// <c>"TargetMethodName(arguments)"</c>,
		/// <c>"TargetMethodName:Button Label"</c>
		/// or <c>"TargetMethodName(arguments):Button Label"</c>.
		/// If no button label is specified (with a ':'), the method name is used instead. 
		/// Parameter arguments can be separated with commas and must not have spaces in between.
		/// Note that method overloads are currently unsupported.</param>
		public ButtonAttribute(params string[] buttons)
		{
			order = -10;
			labels = new string[buttons.Length];
			methodNames = new string[buttons.Length];
			arguments = new string[buttons.Length][];
			for(int i = 0; i < buttons.Length; i++)
			{
				string[] split = buttons[i].Split(new char[] { ':' }, 2);
				var call = split[0];
				if(call.Contains("("))
				{
					if(!call.Contains(")"))
					{
						Debug.LogError("Malformed method call detected: " + buttons[i]);
					}
					else
					{
						//Get content between parentheses using regex
						var match = Regex.Match(call, @"\(([^)]*)\)");
						arguments[i] = match.Value.Substring(1, match.Value.Length - 2).Split(',');
						var methodName = call.Substring(0, call.IndexOf('('));
						methodNames[i] = methodName;
					}
					var splitCall = call.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
					methodNames[i] = splitCall[0];
				}
				else
				{
					methodNames[i] = split[0];
					arguments[i] = Array.Empty<string>();
				}
				if(split.Length > 1)
				{
					labels[i] = split[1];
				}
				else
				{
					labels[i] = ObjectNames.NicifyVariableName(split[0]);
				}
			}
		}
	}

	/// <summary>
	/// Draws a clickable button above or below a field in the inspector that is only available when not in play mode.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class EditorButtonAttribute : ButtonAttribute
	{
		public override Usage EnabledIn => Usage.EditMode;

		public EditorButtonAttribute(params string[] buttons) : base(buttons) { }
	}

	/// <summary>
	/// Draws a clickable button above or below a field in the inspector that is only available when in play mode.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class RuntimeButtonAttribute : ButtonAttribute
	{
		public override Usage EnabledIn => Usage.PlayMode;

		public RuntimeButtonAttribute(params string[] buttons) : base(buttons) { }
	}
}