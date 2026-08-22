using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Attribute that allows for a dynamic range to be defined for a float or int field in the Unity Inspector. The range can be defined using either constant values or other serialized fields.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
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