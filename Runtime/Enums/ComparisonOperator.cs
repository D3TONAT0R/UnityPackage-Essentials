using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace D3T
{
	public enum ComparisonOperator 
	{
		[InspectorName("<")] Less,
		[InspectorName("<=")] LessOrEqual,
		[InspectorName("==")] Equal,
		[InspectorName(">=")] EqualOrMore,
		[InspectorName(">")] More,
	}
}