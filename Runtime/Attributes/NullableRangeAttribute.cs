using System;
using UnityEngine;

namespace D3T
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
