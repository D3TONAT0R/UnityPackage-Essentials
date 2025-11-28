using UnityEngine;

namespace UnityEssentials.Collections
{
	/// <summary>
	/// Base class for elements that can be added to a Polymorhic Stack.
	/// </summary>
	[System.Serializable]
	public abstract class StackComponent
	{
		public virtual string HeaderTitle
		{
			get
			{
				if(!string.IsNullOrEmpty(CustomName))
				{
					return $"{CustomName} ({ReadableTypeName})";
				}
				else
				{
					return ReadableTypeName;
				}
			}
		}

		public virtual string CustomName { get; set; } = null;

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
