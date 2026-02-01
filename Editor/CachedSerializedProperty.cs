using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public struct CachedSerializedProperty
	{
		private SerializedProperty property;

		public SerializedProperty Property => property;

		public SerializedProperty Find(SerializedProperty parent, string path)
		{
			if (property == null)
			{
				property = parent.FindPropertyRelative(path);
				if (property == null)
				{
					Debug.LogError($"Property '{path}' not found in '{parent.propertyPath}'");
				}
			}
			return property;
		}

		public SerializedProperty Find(SerializedObject parent, string path)
		{
			if (property == null)
			{
				property = parent.FindProperty(path);
				if (property == null)
				{
					Debug.LogError($"Property '{path}' not found in '{parent.targetObject.GetType().Name}'");
				}
			}
			return property;
		}
		
		public void ClearCache()
		{
			property = null;
		}
	}
}
