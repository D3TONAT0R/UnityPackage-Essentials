using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Add this attribute to an Enum field to have it drawn as horizontal buttons instead of the default dropdown.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class EnumButtonsAttribute : PropertyAttribute
	{

	}
}
