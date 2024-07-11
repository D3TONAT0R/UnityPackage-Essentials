using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Helper class for game object related operations.
	/// </summary>
	public static class ObjectUtility
	{
		/// <summary>
		/// Destroys all components in the given list, then clears the list.
		/// </summary>
		public static void DestroyAllComponents<T>(List<T> components) where T : Component
		{
			for(int i = 0; i < components.Count; i++)
			{
				if(components[i]) Object.Destroy(components[i]);
			}
			components.Clear();
		}

		/// <summary>
		/// Destroys all game objects related to the components in the given list, then clears the list.
		/// </summary>
		public static void DestroyAllGameObjects<T>(List<T> components) where T : Component
		{
			for(int i = 0; i < components.Count; i++)
			{
				if(components[i] && components[i].gameObject) Object.Destroy(components[i].gameObject);
			}
			components.Clear();
		}

		/// <summary>
		/// Destroys all game objects in the given list, then clears the list.
		/// </summary>
		public static void DestroyAllGameObjects(List<GameObject> gameObjects)
		{
			for(int i = 0; i < gameObjects.Count; i++)
			{
				if(gameObjects[i]) Object.Destroy(gameObjects[i].gameObject);
			}
			gameObjects.Clear();
		}
	}
}
