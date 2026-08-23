using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Add this attribute to a string field to create a tag popup (single tag only).
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class TagPopupAttribute : PropertyAttribute
	{

	}
}
