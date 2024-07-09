using System;
using UnityEngine;

namespace D3T
{
	/// <summary>
	/// Constrains a <see cref="FloatRange"/> field to the given range.
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
