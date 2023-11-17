using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEssentials
{
	public static class TerrainExtensions
	{
		/// <summary>
		/// Converts the given world position to normalized terrain coordinates.
		/// </summary>
		public static Vector2 WorldPosToNormalizedTerrainCoord(this Terrain terrain, Vector3 worldPos)
		{
			var terrainPos = terrain.GetPosition();
			return new Vector2((worldPos.x - terrainPos.x) / terrain.terrainData.size.x, (worldPos.z - terrainPos.z) / terrain.terrainData.size.z);
		}

		/// <summary>
		/// Returns the world position from the given normalized terrain coordinate.
		/// </summary>
		public static Vector3 NormalizedTerrainCoordToWorldPos(this Terrain terrain, Vector2 normPos)
		{
			float y = terrain.terrainData.GetInterpolatedHeight(normPos.x, normPos.y);
			return terrain.GetPosition() + new Vector3(normPos.x * terrain.terrainData.size.x, y, normPos.y * terrain.terrainData.size.z);
		}

		/// <summary>
		/// Returns the absolute Y coordinate at the given world position.
		/// </summary>
		public static float GetAbsoluteHeightAt(this Terrain terrain, Vector3 worldPos)
		{
			var normCoord = WorldPosToNormalizedTerrainCoord(terrain, worldPos);
			return terrain.terrainData.GetInterpolatedHeight(normCoord.x, normCoord.y) + terrain.GetPosition().y;
		}

		/// <summary>
		/// Returns the absolute Y coordinate at the given world position.
		/// </summary>
		public static float GetAbsoluteHeightAt(this Terrain terrain, float worldPosX, float worldPosZ)
		{
			return GetAbsoluteHeightAt(terrain, new Vector3(worldPosX, 0, worldPosZ));
		}

		/// <summary>
		/// Returns an array containing all tree instances inside the given area.
		/// </summary>
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

		/// <summary>
		/// Returns an array containing all tree instances within the given radius from a given center point.
		/// </summary>
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

		/// <summary>
		/// Returns the world position of this tree instance.
		/// </summary>
		public static Vector3 GetWorldPosition(this TreeInstance tree, Terrain terrain)
		{
			return GetWorldPosition(tree, terrain.GetPosition(), terrain.terrainData.size);
		}

		/// <summary>
		/// Returns the world position of this tree instance.
		/// </summary>
		public static Vector3 GetWorldPosition(this TreeInstance tree, Vector3 terrainPosition, Vector3 terrainSize)
		{
			return terrainPosition + Vector3.Scale(tree.position, terrainSize);
		}
	} 
}
