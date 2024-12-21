using UnityEngine;

namespace UnityEssentials
{
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
