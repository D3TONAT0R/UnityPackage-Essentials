using UnityEngine;

namespace UnityEssentials.Collections
{
	[System.Serializable]
	public abstract class StackElement
	{
		public virtual string HeaderTitle
		{
			get
			{
				string custom = null;
				if(this is ICustomElementNameProvider c)
				{
					if(!string.IsNullOrWhiteSpace(c.CustomName)) custom = c.CustomName;
				}
				if(custom != null)
				{
					return $"{custom} ({ReadableTypeName})";
				}
				else
				{
					return ReadableTypeName;
				}
			}
		}

		public string ReadableTypeName
		{
			get
			{
#if UNITY_EDITOR
				return UnityEditor.ObjectNames.NicifyVariableName(GetType().Name);
#else
				return GetType().Name;
#endif
			}
		}

		[HideInInspector]
		public MonoBehaviour hostComponent;

		public void InvokeStart(MonoBehaviour host)
		{
			hostComponent = host;
		}

		protected virtual void Start()
		{

		}

		public virtual void Update()
		{

		}

		public virtual void OnDrawGizmos(MonoBehaviour parent, bool selected)
		{
			Gizmos.color = Color.white;
			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}
