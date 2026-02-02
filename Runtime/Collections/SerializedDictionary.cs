using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials.Collections
{
	/// <summary>
	/// Interface for a dictionary that can be serialized by unity.
	/// </summary>
	public interface ISerializedDictionary
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

		Exception SerializationException { get; }
		
		bool UseMonospaceKeyLabels { get; }

		/// <summary>
		/// Clears the dictionary.
		/// </summary>
		void Clear();
	}

	/// <summary>
	/// Interface for a value type in a UnityDictionary. Can be used for polymorphic support of dictionaries.
	/// </summary>
	public interface ISerializedDictionaryValue { }

	/// <summary>
	/// A value inside a UnityDictionary. Can be used for polymorphic support of dictionaries.
	/// </summary>
	[Serializable]
	public abstract class ISerializedDictionaryValue<T> : ISerializedDictionaryValue
	{
		public T value;

		public static implicit operator T(ISerializedDictionaryValue<T> c) => c.value;
	}

	[Serializable] public class BoolValue : ISerializedDictionaryValue<bool> { }
	[Serializable] public class IntValue : ISerializedDictionaryValue<int> { }
	[Serializable] public class FloatValue : ISerializedDictionaryValue<float> { }
	[Serializable] public class StringValue : ISerializedDictionaryValue<string> { }
	[Serializable] public class Vector2Value : ISerializedDictionaryValue<Vector2> { }
	[Serializable] public class Vector3Value : ISerializedDictionaryValue<Vector3> { }
	[Serializable] public class Vector4Value : ISerializedDictionaryValue<Vector4> { }

	/// <summary>
	/// A wrapper for .NETs <see cref="Dictionary{TKey, TValue}"/> that supports serialization in Unity.
	/// </summary>
	/// <typeparam name="K">The key type for the dictionary.</typeparam>
	/// <typeparam name="V">The value type for the dictionary.</typeparam>
	[Serializable]
	public class SerializedDictionary<K, V> : ISerializedDictionary, ISerializationCallbackReceiver, IEnumerable<KeyValuePair<K, V>>
	{
		protected Dictionary<K, V> dictionary = new Dictionary<K, V>();

		[SerializeField]
		protected List<K> serializedKeys = new List<K>();
		[SerializeField, SerializeReference]
		protected List<V> serializedValues = new List<V>();

		public Type KeyType => typeof(K);
		public Type ValueType => typeof(V);
		public virtual Dictionary<K, V>.KeyCollection Keys => dictionary?.Keys;
		public virtual Dictionary<K, V>.ValueCollection Values => dictionary?.Values;
		public virtual int Count => dictionary?.Count ?? -1;

		public Exception SerializationException { get; private set; }
		public bool Valid => SerializationException == null;

		public virtual bool UseMonospaceKeyLabels => true;

		public SerializedDictionary()
		{

		}

		public SerializedDictionary(IDictionary<K, V> dictionary)
		{
			this.dictionary = new Dictionary<K, V>(dictionary);
		}

		public SerializedDictionary(Dictionary<K, V> dictionary)
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
			if(dictionary != null)
			{
				serializedKeys.Clear();
				serializedValues.Clear();
				foreach(var kv in dictionary)
				{
					serializedKeys.Add(kv.Key);
					serializedValues.Add(kv.Value);
				}
			}

		}

		public void OnAfterDeserialize()
		{
			SerializationException = null;
			try
			{
				if(dictionary == null) dictionary = new Dictionary<K, V>();
				dictionary.Clear();
				for(int i = 0; i < serializedKeys.Count; i++)
				{
					bool notNull;
					if(typeof(UnityEngine.Object).IsAssignableFrom(KeyType))
					{
						notNull = (serializedKeys[i] as UnityEngine.Object) != null;
					}
					else
					{
						notNull = serializedKeys[i] != null;
					}
					if(notNull)
					{
						if(!dictionary.ContainsKey(serializedKeys[i]))
						{
							dictionary.Add(serializedKeys[i], serializedValues[i]);
						}
						else
						{
							if(typeof(UnityEngine.Object).IsAssignableFrom(typeof(K)))
							{
								//Avoid using ToString to prevent an exception
								throw new ArgumentException($"Key has already been added to the dictionary (index {i})");
							}
							else
							{
								throw new ArgumentException($"Key '{serializedKeys[i]}' has already been added to the dictionary (index {i})");
							}
						}
					}
					else
					{
						throw new ArgumentException($"Key cannot be null (index {i})");
					}
				}
			}
			catch(Exception e)
			{
#if UNITY_EDITOR
				UnityEditor.EditorApplication.delayCall += () =>
				{
					if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
					{
						e.LogException("Exception thrown while deserializing dictionary");
					}
				};
#else
				e.LogException("Exception thrown while deserializing dictionary");
#endif
				SerializationException = e;
				dictionary = null;
			}
		}
	}
}
