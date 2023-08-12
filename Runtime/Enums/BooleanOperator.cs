using System.Collections;
using UnityEngine;

namespace D3T
{
	public enum BooleanOperator {
		[InspectorName("AND (Match All)")]
		AND,
		[InspectorName("OR (Match Any)")]
		OR,
		[InspectorName("XOR (Match Only One)")]
		XOR
	}

}