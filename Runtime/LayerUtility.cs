using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace D3T
{
	public static class LayerUtility
	{
		[Obsolete]
		/// <summary>
		/// Returns the collision mask for the given layer.
		/// </summary>
		public static LayerMask GetCollisionLayerMask(int layer)
		{
			return layer.GetCollisionLayerMask();
		}
	}
}
