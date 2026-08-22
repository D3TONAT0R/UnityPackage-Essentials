using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Add this attribute to an integer field to create a layer popup (single layer only).
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class LayerPopupAttribute : PropertyAttribute
	{

	}
}
