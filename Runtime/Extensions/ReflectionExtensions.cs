using System;
using System.Reflection;
using UnityEngine;

namespace UnityEssentials
{
	public static class ReflectionExtensions
	{
		public static object GetValue(this MemberInfo m, object obj)
		{
			if(m is FieldInfo fi) return fi.GetValue(obj);
			if(m is PropertyInfo pi) return pi.GetValue(obj);
			throw new ArgumentException("MemberInfo must be of type FieldInfo or PropertyInfo");
		}
		
		public static void SetValue(this MemberInfo m, object obj, object value)
		{
			if(m is FieldInfo fi) fi.SetValue(obj, value);
			else if(m is PropertyInfo pi) pi.SetValue(obj, value);
			else throw new ArgumentException("MemberInfo must be of type FieldInfo or PropertyInfo");
		}
		
		public static bool CanRead(this MemberInfo m)
		{
			if(m is FieldInfo) return true;
			else if(m is PropertyInfo pi) return pi.CanRead;
			else if(m is MethodInfo) return true;
			else throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo or MethodInfo");
		}
		
		public static bool CanWrite(this MemberInfo m)
		{
			if(m is FieldInfo) return true;
			else if(m is PropertyInfo pi) return pi.CanWrite;
			else if(m is MethodInfo) return false;
			else throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo or MethodInfo");
		}
		
		public static Type GetValueType(this MemberInfo m)
		{
			if(m is FieldInfo fi) return fi.FieldType;
			else if(m is PropertyInfo pi) return pi.PropertyType;
			else if(m is MethodInfo mi) return mi.ReturnType;
			else throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo or MethodInfo");
		}
		
		public static bool IsStatic(this MemberInfo m)
		{
			if(m is FieldInfo fi) return fi.IsStatic;
			else if(m is PropertyInfo pi) return (pi.GetGetMethod(true)?.IsStatic ?? false) || (pi.GetSetMethod(true)?.IsStatic ?? false);
			else if(m is MethodInfo mi) return mi.IsStatic;
			else throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo or MethodInfo");
		}
		
		public static bool IsPublic(this MemberInfo m) 
		{
			if(m is FieldInfo fi) return fi.IsPublic;
			else if(m is PropertyInfo pi) return (pi.GetGetMethod(true)?.IsPublic ?? false) || (pi.GetSetMethod(true)?.IsPublic ?? false);
			else if(m is MethodInfo mi) return mi.IsPublic;
			else throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo or MethodInfo");
		}
		
		public static Type[] GetInterfacesNonAlloc(this Type t)
		{
			return ReflectionUtility.GetInterfaces(t);
		}
	}
}