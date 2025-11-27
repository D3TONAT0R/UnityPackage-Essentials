using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public class CustomElementDrawerAttribute : Attribute
	{
		public Type targetType;

		public CustomElementDrawerAttribute(Type targetType)
		{
			this.targetType = targetType;
		}
	} 
}
