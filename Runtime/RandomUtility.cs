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
		/// Picks a random item and removes it from the list.
		/// </summary>
		public static T TakeRandomItem<T>(List<T> list)
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

		/// <summary>
		/// Returns a shuffled version of the given array.
		/// </summary>
		public static T[] Shuffle<T>(T[] array)
		{
			T[] shuffled = new T[array.Length];
			var list = array.ToList();
			for(int i = 0; i < array.Length; i++)
			{
				shuffled[i] = TakeRandomItem(list);
			}
			return shuffled;
		}

		/// <summary>
		/// Randomly shuffles the given list.
		/// </summary>
		public static void Shuffle<T>(List<T> list)
		{
			var elements = new List<T>(list);
			list.Clear();
			for(int i = 0; i < elements.Count; i++)
			{
				list.Add(TakeRandomItem(elements));
			}
		}
	}
}