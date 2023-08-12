using System.Collections.Generic;
using UnityEngine;

namespace D3T
{
	public class IntPopupAttribute : PropertyAttribute
	{
		public int[] choices;

		public IntPopupAttribute(params int[] choices)
		{
			this.choices = choices;
		}
	}

	public class IntPopupRangeAttribute : IntPopupAttribute
	{
		public IntPopupRangeAttribute(int from, int to)
		{
			List<int> ints = new List<int>();
			for(int i = from; i <= to; i++)
			{
				ints.Add(i);
			}
			choices = ints.ToArray();
		}
	}
}
