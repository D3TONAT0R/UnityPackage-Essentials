using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Add this attribute to a GameObject field to only allow the user to assign prefabs to it.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class PrefabAttribute : PropertyAttribute
	{
		
	}
}
