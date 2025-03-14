using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Makes a nullable float or int field restricted to a specific range.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class NullableRangeAttribute : PropertyAttribute
	{
		public float min;
		public float max;

		public NullableRangeAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
