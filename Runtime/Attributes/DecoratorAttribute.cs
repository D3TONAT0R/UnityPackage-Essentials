using System;
using UnityEngine;

namespace D3T
{
	/// <summary>
	/// Base class for custom inspector field decorator attributes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public abstract class DecoratorAttribute : PropertyAttribute
	{

	}
}
