using System.Reflection;
using UnityEditor;

namespace UnityEssentialsEditor
{
	public struct CachedAttribute<T> where T : System.Attribute
	{
		public T Attribute { get; private set; }

		public bool Resolved { get; private set; }
		
		public T Get(MemberInfo m, bool inherit = true)
		{
			if (!Resolved)
			{
				Attribute = m.GetCustomAttribute<T>(inherit);
				Resolved = true;
			}
			return Attribute;
		}
		
		public bool TryGet(MemberInfo m, out T attribute, bool inherit = true)
		{
			attribute = Get(m, inherit);
			return attribute != null;
		}

		public T Get(SerializedProperty prop, bool inherit = true)
		{
			if (!Resolved)
			{
				Attribute = PropertyDrawerUtility.GetAttribute<T>(prop, inherit);
				Resolved = true;
			}
			return Attribute;
		}
		
		public bool TryGet(SerializedProperty prop, out T attribute, bool inherit = true)
		{
			attribute = Get(prop, inherit);
			return attribute != null;
		}
	}
}