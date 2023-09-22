using System;
using UnityEngine;

namespace D3T
{
	/// <summary>
	/// Add this attribute to a field to show an error box if the object is null or does not have the specified component(s).
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class RequiredAttribute : PropertyAttribute
	{
		public bool errorIfNull = true;
		public Type[] components;

		public RequiredAttribute(params Type[] requiredComponents)
		{
			components = requiredComponents;
		}
	} 
}
