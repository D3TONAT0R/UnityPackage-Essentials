using UnityEssentials;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace UnityEssentialsEditor
{
	[CustomEditor(typeof(Transform))]
	[CanEditMultipleObjects]
	public class ExtendedTransformEditor : Editor
	{
		public struct TransformGUIContext
		{
			public readonly SerializedObject serializedObject;
			public readonly Object[] targets;
			
			public bool HasMultipleTargets => targets.Length > 1;

			public TransformGUIContext(SerializedObject serializedObject, Object[] targets)
			{
				this.serializedObject = serializedObject;
				this.targets = targets;
			}
		}
		
		/// <summary>
		/// Subscribe to this event	to draw GUI before the built-in inspector.
		/// </summary>
		public static event Action<TransformGUIContext> BeforeBuiltinGUI;
		/// <summary>
		/// Subscribe to this event	to draw GUI after the built-in inspector.
		/// </summary>
		public static event Action<TransformGUIContext> AfterBuiltinGUI;
		/// <summary>
		/// Subscribe to this event to draw extra properties in the extra properties section.
		/// </summary>
		public static event Action<TransformGUIContext> DrawExtraPropertiesGUI;
		/// <summary>
		/// Subscribe to this event to draw GUI in the toolbar section.
		/// </summary>
		public static event Action<TransformGUIContext> DrawToolbarGUI;
		/// <summary>
		/// Subscribe to this event to draw GUI after the extra properties provided by this package.
		/// </summary>
		public static event Action<TransformGUIContext> AfterExtrasGUI;
		
		private const int TOOLBAR_BUTTON_WIDTH = 25;

		private Editor _defaultEditor;
		private Transform _transform;

		private static bool expandExtraProperties = false;
		private static bool expandExtraTools = false;

		private static GUIStyle pathLabelStyle;
		private static GUIContent positionIcon;
		private static GUIContent rotationIcon;
		private static GUIContent scaleIcon;
		private static GUIContent globalIcon;
		private static GUIContent localIcon;

		private static Space currentSpace = Space.Self;

		private static Space copiedSpace = Space.Self;
		private static Vector3? copiedPosition;
		private static Quaternion? copiedRotation;
		private static Vector3? copiedScale;

		private static readonly GUIContent reset = new GUIContent("Reset", "Reset Transform");
		private static readonly GUIContent apply = new GUIContent("Apply", "Apply Transform to Children");
		private static readonly GUIContent position = new GUIContent("Position");
		private static readonly GUIContent rotation = new GUIContent("Rotation");
		private static readonly GUIContent scale = new GUIContent("Scale");
		private static readonly GUIContent fullTransform = new GUIContent("Full Transform");
		private static readonly GUIContent worldPosition = new GUIContent("Position (world)");
		private static readonly GUIContent worldRotation = new GUIContent("Rotation (world)");
		private static readonly GUIContent lossyScale = new GUIContent("Scale (lossy)");
		private static readonly GUIContent upDirection = new GUIContent("Up Direction");
		private static readonly GUIContent forwardDirection = new GUIContent("Forward Direction");
		private static readonly GUIContent recursiveChildCount = new GUIContent("Child Count (recursive)");
		private static readonly GUIContent parentDepth = new GUIContent("Parent Depth");
		private static readonly GUIContent copy = new GUIContent("Copy", "Copy Transform Values using the current space");
		private static readonly GUIContent align = new GUIContent("Align", "Align Transform to Scene View");
		private static readonly GUIContent paste = new GUIContent("Paste", "Paste Transform Values using the copied space");
		private static readonly GUIContent hierarchyPath = new GUIContent("Hierarchy Path");
		private static GUIContent hierarchyPathString = new GUIContent("");
		private static GUIContent childCounter = new GUIContent("");
		private static GUIContent parentCounter = new GUIContent("");

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
			if (_defaultEditor == null) return;
			var disableMethod = _defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			try
			{
				Debug.unityLogger.logEnabled = false;
				if (disableMethod != null) disableMethod.Invoke(_defaultEditor, null);
				DestroyImmediate(_defaultEditor);
			}
			catch
			{
			}
			finally
			{
				Debug.unityLogger.logEnabled = true;
			}
		}

		public override void OnInspectorGUI()
		{
			if (pathLabelStyle == null)
			{
				pathLabelStyle = new GUIStyle(EditorStyles.miniLabel)
				{
					wordWrap = true,
				};
				positionIcon = new GUIContent(EditorGUIUtility.FindTexture("d_MoveTool"), "Position");
				rotationIcon = new GUIContent(EditorGUIUtility.FindTexture("d_RotateTool"), "Rotation");
				scaleIcon = new GUIContent(EditorGUIUtility.FindTexture("d_ScaleTool"), "Scale");
				globalIcon = new GUIContent(EditorGUIUtility.FindTexture("d_ToolHandleGlobal"), "Global Space");
				localIcon = new GUIContent(EditorGUIUtility.FindTexture("d_ToolHandleLocal"), "Local Space");
			}
			InvokeDrawEvents(BeforeBuiltinGUI);
			_defaultEditor.OnInspectorGUI();
			InvokeDrawEvents(AfterBuiltinGUI);
			var extraPropsSetting = EssentialsProjectSettings.Instance.extraProperties;
			if (extraPropsSetting != EssentialsProjectSettings.InspectorMode.Disabled)
			{
				DrawExtraProperties(extraPropsSetting == EssentialsProjectSettings.InspectorMode.Foldout);
			}
			var toolbarSetting = EssentialsProjectSettings.Instance.toolbar;
			if (toolbarSetting != EssentialsProjectSettings.InspectorMode.Disabled)
			{
				DrawToolbars(toolbarSetting == EssentialsProjectSettings.InspectorMode.Foldout);
			}
			InvokeDrawEvents(AfterExtrasGUI);
		}

		private void DrawExtraProperties(bool foldout)
		{
			GUILayout.Space(5);
			if (!foldout || Foldout("Extra Properties", ref expandExtraProperties, "TransformExtraPropertiesExpanded"))
			{
				if (targets.Length == 1)
				{
					EditorGUI.BeginChangeCheck();
					var worldPos = EditorGUILayout.Vector3Field(worldPosition, _transform.position);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(_transform, "Move (world)");
						_transform.position = worldPos;
					}
					EditorGUI.BeginChangeCheck();
					var worldEuler = EditorGUILayout.Vector3Field(worldRotation, _transform.eulerAngles);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(_transform, "Rotate (world)");
						_transform.eulerAngles = worldEuler;
					}
					GUI.backgroundColor = Color.white.WithAlpha(0.5f);
					EditorGUILayout.Vector3Field(lossyScale, _transform.lossyScale);
					EditorGUILayout.Space();
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.Vector3Field(upDirection, _transform.up);
					EditorGUILayout.Vector3Field(forwardDirection, _transform.forward);
					GUI.backgroundColor = Color.white;
					EditorGUILayout.Space();
					childCounter.text = $"{_transform.childCount} ({RecursiveChildCount(_transform)})";
					EditorGUILayout.LabelField(recursiveChildCount, childCounter);
					parentCounter.text = RecursiveParentCount(_transform).ToString();
					EditorGUILayout.LabelField(parentDepth, parentCounter);
					GUILayout.BeginHorizontal();
					GUILayout.Label(hierarchyPath, GUILayout.Width(EditorGUIUtility.labelWidth - 3));

					hierarchyPathString.text = _transform.GetHierarchyPathString();
					GUILayout.Label(hierarchyPathString, pathLabelStyle,
						GUILayout.Width(EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 30));
					GUILayout.EndHorizontal();
					
					InvokeDrawEvents(DrawExtraPropertiesGUI);
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
			if (!foldout || Foldout("Tools", ref expandExtraTools, "TransformToolbarExpanded"))
			{
				DrawPrimaryToolbar();
				DrawSecondaryToolbar();
				InvokeDrawEvents(DrawToolbarGUI);
			}
		}

		private void DrawPrimaryToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			TriButtonRow(reset, TOOLBAR_BUTTON_WIDTH * 2,
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
			if (EditorGUI.DropdownButton(rect, apply, FocusType.Passive, EditorStyles.toolbarDropDown))
			{
				var menu = new GenericMenu();
				menu.AddItem(position, false, () => ExtraContextMenuItems.ApplyPosition(new MenuCommand(_transform)));
				menu.AddItem(rotation, false, () => ExtraContextMenuItems.ApplyRotation(new MenuCommand(_transform)));
				menu.AddItem(scale, false, () => ExtraContextMenuItems.ApplyScale(new MenuCommand(_transform)));
				menu.AddItem(fullTransform, false, () => ExtraContextMenuItems.ApplyFullTransform(new MenuCommand(_transform)));
				menu.DropDown(rect);
			}
			GUI.enabled = true;

			GUILayout.FlexibleSpace();

			var icon = currentSpace == Space.Self ? localIcon : globalIcon;
			if (GUILayout.Button(icon, EditorStyles.toolbarButton, GUILayout.Width(TOOLBAR_BUTTON_WIDTH)))
			{
				currentSpace = currentSpace == Space.Self ? Space.World : Space.Self;
			}

			//Copy transform data
			GUI.enabled = targets.Length == 1;
			TriButtonRow(copy, TOOLBAR_BUTTON_WIDTH * 2,
				t =>
				{
					//Copy all transform data
					copiedSpace = currentSpace;
					if (currentSpace == Space.Self)
					{
						copiedPosition = t.localPosition;
						copiedRotation = t.localRotation;
						copiedScale = t.localScale;
					}
					else
					{
						copiedPosition = t.position;
						copiedRotation = t.rotation;
						copiedScale = t.localScale;
					}
				},
				t =>
				{
					//Copy position
					copiedSpace = currentSpace;
					copiedPosition = currentSpace == Space.Self ? t.localPosition : t.position;
					copiedRotation = null;
					copiedScale = null;
				},
				t =>
				{
					//Copy rotation
					copiedSpace = currentSpace;
					copiedPosition = null;
					copiedRotation = currentSpace == Space.Self ? t.localRotation : t.rotation;
					copiedScale = null;
				},
				t =>
				{
					//Copy scale
					copiedSpace = currentSpace;
					copiedPosition = null;
					copiedRotation = null;
					copiedScale = t.localScale;
				}
			);
			GUI.enabled = true;

			EditorGUILayout.EndHorizontal();
		}

		private void DrawSecondaryToolbar()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar);

			TriButtonRow(align, TOOLBAR_BUTTON_WIDTH * 2,
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

			if (copiedPosition.HasValue || copiedRotation.HasValue || copiedScale.HasValue)
			{
				GUI.color = Color.white.WithAlpha(0.5f);
				var icon = copiedSpace == Space.Self ? localIcon : globalIcon;
				GUILayout.Label(icon, EditorStyles.centeredGreyMiniLabel);
				GUI.color = Color.white;
			}
			//Paste transform data
			TriButtonRow(paste, TOOLBAR_BUTTON_WIDTH * 2,
				t =>
				{
					Undo.RecordObject(t, "Paste Transform Values");
					if (copiedPosition.HasValue)
					{
						if (copiedSpace == Space.Self) t.localPosition = copiedPosition.Value;
						else t.position = copiedPosition.Value;
					}
					if (copiedRotation.HasValue)
					{
						if (copiedSpace == Space.Self) t.localRotation = copiedRotation.Value;
						else t.rotation = copiedRotation.Value;
					}
					if (copiedScale.HasValue)
					{
						t.localScale = copiedScale.Value;
					}
				},
				t =>
				{
					if (copiedPosition.HasValue)
					{
						Undo.RecordObject(t, "Paste Position");
						if (copiedSpace == Space.Self) t.localPosition = copiedPosition.Value;
						else t.position = copiedPosition.Value;
					}
				},
				t =>
				{
					if (copiedRotation.HasValue)
					{
						Undo.RecordObject(t, "Paste Rotation");
						if (copiedSpace == Space.Self) t.localRotation = copiedRotation.Value;
						else t.rotation = copiedRotation.Value;
					}
				},
				t =>
				{
					if (copiedScale.HasValue)
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
			if (state != lastState && !string.IsNullOrWhiteSpace(editorPref))
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
			if (mainButton != null && GUILayout.Button(mainContent, EditorStyles.toolbarButton, GUILayout.Width(mainWidth)))
				ForEachTransform(mainButton);
			GUI.enabled = enabled && (posEnabledFunc?.Invoke() ?? true);
			if (posButton != null && GUILayout.Button(positionIcon, EditorStyles.toolbarButton, GUILayout.Width(TOOLBAR_BUTTON_WIDTH)))
				ForEachTransform(posButton);
			GUI.enabled = enabled && (rotEnabledFunc?.Invoke() ?? true);
			if (rotButton != null && GUILayout.Button(rotationIcon, EditorStyles.toolbarButton, GUILayout.Width(TOOLBAR_BUTTON_WIDTH)))
				ForEachTransform(rotButton);
			GUI.enabled = enabled && (scaleEnabledFunc?.Invoke() ?? true);
			if (scaleButton != null && GUILayout.Button(scaleIcon, EditorStyles.toolbarButton, GUILayout.Width(TOOLBAR_BUTTON_WIDTH)))
				ForEachTransform(scaleButton);
			GUI.enabled = true;
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

		private void ForEachTransform(Action<Transform> action)
		{
			foreach (var t in targets)
			{
				if (t is Transform transform)
				{
					action.Invoke(transform);
				}
			}
		}

		private void InvokeDrawEvents(Action<TransformGUIContext> action)
		{
			try
			{
				action?.Invoke(new TransformGUIContext(serializedObject, targets));
			}
			catch (Exception e)
			{
				e.LogException("Exception in ExtendedTransformEditor event");
			}
		}
	}
}