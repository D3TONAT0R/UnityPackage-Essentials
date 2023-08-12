using D3T;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	[CustomEditor(typeof(Transform))]
	[CanEditMultipleObjects]
	public class TransformEditor : Editor
	{

		private Editor _defaultEditor;
		private Transform _transform;

		static bool foldoutExtraInfo = false;

		private GUIStyle pathLabelStyle;

		private void OnEnable()
		{
			//When this inspector is created, also create the built-in inspector
			_defaultEditor = CreateEditor(targets, System.Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
			_transform = target as Transform;
		}

		private void OnDisable()
		{
			//When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
			//Also, make sure to call any required methods like OnDisable
			var disableMethod = _defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			try
			{
				if (disableMethod != null) disableMethod.Invoke(_defaultEditor, null);
				DestroyImmediate(_defaultEditor);
			}
			catch { }
		}

		public override void OnInspectorGUI()
		{
			if (pathLabelStyle == null)
			{
				pathLabelStyle = new GUIStyle(EditorStyles.miniLabel)
				{
					wordWrap = true,
				};
			}
			_defaultEditor.OnInspectorGUI();
			foldoutExtraInfo = EditorGUILayout.Foldout(foldoutExtraInfo, "Additional Info");
			if (foldoutExtraInfo)
			{
				EditorGUI.BeginChangeCheck();
				var worldPos = EditorGUILayout.Vector3Field("Position (world)", _transform.position);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(_transform, "Move (world)");
					_transform.position = worldPos;
				}
				EditorGUI.BeginChangeCheck();
				var worldEuler = EditorGUILayout.Vector3Field("Rotation (world)", _transform.eulerAngles);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(_transform, "Rotate (world)");
					_transform.eulerAngles = worldEuler;
				}
				GUI.backgroundColor = Color.white.SetAlpha(0.5f);
				EditorGUILayout.Vector3Field("Scale (lossy)", _transform.lossyScale);
				EditorGUILayout.Space();
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Vector3Field("Up Direction", _transform.up);
				EditorGUILayout.Vector3Field("Forward Direction", _transform.forward);
				GUI.backgroundColor = Color.white;
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Child Count (recursive)", $"{_transform.childCount} ({RecursiveChildCount(_transform)})");
				EditorGUILayout.LabelField("Parent Depth", RecursiveParentCount(_transform).ToString());
				GUILayout.BeginHorizontal();
				GUILayout.Label("Hierarchy Path", GUILayout.Width(EditorGUIUtility.labelWidth - 3));

				GUILayout.Label(_transform.GetHierarchyPathString(), pathLabelStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 30));
				GUILayout.EndHorizontal();
			}
		}

		public override string GetInfoString()
		{
			return base.GetInfoString();
			//return GameObjectEditor.GetTransformPathString(_transform);
		}

		private int RecursiveChildCount(Transform transform)
		{
			int count = transform.childCount;
			for (int i = 0; i < transform.childCount; i++)
			{
				count += RecursiveChildCount(transform.GetChild(i));
			}
			return count;
		}

		private int RecursiveParentCount(Transform transform)
		{
			int count = 0;
			if (transform.parent)
			{
				count += RecursiveParentCount(transform.parent) + 1;
			}
			return count;
		}
	} 
}