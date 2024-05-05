using UnityEngine;

namespace D3T
{
	/// <summary>
	/// Makes a float field display as a progress bar in the inspector.
	/// </summary>
	public class ProgressBarAttribute : PropertyAttribute
	{
		public readonly float min;
		public readonly float max;
		public bool showAsPercentage;

		public float valueScale = 1f;
		public int decimals = 1;

		public ProgressBarAttribute(float min, float max, bool showAsPercentage = true)
		{
			this.min = min;
			this.max = max;
			this.showAsPercentage = showAsPercentage;
		}
	}
}
