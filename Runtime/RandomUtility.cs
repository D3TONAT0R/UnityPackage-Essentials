using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Helper class for random operations.
	/// </summary>
	public static class RandomUtility
	{

		/// <summary>
		/// Generates a random boolean using the given probability.
		/// </summary>
		public static bool Probability(float prob)
		{
			return Random.value <= prob;
		}

		/// <summary>
		/// Returns a random integer between min (inclusive) and max (exclusive), excluding the given integers.
		/// </summary>
		public static int RangeExcluding(int min, int max, params int[] exclude)
		{
			if(max - min <= 0) return -1;
			var array = Enumerable.Range(min, max - min).Except(exclude).ToArray();
			if(array.Length > 0)
			{
				return Random.Range(0, array.Length);
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// Picks a random item from the given array.
		/// </summary>
		/// <returns>A random item from the array.</returns>
		public static T PickRandom<T>(T[] array)
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
		public static T PickRandomExcluding<T>(T[] array, params int[] excludeIndices)
		{
			int index = PickRandomIndexExcluding(array.Length, excludeIndices);
			if(index >= 0) return array[index];
			else return default;
		}

		/// <summary>
		/// Picks a random item from the given list, excluding items at the given indices.
		/// </summary>
		/// <returns>A random item from the list.</returns>
		public static T PickRandomExcluding<T>(List<T> list, params int[] excludeIndices)
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
			var indices = Enumerable.Range(0, length).Except(excludeIndices).ToArray();
			if(indices.Length > 0)
			{
				return indices[Random.Range(0, indices.Length)];
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// Picks a random weighted index using the given weighted array.
		/// </summary>
		/// <param name="weights">A weighted array. Higher values have a greater chance of being picked.</param>
		/// <returns>The picked item's index.</returns>
		public static int PickRandomWeighted(float[] weights)
		{
			float total = 0;
			foreach(var w in weights) total += w;
			float pick = Random.value * total;
			for(int i = 0; i < weights.Length; i++)
			{
				pick -= weights[i];
				if(pick <= 0)
				{
					return i;
				}
			}
			return weights.Length - 1;
		}
	}
}