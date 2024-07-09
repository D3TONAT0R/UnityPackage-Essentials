using UnityEngine;

namespace D3T
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