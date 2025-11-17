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

		private const int TOOLBAR_BUTTON_WIDTH = 25;

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
			if(_defaultEditor == null) return;
			var disableMethod = _defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			try
			{
				Debug.unityLogger.logEnabled = false;
				if(disableMethod != null) disableMethod.Invoke(_defaultEditor, null);
				DestroyImmediate(_defaultEditor);
			}
			catch { }
			finally
			{
				Debug.unityLogger.logEnabled = true;
			}
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
			var extraPropsSetting = EssentialsProjectSettings.Instance.extraProperties;
			if(extraPropsSetting != EssentialsProjectSettings.InspectorMode.Disabled)
			{
				DrawExtraProperties(extraPropsSetting == EssentialsProjectSettings.InspectorMode.Foldout);
			}
			var toolbarSetting = EssentialsProjectSettings.Instance.toolbar;
			if(toolbarSetting != EssentialsProjectSettings.InspectorMode.Disabled)
			{
				DrawToolbars(toolbarSetting == EssentialsProjectSettings.InspectorMode.Foldout);
			}
		}

		private void DrawExtraProperties(bool foldout)
		{
			GUILayout.Space(5);
			if(!foldout || Foldout("Extra Properties", ref expandExtraProperties, "TransformExtraPropertiesExpanded"))
			{
				if(targets.Length == 1)
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
				else
				{
					GUILayout.Label("Multi-object editing is not supported.", EditorStyles.centeredGreyMiniLabel);
				}
			}
		}

		private void DrawToolbars(bool foldout)
		{
			GUILayout.Space(5);
			if(!foldout || Foldout("Tools", ref expandExtraTools, "TransformToolbarExpanded"))
			{
				DrawPrimaryToolbar();
				DrawSecondaryToolbar();
			}
		}

		private void DrawPrimaryToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			TriButtonRow(new GUIContent("Reset"), TOOLBAR_BUTTON_WIDTH * 2,
				t =>
				{
					Undo.RecordObject(t, "Reset Transform");
					t.localPosition = Vector3.zero;
					t.localRotation = Quaternion.identity;
					t.localScale = Vector3.one;
				},
				t =>
				{
					Undo.RecordObject(t, "Reset Position");
					t.localPosition = Vector3.zero;
				},
				t =>
				{
					Undo.RecordObject(t, "Reset Rotation");
					t.localRotation = Quaternion.identity;
				},
				t =>
				{
					Undo.RecordObject(t, "Reset Scale");
					t.localScale = Vector3.one;
				}
			);

			GUI.enabled = _transform.childCount > 0;
			var rect = GUILayoutUtility.GetRect(TOOLBAR_BUTTON_WIDTH * 2, 0, GUILayout.ExpandHeight(true));
			if(EditorGUI.DropdownButton(rect, new GUIContent("Apply"), FocusType.Passive, EditorStyles.toolbarDropDown))
			{
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("Position"), false, () => ExtraContextMenuItems.ApplyPosition(new MenuCommand(_transform)));
				menu.AddItem(new GUIContent("Rotation"), false, () => ExtraContextMenuItems.ApplyRotation(new MenuCommand(_transform)));
				menu.AddItem(new GUIContent("Scale"), false, () => ExtraContextMenuItems.ApplyScale(new MenuCommand(_transform)));
				menu.AddItem(new GUIContent("Full Transform"), false, () => ExtraContextMenuItems.ApplyFullTransform(new MenuCommand(_transform)));
				menu.DropDown(rect);
			}
			GUI.enabled = true;

			GUILayout.FlexibleSpace();

			//Copy transform data
			GUI.enabled = targets.Length == 1;
			TriButtonRow(new GUIContent("Copy"), TOOLBAR_BUTTON_WIDTH * 2,
				t =>
				{
					//Copy all transform data
					copiedPosition = _transform.position;
					copiedRotation = _transform.rotation;
					copiedScale = _transform.localScale;
				},
				t =>
				{
					//Copy position
					copiedPosition = _transform.position;
					copiedRotation = null;
					copiedScale = null;
				},
				t =>
				{
					//Copy rotation
					copiedPosition = null;
					copiedRotation = _transform.rotation;
					copiedScale = null;
				},
				t =>
				{
					//Copy scale
					copiedPosition = null;
					copiedRotation = null;
					copiedScale = _transform.localScale;
				}
			);
			GUI.enabled = true;

			EditorGUILayout.EndHorizontal();
		}

		private void DrawSecondaryToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar);

			TriButtonRow(new GUIContent("Align"), TOOLBAR_BUTTON_WIDTH * 2,
				t =>
				{
					Undo.RecordObject(t, "Align Transform to View");
					t.position = SceneView.lastActiveSceneView.camera.transform.position;
					t.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
				},
				t =>
				{
					Undo.RecordObject(t, "Align Position to View");
					t.position = SceneView.lastActiveSceneView.camera.transform.position;
				},
				t =>
				{
					Undo.RecordObject(t, "Align Rotation to View");
					t.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
				},
				null
			);

			GUILayout.FlexibleSpace();

			//Paste transform data
			TriButtonRow(new GUIContent("Paste"), TOOLBAR_BUTTON_WIDTH * 2,
				t =>
				{
					Undo.RecordObject(t, "Paste Transform Values");
					if(copiedPosition.HasValue) t.position = copiedPosition.Value;
					if(copiedRotation.HasValue) t.rotation = copiedRotation.Value;
					if(copiedScale.HasValue) t.localScale = copiedScale.Value;
				},
				t =>
				{
					if(copiedPosition.HasValue)
					{
						Undo.RecordObject(t, "Paste Position");
						t.position = copiedPosition.Value;
					}
				},
				t =>
				{
					if(copiedRotation.HasValue)
					{
						Undo.RecordObject(t, "Paste Rotation");
						t.rotation = copiedRotation.Value;
					}
				},
				t =>
				{
					if(copiedScale.HasValue)
					{
						Undo.RecordObject(t, "Paste Scale");
						t.localScale = copiedScale.Value;
					}
				},
				() => copiedPosition.HasValue && copiedRotation.HasValue && copiedScale.HasValue,
				() => copiedPosition.HasValue,
				() => copiedRotation.HasValue,
				() => copiedScale.HasValue
			);

			GUILayout.EndHorizontal();
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


		private void TriButtonRow(GUIContent mainContent, int mainWidth,
			Action<Transform> mainButton, Action<Transform> posButton, Action<Transform> rotButton, Action<Transform> scaleButton,
			Func<bool> mainEnabledFunc = null, Func<bool> posEnabledFunc = null, Func<bool> rotEnabledFunc = null, Func<bool> scaleEnabledFunc = null)
		{
			bool enabled = GUI.enabled;
			GUI.enabled = enabled && (mainEnabledFunc?.Invoke() ?? true);
			if(mainButton != null && GUILayout.Button(mainContent, EditorStyles.toolbarButton, GUILayout.Width(mainWidth))) ForEachTransform(mainButton);
			GUI.enabled = enabled && (posEnabledFunc?.Invoke() ?? true);
			if(posButton != null && GUILayout.Button(positionIcon, EditorStyles.toolbarButton, GUILayout.Width(TOOLBAR_BUTTON_WIDTH))) ForEachTransform(posButton);
			GUI.enabled = enabled && (rotEnabledFunc?.Invoke() ?? true);
			if(rotButton != null && GUILayout.Button(rotationIcon, EditorStyles.toolbarButton, GUILayout.Width(TOOLBAR_BUTTON_WIDTH))) ForEachTransform(rotButton);
			GUI.enabled = enabled && (scaleEnabledFunc?.Invoke() ?? true);
			if(scaleButton != null && GUILayout.Button(scaleIcon, EditorStyles.toolbarButton, GUILayout.Width(TOOLBAR_BUTTON_WIDTH))) ForEachTransform(scaleButton);
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

		private void ForEachTransform(Action<Transform> action)
		{
			foreach(var t in targets)
			{
				if(t is Transform transform)
				{
					action.Invoke(transform);
				}
			}
		}
	}
}