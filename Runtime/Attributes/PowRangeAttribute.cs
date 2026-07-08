using UnityEngine;

namespace UnityEssentials
{
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