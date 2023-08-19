using UnityEngine;

namespace D3T
{
	public class SeparatorAttribute : DecoratorAttribute
	{
		public readonly bool fullWidth;

		public SeparatorAttribute(bool fullWidth = true)
		{
			this.fullWidth = fullWidth;
		}
	} 
}
