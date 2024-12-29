using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// An axis with a positive or negative direction.
	/// </summary>
	public enum AxisDirection
	{
		[InspectorName("X-")]
		XNeg = 0,
		[InspectorName("X+")]
		XPos = 1,
		[InspectorName("Y-")]
		YNeg = 2,
		[InspectorName("Y+")]
		YPos = 3,
		[InspectorName("Z-")]
		ZNeg = 4,
		[InspectorName("Z+")]
		ZPos = 5
	}
}
