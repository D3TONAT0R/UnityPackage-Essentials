using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityEssentials
{
	/// <summary>
	/// Helper class for random operations.
	/// </summary>
	public static class RandomUtility
	{
		[ThreadStatic]
		private static List<int> indices;
		
		/// <summary>
		/// Generates a random boolean using the given probability.
		/// </summary>
		public static bool Probability(float prob)
		{
			return Random.value <= prob;
		}

		/// <summary>
		/// Returns a random point within the given bounds.
		/// </summary>
		public static Vector2 RandomPoint2D(Vector2 min, Vector2 max)
		{
			return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
		}

		/// <summary>
		/// Returns a random point within the given bounds.
		/// </summary>
		public static Vector2 RandomPoint2D(float min, float max)
		{
			return new Vector2(Random.Range(min, max), Random.Range(min, max));
		}

		/// <summary>
		/// Returns a random point within the given bounds.
		/// </summary>
		public static Vector3 RandomPoint3D(Vector3 min, Vector3 max)
		{
			return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
		}

		/// <summary>
		/// Returns a random point within the given bounds.
		/// </summary>
		public static Vector3 RandomPoint3D(float min, float max)
		{
			return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
		}

		/// <summary>
		/// Returns a random point within the given bounds.
		/// </summary>
		public static Vector4 RandomPoint4D(Vector4 min, Vector4 max)
		{
			return new Vector4(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z), Random.Range(min.w, max.w));
		}

		/// <summary>
		/// Returns a random point within the given bounds.
		/// </summary>
		public static Vector4 RandomPoint4D(float min, float max)
		{
			return new Vector4(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
		}

		/// <summary>
		/// Picks a random item from the given array.
		/// </summary>
		/// <returns>A random item from the array.</returns>
		public static T PickRandom<T>(params T[] array)
		{
			return array[Random.Range(0, array.Length)];
		}

		/// <summary>
		/// Picks a random item from the given list.
		/// </summary>
		/// <returns>A random item from the list.</returns>
		public static T PickRandom<T>(List<T> list)
		{
			return list[Random.Range(0, list.Count)];
		}

		/// <summary>
		/// Picks a random item from the given array, excluding items at the given indices.
		/// </summary>
		/// <returns>A random item from the array.</returns>
		public static T PickRandomExcluding<T>(IList<T> list, params int[] excludeIndices)
		{
			int index = PickRandomIndexExcluding(list.Count, excludeIndices);
			if(index >= 0) return list[index];
			else return default;
		}

		/// <summary>
		/// Picks a random index between zero (inclusive) and the given length (exclusive), excluding the given indices.
		/// </summary>
		/// <returns>A random item from the array.</returns>
		public static int PickRandomIndexExcluding(int length, params int[] excludeIndices)
		{
			indices ??= new List<int>();
			indices.Clear();
			indices.AddRange(Enumerable.Range(0, length).Except(excludeIndices));
			if(indices.Count > 0)
			{
				return PickRandom(indices);
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// Picks a random item and removes it from the list.
		/// </summary>
		public static T TakeRandomItem<T>(IList<T> list)
		{
			int i = Random.Range(0, list.Count);
			var item = list[i];
			list.RemoveAt(i);
			return item;
		}

		/// <summary>
		/// Picks a random weighted index using the given weighted array.
		/// </summary>
		/// <param name="weights">A weighted array. Higher values have a greater chance of being picked.</param>
		/// <returns>The picked item's index.</returns>
		public static int PickRandomWeighted(IList<float> weights)
		{
			float total = 0;
			foreach(var w in weights) total += w;
			float pick = Random.value * total;
			for(int i = 0; i < weights.Count; i++)
			{
				pick -= weights[i];
				if(pick <= 0)
				{
					return i;
				}
			}
			return weights.Count - 1;
		}

		/// <summary>
		/// Randomly shuffles the given array.
		/// </summary>
		public static void Shuffle1<T>(IList<T> list)
		{
			for (int i = list.Count - 1; i > 0; i--)
			{
				int j = Random.Range(0, i + 1);
				(list[i], list[j]) = (list[j], list[i]);
			}
		}
	}
}