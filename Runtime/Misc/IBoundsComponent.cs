using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace D3T.Utility
{
	[System.Flags]
	public enum BoundsPropertyFlags
	{
		Transformable = 1,
		Offsetable = 2,
		Multiple = 4,

		AxisAlignedFixed = 0,
	}

	public interface IBoundsComponent
	{

		BoundsPropertyFlags BoundsProperties { get; }

		void SetBounds(Bounds bounds, int index, bool moveTransform);
	}
}
