using System;
using UnityEngine;

namespace UnityEssentials
{
	public static class ReflectionExtensions
	{
		public static Type[] GetInterfacesNonAlloc(this Type t)
		{
			return ReflectionUtility.GetInterfaces(t);
		}
	}
}