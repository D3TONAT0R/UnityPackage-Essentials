using System;

namespace UnityEssentials
{
	/// <summary>
	/// Marks this inspector field as runtime-only, making it editable only after the game object is initialized.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class RuntimeOnlyAttribute : PropertyModifierAttribute
	{
	}
}
