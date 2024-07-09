﻿using System;

namespace D3T.Collections
{
	/// <summary>
	/// Specifies how a <see cref="PolymorphicList{T}"/> or <see cref="UnityDictionary{K, V}"/> should support polymorphism.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class PolymorphicAttribute : Attribute
	{
		public Type[] specificTypes = null;

		public PolymorphicAttribute()
		{

		}

		public PolymorphicAttribute(params Type[] specificTypes)
		{
			this.specificTypes = specificTypes;
		}
	}
}
