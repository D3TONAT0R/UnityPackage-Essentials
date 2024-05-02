using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Collections
{
	public abstract class PolymorphicList<T> : IEnumerable<T> where T : class
	{
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
