using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Collections
{
	/// <summary>
	/// Wrapper for a generic list that can be serialized and supports polymorphism.
	/// </summary>
	public abstract class PolymorphicList<T> : IEnumerable<T> where T : class
	{
		/// <summary>
		/// The actual list.
		/// </summary>
		[SerializeReference]
		public List<T> list = new List<T>();

		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}
