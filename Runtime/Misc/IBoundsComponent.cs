using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Defines properties for a bounding box.
	/// </summary>
	[System.Flags]
	public enum BoundsPropertyFlags
	{
		Transformable = 1,
		Offsetable = 2,
		Multiple = 4,

		AxisAlignedFixed = 0,
	}

	/// <summary>
	/// Interface for a component with a bounding box.
	/// </summary>
	public interface IBoundsComponent
	{

		BoundsPropertyFlags BoundsProperties { get; }

		void SetBounds(Bounds bounds, int index, bool moveTransform);
	}
}
