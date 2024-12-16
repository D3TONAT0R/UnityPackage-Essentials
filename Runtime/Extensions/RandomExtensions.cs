using System.Collections.Generic;

namespace UnityEssentials
{
	public static class RandomExtensions
	{
		/// <summary>
		/// Picks a random item from the given array.
		/// </summary>
		/// <returns>A random item from the array.</returns>
		public static T PickRandom<T>(this T[] array)
		{
			return RandomUtility.PickRandom(array);
		}

		/// <summary>
		/// Picks a random item from the given list.
		/// </summary>
		/// <returns>A random item from the list.</returns>
		public static T PickRandom<T>(this List<T> list)
		{
			return RandomUtility.PickRandom(list);
		}

		/// <summary>
		/// Picks a random item from the given array, excluding items at the given indices.
		/// </summary>
		/// <returns>A random item from the array.</returns>
		public static T PickRandomExcluding<T>(this T[] array, params int[] excludeIndices)
		{
			return RandomUtility.PickRandomExcluding(array, excludeIndices);
		}

		/// <summary>
		/// Picks a random item from the given list, excluding items at the given indices.
		/// </summary>
		/// <returns>A random item from the list.</returns>
		public static T PickRandomExcluding<T>(this List<T> list, params int[] excludeIndices)
		{
			return RandomUtility.PickRandomExcluding(list, excludeIndices);
		}

		/// <summary>
		/// Picks a random item and removes it from the list.
		/// </summary>
		public static T TakeRandomItem<T>(this List<T> list)
		{
			return RandomUtility.TakeRandomItem(list);
		}
	}
}