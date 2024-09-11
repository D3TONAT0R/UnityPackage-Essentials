using System;
using UnityEngine;

namespace D3T
{
	public static class ReflectionExtensions
	{
		public static Type[] GetInterfacesNonAlloc(this Type t)
		{
			return ReflectionUtility.GetInterfaces(t);
		}
	}
}