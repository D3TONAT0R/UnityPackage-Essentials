using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public struct CachedSerializedProperty
	{
		public SerializedProperty Property { get; private set; }
		
		public bool Resolved { get; private set; }

		public SerializedProperty Find(SerializedProperty parent, string path)
		{
			if (!Resolved || Property == null)
			{
				Property = parent.FindPropertyRelative(path);
				if (Property == null)
				{
					Debug.LogError($"Property '{path}' not found in '{parent.propertyPath}'");
				}
				Resolved = true;
			}
			return Property;
		}

		public SerializedProperty Get(SerializedProperty parent, Func<SerializedProperty, SerializedProperty> getter)
		{
			if (!Resolved || Property == null)
			{
				Property = getter(parent);
				if (Property == null)
				{
					Debug.LogError($"Property not found in '{parent.propertyPath}'.");
				}
				Resolved = true;
			}
			return Property;
		}

		public SerializedProperty Find(SerializedObject parent, string path)
		{
			if (!Resolved || Property == null)
			{
				Property = parent.FindProperty(path);
				if (Property == null)
				{
					Debug.LogError($"Property '{path}' not found in '{parent.targetObject.GetType().Name}'");
				}
				Resolved = true;
			}
			return Property;
		}

		public SerializedProperty Get(SerializedObject parent, Func<SerializedObject, SerializedProperty> getter)
		{
			if (!Resolved || Property == null)
			{
				Property = getter(parent);
				if (Property == null)
				{
					Debug.LogError($"Property not found in '{parent.targetObject.GetType().Name}'.");
				}
				Resolved = true;
			}
			return Property;
		}
		
		public void ClearCache()
		{
			Property = null;
			Resolved = false;
		}
	}
}
