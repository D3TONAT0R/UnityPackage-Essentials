using System;

namespace D3T
{
	/// <summary>
	/// Draws a separator line above an inspector field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class SeparatorAttribute : DecoratorAttribute
	{
		public readonly bool fullWidth;

		public SeparatorAttribute(bool fullWidth = true)
		{
			this.fullWidth = fullWidth;
		}
	} 
}
