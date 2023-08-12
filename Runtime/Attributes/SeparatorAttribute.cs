using UnityEngine;

namespace D3T
{
	public class SeparatorAttribute : PropertyAttribute
	{
		public readonly bool fullWidth;

		public SeparatorAttribute(bool fullWidth = true)
		{
			this.fullWidth = fullWidth;
		}
	} 
}
