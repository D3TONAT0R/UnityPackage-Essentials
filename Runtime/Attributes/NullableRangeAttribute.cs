using UnityEngine;

namespace D3T
{
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
