using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Constrains a <see cref="FloatRange"/> or <see cref="IntRange"/> field to the given range.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class MinMaxRangeAttribute : PropertyAttribute
	{
		public float min;
		public float max;

		public MinMaxRangeAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
