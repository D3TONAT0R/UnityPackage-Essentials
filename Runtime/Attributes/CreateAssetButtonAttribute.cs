﻿using System;
using UnityEngine;

namespace D3T
{

	/// <summary>
	/// Add this attribute to an object field to add a button that creates a new asset for that type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class CreateAssetButtonAttribute : PropertyAttribute
	{
		public readonly string defaultPath;

		public CreateAssetButtonAttribute(string defaultPath = null)
		{
			this.defaultPath = defaultPath;
		}
	}
}