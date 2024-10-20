﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace D3T.Collections
{
	/// <summary>
	/// Interface for a dictionary that can be serialized by unity.
	/// </summary>
	public interface IUnityDictionary
	{
		/// <summary>
		/// Returns true if the contents of the dictionary are valid.
		/// </summary>
		bool Valid { get; }

		/// <summary>
		/// The type of the keys in the dictionary.
		/// </summary>
		Type KeyType { get; }

		/// <summary>
		/// The type of the values in the dictionary.
		/// </summary>
		Type ValueType { get; }

		/// <summary>
		/// The number of items in the dictionary.
		/// </summary>
		int Count { get; }

		System.Exception SerializationException { get; }

		/// <summary>
		/// Clears the dictionary.
		/// </summary>
		void Clear();

#if UNITY_EDITOR
		void Editor_Add(Type valueType);

		void Editor_Remove(int index);

		void Editor_Swap(int from, int to);

		void Editor_ReplaceValue(object key, object newValue);
#endif
	}

	/// <summary>
	/// Interface for a value type in a Unity.
	/// </summary>
	public interface IUnityDictionaryValue { }

	/// <summary>
	/// A value inside a UnityDictionary.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[System.Serializable]
	public abstract class UnityDictionaryValue<T> : IUnityDictionaryValue
	{
		public T value;

		public static implicit operator T(UnityDictionaryValue<T> c) => c.value;
	}

	[System.Serializable] public class BoolValue : UnityDictionaryValue<bool> { }
	[System.Serializable] public class IntValue : UnityDictionaryValue<int> { }
	[System.Serializable] public class FloatValue : UnityDictionaryValue<float> { }
	[System.Serializable] public class StringValue : UnityDictionaryValue<string> { }
	[System.Serializable] public class Vector2Value : UnityDictionaryValue<Vector2> { }
	[System.Serializable] public class Vector3Value : UnityDictionaryValue<Vector3> { }
	[System.Serializable] public class Vector4Value : UnityDictionaryValue<Vector4> { }

	/// <summary>
	/// A wrapper for .NETs <see cref="Dictionary{TKey, TValue}"/> that supports serialization in Unity.
	/// </summary>
	/// <typeparam name="K"></typeparam>
	/// <typeparam name="V"></typeparam>
	public class UnityDictionary<K, V> : IUnityDictionary, ISerializationCallbackReceiver, IEnumerable<KeyValuePair<K, V>>
	{
		protected Dictionary<K, V> dictionary = new Dictionary<K, V>();

		[SerializeField]
		protected List<K> _keys = new List<K>();
		[SerializeField, SerializeReference]
		protected List<V> _values = new List<V>();

		public Type KeyType => typeof(K);
		public Type ValueType => typeof(V);
		public virtual Dictionary<K, V>.KeyCollection Keys => dictionary.Keys;
		public virtual Dictionary<K, V>.ValueCollection Values => dictionary.Values;
		public virtual int Count => dictionary.Count;

		public System.Exception SerializationException { get; private set; }
		public bool Valid => SerializationException == null;

		public virtual bool UseMonospaceKeyLabels => true;

		public UnityDictionary()
		{

		}

		public UnityDictionary(IDictionary<K, V> dictionary)
		{
			this.dictionary = new Dictionary<K, V>(dictionary);
		}

		public UnityDictionary(Dictionary<K, V> dictionary)
		{
			this.dictionary = new Dictionary<K, V>(dictionary);
		}

		public virtual V this[K key]
		{
			get => dictionary[key];
			set => dictionary[key] = value;
		}

		public virtual void Add(K key, V value) => dictionary.Add(key, value);
		public virtual bool Remove(K key) => dictionary.Remove(key);
		public virtual bool ContainsKey(K key) => dictionary.ContainsKey(key);
		public virtual bool ContainsValue(V value) => dictionary.ContainsValue(value);
		public virtual bool TryGetValue(K key, out V value) => dictionary.TryGetValue(key, out value);
		public virtual void Clear() => dictionary.Clear();
		public Dictionary<K, V> ToDictionary() => new Dictionary<K, V>(dictionary);
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => dictionary.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();

		public void OnBeforeSerialize()
		{
			/*
			if(dictionary != null)
			{
				_keys = dictionary.Keys.ToList();
				_values = dictionary.Values.ToList();
			}
			*/
		}

		//TODO: has problems when keys are of type UnityEngine.Object
		public void OnAfterDeserialize()
		{
			SerializationException = null;
			try
			{
				dictionary = new Dictionary<K, V>();
				for(int i = 0; i < _keys.Count; i++)
				{
					bool notNull;
					if(typeof(UnityEngine.Object).IsAssignableFrom(KeyType))
					{
						notNull = (_keys[i] as UnityEngine.Object) != null;
					}
					else
					{
						notNull = _keys[i] != null;
					}
					if(notNull)
					{
						dictionary.Add(_keys[i], _values[i]);
					}
				}
			}
			catch(Exception e)
			{
				//e.LogException();
				SerializationException = e;
				dictionary = null;
			}
		}

#if UNITY_EDITOR
		public virtual void Editor_Add(Type valueType)
		{
			OnBeforeSerialize();
			K key = _keys.Count > 0 ? _keys.Last() : (K)Editor_CreateInstance(typeof(K));
			_keys.Add(Editor_GetUniqueKey(key));
			_values.Add((V)Editor_CreateInstance(valueType));
			OnAfterDeserialize();
		}

		public virtual void Editor_Remove(int index)
		{
			OnBeforeSerialize();
			_keys.RemoveAt(index);
			_values.RemoveAt(index);
			OnAfterDeserialize();
		}

		public virtual void Editor_Swap(int from, int to)
		{
			OnBeforeSerialize();
			(_keys[to], _keys[from]) = (_keys[from], _keys[to]);
			(_values[to], _values[from]) = (_values[from], _values[to]);
			OnAfterDeserialize();
		}

		public virtual void Editor_ReplaceValue(object key, object newValue)
		{
			OnBeforeSerialize();
			int index = _keys.IndexOf((K)key);
			if(index >= 0)
			{
				_values[index] = (V)newValue;
			}
			OnAfterDeserialize();
		}

		private K Editor_GetUniqueKey(K start)
		{
			if(start is string s)
			{
				return (K)(object)UnityEditor.ObjectNames.GetUniqueName((string[])(object)_keys.ToArray(), s);
			}
			else if(start is int i)
			{
				var ints = (List<int>)(object)_keys;
				while(ints.Contains(i)) i++;
				return (K)(object)i;
			}
			else if(start is float f)
			{
				var floats = (List<float>)(object)_keys;
				while(floats.Contains(f)) f++;
				return (K)(object)f;
			}
			else
			{
				//Unable to create unique key for this type
				return start;
			}
		}

		private object Editor_CreateInstance(Type type)
		{
			if(type == typeof(string)) return "";
			else if(typeof(UnityEngine.Object).IsAssignableFrom(type)) return null;
			else return Activator.CreateInstance(type);
		}
#endif
	}
}
