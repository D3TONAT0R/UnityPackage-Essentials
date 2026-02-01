using UnityEditor;

namespace UnityEssentialsEditor
{
	public struct CachedObject<T>
	{
		public T Value { get; private set; }
		
		public bool Resolved { get; private set; }
		
		public T Get(SerializedProperty property)
		{
			if (!Resolved)
			{
				Value = PropertyDrawerUtility.GetPropertyValue<T>(property);
				Resolved = true;
			}
			return Value;
		}
		
		public void ClearCache()
		{
			Resolved = false;
			Value = default;
		}
	}
}