using UnityEngine;
using System.Collections.Generic;

namespace D3T
{
	public static class ObjectUtility
	{
		public static void DestroyAllComponents<T>(List<T> components) where T : Component
		{
			for(int i = 0; i < components.Count; i++)
			{
				if(components[i]) Object.Destroy(components[i]);
			}
			components.Clear();
		}

		public static void DestroyAllGameObjects<T>(List<T> components) where T : Component
		{
			for(int i = 0; i < components.Count; i++)
			{
				if(components[i] && components[i].gameObject) Object.Destroy(components[i].gameObject);
			}
			components.Clear();
		}

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
