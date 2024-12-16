using System;
using UnityEngine;

namespace UnityEssentials
{

	/// <summary>
	/// Draws a clickable button above or below a field in the inspector.
	/// </summary>
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

		public readonly string[] buttonNames;
		public readonly string[] buttonMethodNames;

		public bool Below { get; set; }

		public virtual Usage EnabledIn => Usage.Both;

		public ButtonAttribute(params string[] buttons)
		{
			buttonNames = new string[buttons.Length];
			buttonMethodNames = new string[buttons.Length];
			for(int i = 0; i < buttons.Length; i++)
			{
				string[] split = buttons[i].Split(new char[] { ':' }, 2);
				buttonMethodNames[i] = split[0];
				if(split.Length > 1)
				{
					buttonNames[i] = split[1];
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