using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// An operator for boolean operations.
	/// </summary>
	public enum BooleanOperator
	{
		[InspectorName("AND (Match All)")]
		And = 0,
		[InspectorName("OR (Match Any)")]
		Or = 1,
		[InspectorName("XOR (Match Only One)")]
		Xor = 2
	}
}