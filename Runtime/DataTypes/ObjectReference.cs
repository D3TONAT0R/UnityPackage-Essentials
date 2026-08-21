using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEssentials
{
	/// <summary>
	/// A struct that can hold a reference to a Unity Object of type T, which can be a Component, ScriptableObject,
	/// or a class or interface inherited by any MonoBehaviours.
	/// </summary>
	/// <typeparam name="T">The type constraint to use for the object picker in the inspector.</typeparam>
	[System.Serializable]
	public struct ObjectReference<T> where T : class // Constraint must be 'class' instead of 'Object' to allow for interface types
	{
		[SerializeField]
		private Object objectRef;
		
		/// <summary>
		/// Returns true if the reference points to an existing object of type T.
		/// </summary>
		public bool Exists => objectRef && Value != null;
		
		/// <summary>
		/// Returns the object reference as a Component.
		/// </summary>
		public Component Component => objectRef as Component;
		
		/// <summary>
		/// Returns the object reference as a ScriptableObject.
		/// </summary>
		public ScriptableObject ScriptableObject => objectRef as ScriptableObject;
		
		/// <summary>
		/// Returns the object reference as type T.
		/// </summary>
		public T Value => objectRef as T;
		
		/// <summary>
		/// The game object this object is attached to. If the reference is not a Component, returns null.
		/// </summary>
		public GameObject gameObject => Component ? Component.gameObject : null;
		
		/// <summary>
		/// The transform of the game object this object is attached to. If the reference is not a Component, returns null.
		/// </summary>
		public Transform transform => Component ? Component.transform : null;

		public ObjectReference(T value)
		{
			if (value != null)
			{
				if(value is Object o) objectRef = o;
				else throw new ArgumentException("Value must be a UnityEngine.Object or an interface inheriting from a UnityEngine.Object.");
			}
			else
			{
				objectRef = null;
			}
		}
		
		public static implicit operator T(ObjectReference<T> r) => r.Value;
		
		public static implicit operator ObjectReference<T>(T value) => new ObjectReference<T>(value);
		
		public static implicit operator bool(ObjectReference<T> r) => r.objectRef && r.Value != null;
	}
}