using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// An axis with a positive or negative direction.
	/// </summary>
	public enum AxisDirection
	{
		[InspectorName("X-")]
		XNeg,
		[InspectorName("X+")]
		XPos,
		[InspectorName("Y-")]
		YNeg,
		[InspectorName("Y+")]
		YPos,
		[InspectorName("Z-")]
		ZNeg,
		[InspectorName("Z+")]
		ZPos
	}
}
