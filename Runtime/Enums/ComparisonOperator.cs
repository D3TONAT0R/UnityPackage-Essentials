using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// An operator for comparing two values.
	/// </summary>
	public enum ComparisonOperator
	{
		[InspectorName("<")] Less,
		[InspectorName("<=")] LessOrEqual,
		[InspectorName("==")] Equal,
		[InspectorName(">=")] MoreOrEqual,
		[InspectorName(">")] More,
	}
}