using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Defines a range using a min and max value, with an exponent for non-linear scaling.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class PowRangeAttribute : PropertyAttribute
	{
		public float min;
		public float max;
		public float exponent;

		public PowRangeAttribute(float min, float max, float exponent)
		{
			this.min = min;
			this.max = max;
			this.exponent = exponent;
		}
	}
}