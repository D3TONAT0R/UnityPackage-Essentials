using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Attribute used to configure the usage of a color field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class NullableColorUsageAttribute : PropertyAttribute
	{
		public bool showAlpha = true;
		public bool hdr = false;

		public NullableColorUsageAttribute(bool showAlpha)
		{
			this.showAlpha = showAlpha;
		}

		public NullableColorUsageAttribute(bool showAlpha, bool hdr)
		{
			this.showAlpha = showAlpha;
			this.hdr = hdr;
		}
	} 
}
