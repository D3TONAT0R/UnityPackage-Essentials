using UnityEngine;

namespace UnityEssentials
{
	public static class ComponentExtensions
	{
		/// <summary>
		/// Gets the component of the given type attached to this GameObject. If it doesn't exist, a new one is created.
		/// </summary>
		public static T GetOrAddComponent<T>(this Component c) where T : Component
		{
			if(c.TryGetComponent<T>(out var comp))
			{
				return comp;
			}
			else
			{
				return c.gameObject.AddComponent<T>();
			}
		}

		/// <summary>
		/// Gets the component of the given type attached to this GameObject. If it doesn't exist, an exception is thrown.
		/// </summary>
		public static T GetRequiredComponent<T>(this Component c)
		{
			T comp = c.GetComponent<T>();
			if(comp == null) throw new System.NullReferenceException($"Could not find required component {typeof(T).Name} on GameObject '{c.name}'.");
			return comp;
		}
	}
}
