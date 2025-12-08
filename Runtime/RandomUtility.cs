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
		/// Returns a random integer between min (inclusive) and max (exclusive), excluding the given integers.
		/// </summary>
		public static int RangeExcluding(int min, int max, params int[] exclude)
		{
			if(max - min <= 0) return -1;
			
			// Build valid range without LINQ to avoid allocations
			int rangeSize = max - min;
			int validCount = rangeSize;
			
			// Count how many values are excluded
			for(int i = 0; i < exclude.Length; i++)
			{
				if(exclude[i] >= min && exclude[i] < max)
				{
					validCount--;
				}
			}
			
			if(validCount <= 0) return -1;
			
			// Pick a random index in the valid range
			int pick = Random.Range(0, validCount);
			int current = min;
			
			// Skip excluded values
			for(int offset = 0; offset <= pick; offset++)
			{
				while(System.Array.IndexOf(exclude, current) >= 0)
				{
					current++;
				}
				if(offset < pick) current++;
			}
			
			return current;
		}

		/// <summary>
		/// Picks a random item from the given array.
		/// </summary>
		/// <returns>A random item from the array.</returns>
		public static T PickRandom<T>(params T[] array)
		{
			if(array == null || array.Length == 0)
			{
				throw new System.ArgumentException("Array cannot be null or empty");
			}
			return array[Random.Range(0, array.Length)];
		}

		/// <summary>
		/// Picks a random item from the given list.
		/// </summary>
		/// <returns>A random item from the list.</returns>
		public static T PickRandom<T>(List<T> list)
		{
			if(list == null || list.Count == 0)
			{
				throw new System.ArgumentException("List cannot be null or empty");
			}
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
			// Calculate valid count without LINQ to avoid allocations
			int validCount = length;
			
			for(int i = 0; i < excludeIndices.Length; i++)
			{
				if(excludeIndices[i] >= 0 && excludeIndices[i] < length)
				{
					validCount--;
				}
			}
			
			if(validCount <= 0) return -1;
			
			// Pick a random index in the valid range
			int pick = Random.Range(0, validCount);
			int current = 0;
			
			// Skip excluded indices
			for(int offset = 0; offset <= pick; offset++)
			{
				while(System.Array.IndexOf(excludeIndices, current) >= 0)
				{
					current++;
				}
				if(offset < pick) current++;
			}
			
			return current;
		}

		/// <summary>
		/// Picks a random item and removes it from the list.
		/// </summary>
		public static T TakeRandomItem<T>(List<T> list)
		{
			if(list == null || list.Count == 0)
			{
				throw new System.ArgumentException("List cannot be null or empty");
			}
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
		public static int PickRandomWeighted(float[] weights)
		{
			if(weights == null || weights.Length == 0)
			{
				throw new System.ArgumentException("Weights array cannot be null or empty");
			}
			float total = 0;
			foreach(var w in weights) total += w;
			if(total <= 0)
			{
				throw new System.ArgumentException("Total weight must be greater than zero");
			}
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

		/// <summary>
		/// Returns a shuffled version of the given array using the Fisher-Yates algorithm.
		/// </summary>
		public static T[] Shuffle<T>(T[] array)
		{
			T[] shuffled = new T[array.Length];
			System.Array.Copy(array, shuffled, array.Length);
			
			// Fisher-Yates shuffle algorithm
			for(int i = shuffled.Length - 1; i > 0; i--)
			{
				int j = Random.Range(0, i + 1);
				T temp = shuffled[i];
				shuffled[i] = shuffled[j];
				shuffled[j] = temp;
			}
			return shuffled;
		}

		/// <summary>
		/// Randomly shuffles the given list using the Fisher-Yates algorithm.
		/// </summary>
		public static void Shuffle<T>(List<T> list)
		{
			// Fisher-Yates shuffle algorithm - in-place
			for(int i = list.Count - 1; i > 0; i--)
			{
				int j = Random.Range(0, i + 1);
				T temp = list[i];
				list[i] = list[j];
				list[j] = temp;
			}
		}
	}
}