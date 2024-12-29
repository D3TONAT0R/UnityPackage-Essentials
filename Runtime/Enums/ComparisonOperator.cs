using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// An operator for comparing two values.
	/// </summary>
	public enum ComparisonOperator
	{
		[InspectorName("<")] Less = 0,
		[InspectorName("<=")] LessOrEqual = 1,
		[InspectorName("==")] Equal = 2,
		[InspectorName(">=")] MoreOrEqual = 3,
		[InspectorName(">")] More = 4,
	}
}