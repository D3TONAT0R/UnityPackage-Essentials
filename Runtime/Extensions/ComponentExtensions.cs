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
		/// Gets the component of the given type attached to this GameObject. If it doesn't exist, a new one is created.
		/// </summary>
		public static T GetOrAddComponent<T>(this GameObject g) where T : Component
		{
			if(g.TryGetComponent<T>(out var comp))
			{
				return comp;
			}
			else
			{
				return g.gameObject.AddComponent<T>();
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

		/// <summary>
		/// Gets the component of the given type attached to this GameObject. If it doesn't exist, an exception is thrown.
		/// </summary>
		public static T GetRequiredComponent<T>(this GameObject g)
		{
			T comp = g.GetComponent<T>();
			if(comp == null) throw new System.NullReferenceException($"Could not find required component {typeof(T).Name} on GameObject '{g.name}'.");
			return comp;
		}

		/// <summary>
		/// (EDITOR ONLY) Invokes the given action with a slight delay. Useful for avoiding "SendMessage" related warnings during an OnValidate event. Does nothing in builds.
		/// </summary>
		public static void EditorDelayCall(this MonoBehaviour m, System.Action onValidateAction)
		{
#if UNITY_EDITOR
			bool wasPlaying = Application.isPlaying;
			UnityEditor.EditorApplication.delayCall += _OnValidate;

			void _OnValidate()
			{
				UnityEditor.EditorApplication.delayCall -= _OnValidate;
				if(Application.isPlaying == wasPlaying && m != null)
				{
					onValidateAction();
				}
			}
#endif
		}

		/// <summary>
		/// (EDITOR ONLY) Checks if this GameObject is currently selected in the editor. Always returns false in builds.
		/// </summary>
		public static bool IsSelectedInEditor(this GameObject go)
		{
#if UNITY_EDITOR
			return UnityEditor.Selection.Contains(go);
#else
			return false;
#endif
		}

		/// <summary>
		/// (EDITOR ONLY) Checks if this Component is currently selected in the editor. Always returns false in builds.
		/// </summary>
		public static bool IsSelectedInEditor(this Component c)
		{
#if UNITY_EDITOR
			return IsSelectedInEditor(c.gameObject);
#else
			return false;
#endif
		}

		/// <summary>
		/// (EDITOR ONLY) Checks if this GameObject is the active selection in the editor. Always returns false in builds.
		/// </summary>
		public static bool IsActiveSelectionInEditor(this GameObject go)
		{
#if UNITY_EDITOR
			return UnityEditor.Selection.activeGameObject == go;
#else
			return false;
#endif
		}

		/// <summary>
		/// (EDITOR ONLY) Checks if this Component is the active selection in the editor. Always returns false in builds.
		///	</summary>
		public static bool IsActiveSelectionInEditor(this Component c)
		{
#if UNITY_EDITOR
			return IsActiveSelectionInEditor(c.gameObject);
#else
			return false;
#endif
		}
	}
}
