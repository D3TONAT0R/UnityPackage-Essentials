using System;
using UnityEngine;

namespace D3T
{
#if !UNITY_2022_2_OR_NEWER
	public enum MaterialPropertyType
	{
		Float,
		Int,
		Vector,
		Matrix,
		Texture,
		ConstantBuffer,
		ComputeBuffer
	}
#endif

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class MaterialPropertyHintAttribute : Attribute
	{
		public MaterialPropertyType propertyType;

		public string targetMemberName = null;

		public MaterialPropertyHintAttribute(MaterialPropertyType propertyType)
		{
			this.propertyType = propertyType;
		}

		public MaterialPropertyHintAttribute(MaterialPropertyType propertyType, string targetMemberName)
		{
			this.propertyType = propertyType;
			this.targetMemberName = targetMemberName;
		}
	}
}
