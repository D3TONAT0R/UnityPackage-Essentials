using UnityEssentials;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using System;

namespace UnityEssentialsEditor
{
	[CustomEditor(typeof(Transform))]
	[CanEditMultipleObjects]
	public class ExtendedTransformEditor : Editor
	{

		private Editor _defaultEditor;
		private Transform _transform;
		private static bool expandExtraProperties = false;
		private static bool expandExtraTools = false;

		private static GUIStyle pathLabelStyle;
		private static Texture2D positionIcon;
		private static Texture2D rotationIcon;
		private static Texture2D scaleIcon;

		private static Vector3? copiedPosition;
		private static Quaternion? copiedRotation;
		private static Vector3? copiedScale;

		private void OnEnable()
		{
			//When this inspector is created, also create the built-in inspector
			_defaultEditor = CreateEditor(targets, System.Type.GetType("UnityEditor.TransformInspector, UnityEditor"));
			_transform = target as Transform;
			expandExtraProperties = EditorPrefs.GetBool("TransformExtraPropertiesExpanded", false);
			expandExtraTools = EditorPrefs.GetBool("TransformToolbarExpanded", false);
		}

		private void OnDisable()
		{
			//When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
			//Also, make sure to call any required methods like OnDisable
			var disableMethod = _defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			try
			{
				if(disableMethod != null) disableMethod.Invoke(_defaultEditor, null);
				DestroyImmediate(_defaultEditor);
			}
			catch { }
		}

		public override void OnInspectorGUI()
		{
			if(pathLabelStyle == null)
			{
				pathLabelStyle = new GUIStyle(EditorStyles.miniLabel)
				{
					wordWrap = true,
				};
				positionIcon = EditorGUIUtility.FindTexture("d_MoveTool");
				rotationIcon = EditorGUIUtility.FindTexture("d_RotateTool");
				scaleIcon = EditorGUIUtility.FindTexture("d_ScaleTool");
			}
			_defaultEditor.OnInspectorGUI();
			DrawExtraProperties();
			DrawToolbars();
		}

