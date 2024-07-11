using D3T;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
#if UNITY_2020_1_OR_NEWER
using ToolManager = UnityEditor.EditorTools.ToolManager;
#else
using ToolManager = UnityEditor.EditorTools.EditorTools;
#endif
using UnityEngine;

namespace D3TEditor.Tools
{
	[EditorTool(toolName, typeof(ReflectionProbe))]
	internal class ReflectionProbeBoundsDefinitionTool : BoundsDefinitionTool
	{
		protected override BoundsPropertyFlags BoundsProperties => BoundsPropertyFlags.AxisAlignedFixed;

		protected override bool ApplyBoundsToTarget()
		{
			var probe = target as ReflectionProbe;
			probe.transform.position = calculatedBounds.center;
			probe.center = Vector3.zero;
			probe.size = calculatedBounds.size;
			return true;
		}
	}

	[EditorTool(toolName, typeof(IBoundsComponent))]
	public class ComponentBoundsDefinitionTool : BoundsDefinitionTool
	{
		protected override BoundsPropertyFlags BoundsProperties => _boundsProperties;
		private BoundsPropertyFlags _boundsProperties;
		private IBoundsComponent targetInterface;

		protected override void OnBecameActive()
		{
			base.OnBecameActive();
			targetInterface = (IBoundsComponent)targetComponent;
			_boundsProperties = targetInterface.BoundsProperties;
		}

		protected override bool ApplyBoundsToTarget()
		{
			targetInterface.SetBounds(calculatedBounds, 0, false);
			return true;
		}
	}

	public abstract class BoundsDefinitionTool : EditorToolBase
	{
		public const string toolName = "Edit Bounds";

		public static GUIContent ToolIcon
		{
			get
			{
				if(toolIcon == null) toolIcon = EditorGUIUtility.IconContent("d_PhysicsRaycaster Icon");
				return toolIcon;
			}
		}
		private static GUIContent toolIcon;

		public override GUIContent toolbarIcon => ToolIcon;

		public override bool ShowToolWindow => true;

		protected List<Vector3> positions;
		protected Matrix4x4 boundsMatrix;
		protected Bounds calculatedBounds;

		protected Component targetComponent;
		protected abstract BoundsPropertyFlags BoundsProperties { get; }

		protected override void OnBecameActive()
		{
			positions = new List<Vector3>();
			targetComponent = (Component)target;
		}

		protected override void OnSceneGUI(EditorWindow window, bool enableInteraction)
		{
			HandleUtility.AddDefaultControl(0);

			Handles.color = new Color(1f, 1f, 0.5f, 0.7f);
			for(int i = 0; i < positions.Count; i++)
			{
				DrawPoint(positions[i], i);
			}

			if(positions.Count > 1)
			{
				Handles.color = new Color(0.8f, 0.8f, 0.8f, 0.7f);
				DrawPoint(calculatedBounds.center, -1);
			}

			Handles.color = new Color(1, 1, 0, 0.3f);
			if(AllowClicks && Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out var hit, float.MaxValue, SceneView.lastActiveSceneView.camera.cullingMask, QueryTriggerInteraction.Ignore))
			{
				DrawPoint(hit.point, -1);
				if(MouseDown(0))
				{
					positions.Add(hit.point);
					Event.current.Use();
				}
			}
			RecalculateBounds();

			Handles.matrix = boundsMatrix;
			Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
			Handles.DrawWireCube(calculatedBounds.center, calculatedBounds.size);
			Handles.color = Color.yellow;
			Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
			Handles.DrawWireCube(calculatedBounds.center, calculatedBounds.size);

#if !UNITY_2020_1_OR_NEWER
			window.Repaint();
#endif
		}

		protected override void OnWindowGUI()
		{
			GUI.enabled = positions.Count > 0;
			if(GUILayout.Button("Clear Points"))
			{
				positions.Clear();
			}
			EditorGUILayout.Separator();
			GUI.enabled = positions.Count > 1;
			if(GUILayout.Button("Apply", GUILayout.Height(30)) || KeyDown(KeyCode.Return) || KeyDown(KeyCode.KeypadEnter))
			{
				Apply();
			}
			GUI.enabled = true;
		}

		private void Apply()
		{
			Undo.RecordObject(target, "Define Bounds");
			bool deactivate = ApplyBoundsToTarget();
			if(deactivate) Deactivate();
		}

		protected abstract bool ApplyBoundsToTarget();

		private void DrawPoint(Vector3 p, int index)
		{
			var s = HandleUtility.GetHandleSize(p) * 0.3f;
			var d = 1;

			using(new Handles.DrawingScope(Handles.color.MultiplyAlpha(0.25f)))
			{
				Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
				Handles.DrawDottedLine(p + Vector3.left * s, p + Vector3.right * s, d);
				Handles.DrawDottedLine(p + Vector3.down * s, p + Vector3.up * s, d);
				Handles.DrawDottedLine(p + Vector3.back * s, p + Vector3.forward * s, d);
			}
			Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
			Handles.DrawDottedLine(p + Vector3.left * s, p + Vector3.right * s, d);
			Handles.DrawDottedLine(p + Vector3.down * s, p + Vector3.up * s, d);
			Handles.DrawDottedLine(p + Vector3.back * s, p + Vector3.forward * s, d);

			Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
			if(index >= 0)
			{
				var size = HandleUtility.GetHandleSize(p) * 0.05f;
				if(Handles.Button(p, Quaternion.identity, size, size * 2, Handles.DotHandleCap))
				{
					positions.RemoveAt(index);
				}
			}
		}

		private void RecalculateBounds()
		{
			if(BoundsProperties.HasFlag(BoundsPropertyFlags.Transformable))
			{
				boundsMatrix = targetComponent.transform.localToWorldMatrix;
			}
			else
			{
				boundsMatrix = Matrix4x4.identity;
			}
			var inverseBoundsMatrix = boundsMatrix.inverse;
			if(positions.Count == 0)
			{
				calculatedBounds = new Bounds(Vector3.zero, Vector3.zero);
			}
			else
			{
				calculatedBounds = new Bounds(inverseBoundsMatrix.MultiplyPoint(positions[0]), Vector3.zero);
				for(int i = 1; i < positions.Count; i++)
				{
					calculatedBounds.Encapsulate(inverseBoundsMatrix.MultiplyPoint(positions[i]));
				}
			}
		}
	}
}