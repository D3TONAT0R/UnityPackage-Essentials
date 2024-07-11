using System;
using System.Reflection;
using UnityEditor;

namespace UnityEssentialsEditor.Inspector
{
	public abstract class ExtendedEditor : Editor
	{
		protected Editor defaultEditor;

		protected abstract Type BaseType { get; }

		protected virtual void OnEnable()
		{
			//When this inspector is created, also create the built-in inspector
			defaultEditor = CreateEditor(targets, BaseType);
		}

		protected virtual void OnDisable()
		{
			//When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
			//Also, make sure to call any required methods like OnDisable
			var disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if(disableMethod != null) disableMethod.Invoke(defaultEditor, null);
			DestroyImmediate(defaultEditor);
		}

		protected virtual void OnBeforeDefaultInspectorGUI()
		{

		}

		public sealed override void OnInspectorGUI()
		{
			OnBeforeDefaultInspectorGUI();
			defaultEditor.OnInspectorGUI();
			OnAfterDefaultInspectorGUI();
		}

		protected virtual void OnAfterDefaultInspectorGUI()
		{

		}


		protected virtual void OnSceneGUI()
		{
			var m = defaultEditor.GetType().GetMethod("OnSceneGUI", BindingFlags.Public | BindingFlags.Instance);
			if(m == null) m = defaultEditor.GetType().GetMethod("OnSceneGUI", BindingFlags.NonPublic | BindingFlags.Instance);
			if(m != null)
			{
				m.Invoke(defaultEditor, new object[0]);
			}
		}
	}
}
