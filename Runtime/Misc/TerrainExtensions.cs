using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D3T
{
	public static class TerrainExtensions
	{
		public static Vector2 WorldPosToNormalizedTerrainCoord(this Terrain terrain, Vector3 worldPos)
		{
			var terrainPos = terrain.GetPosition();
			return new Vector2((worldPos.x - terrainPos.x) / terrain.terrainData.size.x, (worldPos.z - terrainPos.z) / terrain.terrainData.size.z);
		}

		public static Vector3 NormalizedTerrainCoordToWorldPos(this Terrain terrain, Vector2 normPos)
		{
			float y = terrain.terrainData.GetInterpolatedHeight(normPos.x, normPos.y);
			return terrain.GetPosition() + new Vector3(normPos.x * terrain.terrainData.size.x, y, normPos.y * terrain.terrainData.size.z);
		}

		public static float GetAbsoluteHeightAt(this Terrain terrain, Vector3 worldPos)
		{
			var normCoord = WorldPosToNormalizedTerrainCoord(terrain, worldPos);
			return terrain.terrainData.GetInterpolatedHeight(normCoord.x, normCoord.y) + terrain.GetPosition().y;
		}

		public static TreeInstance[] GetTreesInArea(this Terrain terrain, Vector3 from, Vector3 to, bool ignoreHeight)
		{
			var offset = terrain.GetPosition();
			var size = terrain.terrainData.size;
			List<TreeInstance> trees = new List<TreeInstance>();
			if(ignoreHeight)
			{
				return terrain.terrainData.treeInstances.Where((tree) =>
					tree.GetWorldPosition(offset, size).XZ().IsBetween(from.XZ(), to.XZ())
				).ToArray();
			}
			else
			{
				return terrain.terrainData.treeInstances.Where((tree) =>
					tree.GetWorldPosition(offset, size).IsBetween(from, to)
				).ToArray();
			}
		}

		public static TreeInstance[] GetTreesWithinRadis(this Terrain terrain, Vector3 center, float radius, bool ignoreHeight)
		{
			var offset = terrain.GetPosition();
			var size = terrain.terrainData.size;
			List<TreeInstance> trees = new List<TreeInstance>();
			if(ignoreHeight)
			{
				center.y = 0;
			}
			if(ignoreHeight)
			{
				return terrain.terrainData.treeInstances.Where((tree) =>
					Vector2.Distance(tree.GetWorldPosition(offset, size).XZ(), center.XZ()) <= radius
				).ToArray();
			}
			else
			{
				return terrain.terrainData.treeInstances.Where((tree) =>
					Vector3.Distance(tree.GetWorldPosition(offset, size), center) <= radius
				).ToArray();
			}
		}

		public static Vector3 GetWorldPosition(this TreeInstance tree, Terrain terrain)
		{
			return GetWorldPosition(tree, terrain.GetPosition(), terrain.terrainData.size);
		}

		public static Vector3 GetWorldPosition(this TreeInstance tree, Vector3 terrainPosition, Vector3 terrainSize)
		{
			return terrainPosition + Vector3.Scale(tree.position, terrainSize);
		}
	} 
}