		private void DrawExtraProperties()
		{
			if(Foldout("Extra Properties", ref expandExtraProperties, "TransformExtraPropertiesExpanded"))
			{
				EditorGUI.BeginChangeCheck();
				var worldPos = EditorGUILayout.Vector3Field("Position (world)", _transform.position);
				if(EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(_transform, "Move (world)");
					_transform.position = worldPos;
				}
				EditorGUI.BeginChangeCheck();
				var worldEuler = EditorGUILayout.Vector3Field("Rotation (world)", _transform.eulerAngles);
				if(EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(_transform, "Rotate (world)");
					_transform.eulerAngles = worldEuler;
				}
				GUI.backgroundColor = Color.white.WithAlpha(0.5f);
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

		private void DrawToolbars()
		{
			if(Foldout("Tools", ref expandExtraTools, "TransformToolbarExpanded"))
			{
				DrawPrimaryToolbar();
				DrawSecondaryToolbar();
			}
		}

		private void DrawPrimaryToolbar()
		{
			EditorGUILayout.BeginHorizontal();
			TriButtonRow(new GUIContent("Reset"), 50,
				() =>
				{
					Undo.RecordObject(_transform, "Reset Transform");
					_transform.localPosition = Vector3.zero;
					_transform.localRotation = Quaternion.identity;
					_transform.localScale = Vector3.one;
				},
				() =>
				{
					Undo.RecordObject(_transform, "Reset Position");
					_transform.localPosition = Vector3.zero;
				},
				() =>
				{
					Undo.RecordObject(_transform, "Reset Rotation");
					_transform.localRotation = Quaternion.identity;
				},
				() =>
				{
					Undo.RecordObject(_transform, "Reset Scale");
					_transform.localScale = Vector3.one;
				}
			);

			GUILayout.FlexibleSpace();
			GUILayout.FlexibleSpace();
			GUILayout.FlexibleSpace();

			//Copy transform data
			TriButtonRow(new GUIContent("C"), 25,
				() =>
				{
					//Copy all transform data
					copiedPosition = _transform.position;
					copiedRotation = _transform.rotation;
					copiedScale = _transform.localScale;
				},
				() =>
				{
					//Copy position
					copiedPosition = _transform.position;
					copiedRotation = null;
					copiedScale = null;
				},
				() =>
				{
					//Copy rotation
					copiedPosition = null;
					copiedRotation = _transform.rotation;
					copiedScale = null;
				},
				() =>
				{
					//Copy scale
					copiedPosition = null;
					copiedRotation = null;
					copiedScale = _transform.localScale;
				}
			);

			GUILayout.FlexibleSpace();

			//Paste transform data
			TriButtonRow(new GUIContent("P"), 25,
				() =>
				{
					Undo.RecordObject(_transform, "Paste Transform Values");
					if(copiedPosition.HasValue) _transform.position = copiedPosition.Value;
					if(copiedRotation.HasValue) _transform.rotation = copiedRotation.Value;
					if(copiedScale.HasValue) _transform.localScale = copiedScale.Value;
				},
				() =>
				{
					if(copiedPosition.HasValue)
					{
						Undo.RecordObject(_transform, "Paste Position");
						_transform.position = copiedPosition.Value;
					}
				},
				() =>
				{
					if(copiedRotation.HasValue)
					{
						Undo.RecordObject(_transform, "Paste Rotation");
						_transform.rotation = copiedRotation.Value;
					}
				},
				() =>
				{
					if(copiedScale.HasValue)
					{
						Undo.RecordObject(_transform, "Paste Scale");
						_transform.localScale = copiedScale.Value;
					}
				},
				() => copiedPosition.HasValue && copiedRotation.HasValue && copiedScale.HasValue,
				() => copiedPosition.HasValue,
				() => copiedRotation.HasValue,
				() => copiedScale.HasValue
			);

			EditorGUILayout.EndHorizontal();
		}

		private bool Foldout(string label, ref bool state, string editorPref)
		{
			var lastState = state;
			state = EditorGUILayout.Foldout(state, label, true);
			if(state != lastState && !string.IsNullOrWhiteSpace(editorPref))
			{
				EditorPrefs.SetBool(editorPref, state);
			}
			return state;
		}

		private void DrawSecondaryToolbar()
		{
			GUILayout.BeginHorizontal();

			TriButtonRow(new GUIContent("View Align"), 80,
				() =>
				{
					Undo.RecordObject(_transform, "Align Transform to View");
					_transform.position = SceneView.lastActiveSceneView.camera.transform.position;
					_transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
				},
				() =>
				{
					Undo.RecordObject(_transform, "Align Position to View");
					_transform.position = SceneView.lastActiveSceneView.camera.transform.position;
				},
				() =>
				{
					Undo.RecordObject(_transform, "Align Rotation to View");
					_transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
				},
				null
			);

			GUILayout.EndHorizontal();
		}

		private void TriButtonRow(GUIContent mainContent, int mainWidth,
			Action mainButton, Action posButton, Action rotButton, Action scaleButton,
			Func<bool> mainEnabledFunc = null, Func<bool> posEnabledFunc = null, Func<bool> rotEnabledFunc = null, Func<bool> scaleEnabledFunc = null)
		{
			bool enabled = GUI.enabled;
			GUI.enabled = enabled && (mainEnabledFunc?.Invoke() ?? true);
			if(mainButton != null && GUILayout.Button(mainContent, EditorStyles.miniButtonLeft, GUILayout.Width(mainWidth))) mainButton.Invoke();
			GUI.enabled = enabled && (posEnabledFunc?.Invoke() ?? true);
			if(posButton != null && GUILayout.Button(positionIcon, EditorStyles.miniButtonMid, GUILayout.Width(25))) posButton.Invoke();
			GUI.enabled = enabled && (rotEnabledFunc?.Invoke() ?? true);
			if(rotButton != null && GUILayout.Button(rotationIcon, EditorStyles.miniButtonMid, GUILayout.Width(25))) rotButton.Invoke();
			GUI.enabled = enabled && (scaleEnabledFunc?.Invoke() ?? true);
			if(scaleButton != null && GUILayout.Button(scaleIcon, EditorStyles.miniButtonRight, GUILayout.Width(25))) scaleButton.Invoke();
		}

		private int RecursiveChildCount(Transform transform)
		{
			int count = transform.childCount;
			for(int i = 0; i < transform.childCount; i++)
			{
				count += RecursiveChildCount(transform.GetChild(i));
			}
			return count;
		}

		private int RecursiveParentCount(Transform transform)
		{
			int count = 0;
			if(transform.parent)
			{
				count += RecursiveParentCount(transform.parent) + 1;
			}
			return count;
		}
	}
}