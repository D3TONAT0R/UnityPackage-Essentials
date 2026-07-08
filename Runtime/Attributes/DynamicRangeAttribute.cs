using UnityEngine;

namespace UnityEssentials
{
	public class DynamicRangeAttribute : PropertyAttribute
	{
		public string minPropertyName;
		public float minValue;
		public string maxPropertyName;
		public float maxValue;
		public bool softLimits;

		public DynamicRangeAttribute(float minValue, float maxValue, bool softLimits = false)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.softLimits = softLimits;
		}
		
		public DynamicRangeAttribute(string minPropertyName, string maxPropertyName, bool softLimits = false)
		{
			this.minPropertyName = minPropertyName;
			this.maxPropertyName = maxPropertyName;
			this.softLimits = softLimits;
		}
		
		public DynamicRangeAttribute(string minPropertyName, float maxValue, bool softLimits = false)
		{
			this.minPropertyName = minPropertyName;
			this.maxValue = maxValue;
			this.softLimits = softLimits;
		}
		
		public DynamicRangeAttribute(float minValue, string maxPropertyName, bool softLimits = false)
		{
			this.minValue = minValue;
			this.maxPropertyName = maxPropertyName;
			this.softLimits = softLimits;
		}
	}
}