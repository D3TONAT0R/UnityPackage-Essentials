using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Collections
{
	[System.Serializable]
	public class PolymorphicStack<T> : IEnumerable<T> where T : StackElement
	{
		[SerializeField, SerializeReference]
		protected List<T> stack = new List<T>();

		[HideInInspector]
		public MonoBehaviour hostComponent;

		public T this[int index]
		{
			get => stack[index];
			set => stack[index] = value;
		}

		public int Count => stack.Count;

		public void Add(T item) => stack.Add(item);

		public bool Remove(T item) => stack.Remove(item);

		public void Swap(int indexA, int indexB)
		{
			var temp = stack[indexA];
			stack[indexA] = stack[indexB];
			stack[indexB] = temp;
		}

		public int IndexOf(T item) => stack.IndexOf(item);

		public void ForEach(System.Action<T> iterator)
		{
			for(int i = 0; i < stack.Count; i++)
			{
				if(stack[i] != null) iterator.Invoke(stack[i]);
			}
		}

		public void InvokeStart(MonoBehaviour hostComponent)
		{
			ForEach(e => e.InvokeStart(hostComponent));
		}

		public void InvokeUpdate()
		{
			ForEach(e => e.Update());
		}

		public void InvokeOnDrawGizmos(MonoBehaviour parent, bool selected)
		{
			ForEach(e => e.OnDrawGizmos(parent, selected));
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return stack.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return stack.GetEnumerator();
		}
	}
}
