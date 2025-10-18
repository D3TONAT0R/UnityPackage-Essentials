using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Base class for prefix and suffix attributes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public abstract class AffixAttribute : PropertyAttribute
	{
		public readonly GUIContent content;
		public readonly float fixedWidth;

		protected AffixAttribute(string content, float fixedWidth = 0)
		{
			this.content = new GUIContent(content);
			this.fixedWidth = fixedWidth;
		}

		protected AffixAttribute(string content, string tooltip, float fixedWidth = 0)
		{
			this.content = new GUIContent(content, tooltip);
			this.fixedWidth = fixedWidth;
		}
	}

	/// <summary>
	/// Applies a prefix label in front of a field.
	/// </summary>
	public class PrefixAttribute : AffixAttribute
	{
		public PrefixAttribute(string prefix, float fixedWidth = 0) : base(prefix, fixedWidth)
		{
		}

		public PrefixAttribute(string prefix, string tooltip, float fixedWidth = 0) : base(prefix, tooltip, fixedWidth)
		{
		}
	}

	/// <summary>
	/// Applies a suffix label at the end of field.
	/// </summary>
	public class SuffixAttribute : AffixAttribute
	{
		public SuffixAttribute(string content, float fixedWidth = 0) : base(content, fixedWidth)
		{
		}

		public SuffixAttribute(string content, string tooltip, float fixedWidth = 0) : base(content, tooltip, fixedWidth)
		{
		}
	}
}