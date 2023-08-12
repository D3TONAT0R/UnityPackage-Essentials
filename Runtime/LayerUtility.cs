using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace D3T
{
	public static class LayerUtility
	{
		public static LayerMask GetCollisionLayerMask(int layer)
		{
			int mask = 0;
			for(int i = 0; i < 32; i++)
			{
				if(!Physics.GetIgnoreLayerCollision(layer, i)) mask += 1 << i;
			}
			return mask;
		}
	}
}
