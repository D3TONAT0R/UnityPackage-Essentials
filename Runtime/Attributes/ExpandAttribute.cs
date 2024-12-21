using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Add this attribute to a class field to make it expanded or to an object reference field to make it expandable.
	/// </summary>
	[System.Serializable]
	public class ExpandAttribute : PropertyAttribute
	{
		public readonly bool drawBox;

		public ExpandAttribute(bool drawBox = true)
		{
			this.drawBox = drawBox;
		}
	}
}
