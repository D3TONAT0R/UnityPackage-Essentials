using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace D3T.Collections
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class PolymorphicDictionaryAttribute : Attribute
	{
		public Type[] specificTypes = null;

		public PolymorphicDictionaryAttribute()
		{

		}

		public PolymorphicDictionaryAttribute(params Type[] specificTypes)
		{
			this.specificTypes = specificTypes;
		}
	} 
}
