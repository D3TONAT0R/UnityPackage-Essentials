using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Applies a suffix label to the end of a field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class SuffixAttribute : PropertyAttribute
	{
		public readonly GUIContent suffix;
		public readonly float fixedWidth;

		public SuffixAttribute(string suffix, float fixedWidth = 0)
		{
			this.suffix = new GUIContent(suffix);
			this.fixedWidth = fixedWidth;
		}

		public SuffixAttribute(string suffix, string tooltip, float fixedWidth = 0)
		{
			this.suffix = new GUIContent(suffix, tooltip);
			this.fixedWidth = fixedWidth;
		}
	}
}