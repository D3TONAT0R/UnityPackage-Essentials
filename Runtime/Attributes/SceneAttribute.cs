using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Add this attribute to a string or int field to get a scene selection dropdown in the inspector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class SceneAttribute : PropertyAttribute
	{
		
	}
}
