using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityEssentials
{
	internal static class UnityInternals
	{
		private static readonly HashSet<string> warned = new HashSet<string>();

		public static FieldInfo Field(Type owner, string name)
		{
			var f = owner?.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (f == null) Warn($"{owner?.FullName}.{name}");
			return f;
		}

		public static MethodInfo Method(Type owner, string name, BindingFlags flags)
		{
			var m = owner?.GetMethod(name, flags);
			if (m == null) Warn($"{owner?.FullName}.{name}()");
			return m;
		}

		public static Type Type(string qualifiedName)
		{
			var t = System.Type.GetType(qualifiedName);
			if (t == null) Warn(qualifiedName);
			return t;
		}

		private static void Warn(string path)
		{
			if (warned.Add(path)) Debug.LogError($"Unity internal '{path}' not found");
		}
	}
}