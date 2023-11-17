using UnityEngine;
using System;

namespace UnityEssentials
{
	/// <summary>
	/// Base class for custom inspector field decorator attributes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public abstract class DecoratorAttribute : PropertyAttribute
	{
		
	}
}
