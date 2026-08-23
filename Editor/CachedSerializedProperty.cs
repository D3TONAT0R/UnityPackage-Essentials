using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public struct CachedSerializedProperty
	{
		private enum LookupSource
		{
			None,
			SerializedPropertyParent,
			SerializedObjectParent
		}

		public SerializedProperty Property { get; private set; }

		public bool Resolved { get; private set; }

		private LookupSource cachedSource;
		private SerializedProperty cachedPropertyParent;
		private SerializedObject cachedObjectParent;
		private string cachedPath;

		private bool Matches(SerializedProperty parent, string path)
		{
			return Resolved
				&& cachedSource == LookupSource.SerializedPropertyParent
				&& ReferenceEquals(cachedPropertyParent, parent)
				&& cachedPath == path
				&& (Property == null || Property.serializedObject == parent.serializedObject);
		}

		private bool Matches(SerializedProperty parent)
		{
			return Resolved
				&& cachedSource == LookupSource.SerializedPropertyParent
				&& ReferenceEquals(cachedPropertyParent, parent)
				&& cachedPath == null
				&& (Property == null || Property.serializedObject == parent.serializedObject);
		}

		private bool Matches(SerializedObject parent, string path)
		{
			return Resolved
				&& cachedSource == LookupSource.SerializedObjectParent
				&& ReferenceEquals(cachedObjectParent, parent)
				&& cachedPath == path
				&& (Property == null || Property.serializedObject == parent);
		}

		private bool Matches(SerializedObject parent)
		{
			return Resolved
				&& cachedSource == LookupSource.SerializedObjectParent
				&& ReferenceEquals(cachedObjectParent, parent)
				&& cachedPath == null
				&& (Property == null || Property.serializedObject == parent);
		}

		private void Cache(SerializedProperty parent, string path)
		{
			cachedSource = LookupSource.SerializedPropertyParent;
			cachedPropertyParent = parent;
			cachedObjectParent = null;
			cachedPath = path;
			Resolved = true;
		}

		private void Cache(SerializedObject parent, string path)
		{
			cachedSource = LookupSource.SerializedObjectParent;
			cachedPropertyParent = null;
			cachedObjectParent = parent;
			cachedPath = path;
			Resolved = true;
		}

		public SerializedProperty Find(SerializedProperty parent, string path)
		{
			if (!Matches(parent, path))
			{
				Property = parent.FindPropertyRelative(path);
				if (Property == null)
				{
					Debug.LogError($"Property '{path}' not found in '{parent.propertyPath}'");
				}
				Cache(parent, path);
			}
			return Property;
		}

		public SerializedProperty Get(SerializedProperty parent, Func<SerializedProperty, SerializedProperty> getter)
		{
			if (!Matches(parent))
			{
				Property = getter(parent);
				if (Property == null)
				{
					Debug.LogError($"Property not found in '{parent.propertyPath}'.");
				}
				Cache(parent, null);
			}
			return Property;
		}

		public SerializedProperty Find(SerializedObject parent, string path)
		{
			if (!Matches(parent, path))
			{
				Property = parent.FindProperty(path);
				if (Property == null)
				{
					Debug.LogError($"Property '{path}' not found in '{parent.targetObject.GetType().Name}'");
				}
				Cache(parent, path);
			}
			return Property;
		}

		public SerializedProperty Get(SerializedObject parent, Func<SerializedObject, SerializedProperty> getter)
		{
			if (!Matches(parent))
			{
				Property = getter(parent);
				if (Property == null)
				{
					Debug.LogError($"Property not found in '{parent.targetObject.GetType().Name}'.");
				}
				Cache(parent, null);
			}
			return Property;
		}

		public void ClearCache()
		{
			cachedSource = LookupSource.None;
			cachedPropertyParent = null;
			cachedObjectParent = null;
			cachedPath = null;
			Property = null;
			Resolved = false;
		}
	}
}
