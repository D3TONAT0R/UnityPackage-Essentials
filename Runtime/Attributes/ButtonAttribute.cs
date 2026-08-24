using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Draws a clickable button above or below a field in the inspector.
	/// </summary>
	//TODO: Incompatible with ShowIf / HideIf / EnabledIf / DisabledIf
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ButtonAttribute : PropertyAttribute
	{
		public class ButtonInfo
		{
			public GUIContent label;
			public string methodName;
			public string[] arguments;
		}
		
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

		public readonly ButtonInfo[] buttons;

		public bool Below { get; set; }

		public virtual Usage EnabledIn => Usage.Both;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="buttonParams">The button(s) that should be drawn in the inspector, in any of the following formats:
		/// <c>"TargetMethodName"</c>,
		/// <c>"TargetMethodName(arguments)"</c>,
		/// <c>"TargetMethodName:Button Label"</c>
		/// or <c>"TargetMethodName(arguments):Button Label"</c>.
		/// If no button label is specified (with a ':'), the method name is used instead. 
		/// Parameter arguments can be separated with commas and must not have spaces in between.
		/// Note that method overloads and combining this attribute with <see cref="ShowIfAttribute"/>
		/// or <see cref="HideIfAttribute"/> are currently unsupported.</param>
		public ButtonAttribute(params string[] buttonParams)
		{
#if UNITY_EDITOR
			order = -1100;
			buttons = new ButtonInfo[buttonParams.Length];
			if(buttonParams.Length > 8)
			{
				Debug.LogWarning($"ButtonAttribute supports a up to 8 buttons per property ({buttonParams.Length} provided).");
			}
			int length = Mathf.Min(buttonParams.Length, 8);
			for (int i = 0; i < length; i++)
			{
				var button = new ButtonInfo();
				buttons[i] = button;
				string[] split = buttonParams[i].Split(new char[] { ':' }, 2);
				
				//Method name and arguments
				var call = split[0];
				if (call.Contains("("))
				{
					if (!call.Contains(")"))
					{
						Debug.LogError($"Malformed method call detected in ButtonAttribute: {buttonParams[i]}");
						button.methodName = null;
						button.arguments = Array.Empty<string>();
					}
					else
					{
						//Get content between parentheses using regex
						var match = Regex.Match(call, @"\(([^)]*)\)");
						button.arguments = match.Value.Substring(1, match.Value.Length - 2).Split(',');
						var methodName = call.Substring(0, call.IndexOf('('));
						button.methodName = methodName;
					}
				}
				else
				{
					button.methodName = split[0];
					button.arguments = Array.Empty<string>();
				}
				
				//Button label
				if (split.Length > 1)
				{
					button.label = new GUIContent(split[1]);
				}
				else
				{
					int parenIndex = call.IndexOf('(');
					if (parenIndex >= 0) button.label = new GUIContent(UnityEditor.ObjectNames.NicifyVariableName(call.Substring(0, parenIndex)));
					else button.label = new GUIContent(UnityEditor.ObjectNames.NicifyVariableName(call));
				}
			}
#endif
		}
	}

	/// <summary>
	/// Draws a clickable button above or below a field in the inspector that is only available when not in play mode.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class EditorButtonAttribute : ButtonAttribute
	{
		public override Usage EnabledIn => Usage.EditMode;

		public EditorButtonAttribute(params string[] buttonParams) : base(buttonParams)
		{
		}
	}

	/// <summary>
	/// Draws a clickable button above or below a field in the inspector that is only available when in play mode.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class RuntimeButtonAttribute : ButtonAttribute
	{
		public override Usage EnabledIn => Usage.PlayMode;

		public RuntimeButtonAttribute(params string[] buttonParams) : base(buttonParams)
		{
		}
	}
}