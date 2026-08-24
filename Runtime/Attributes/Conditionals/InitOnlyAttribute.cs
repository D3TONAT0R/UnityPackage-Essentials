using System;

namespace UnityEssentials
{
	/// <summary>
	/// Marks this inspector field as init-only, making it editable only before the game object is initialized.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class InitOnlyAttribute : PropertyModifierAttribute
	{
	}
}
