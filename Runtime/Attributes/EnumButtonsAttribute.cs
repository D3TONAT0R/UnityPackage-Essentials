using System;
using UnityEngine;

namespace D3T
{
	/// <summary>
	/// Add this attribute to an Enum field to have it drawn as horizontal buttons instead of the default dropdown.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class EnumButtonsAttribute : PropertyAttribute
	{

	} 
}
