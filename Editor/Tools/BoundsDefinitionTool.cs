using UnityEssentials;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
#if UNITY_2020_1_OR_NEWER
using ToolManager = UnityEditor.EditorTools.ToolManager;
#else
#endif
using UnityEngine;

namespace UnityEssentialsEditor.Tools
{
	[EditorTool(TOOL_NAME, typeof(ReflectionProbe))]
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

	[EditorTool(TOOL_NAME, typeof(IBoundsComponent))]
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

#if UNITY_2021_2_OR_NEWER
	[UnityEditor.Overlays.Overlay(typeof(SceneView), "Bounds Tool")]
	internal class BoundsDefinitionToolOverlay : ToolOverlayBase<BoundsDefinitionTool>
	{

	}
#endif

	public abstract class BoundsDefinitionTool : EditorToolBase
	{
		public const string TOOL_NAME = "Edit Bounds";

		public static GUIContent Icon => EditorGUIUtility.IconContent("d_PhysicsRaycaster Icon");

		public override GUIContent toolbarIcon => Icon;

		public override bool ShowToolWindow => true;

		public float offset = 0;

		protected List<Vector3> positions;
		protected Matrix4x4 boundsMatrix;
		protected Bounds calculatedBounds;

		protected Component targetComponent;
		protected abstract BoundsPropertyFlags BoundsProperties { get; }

		protected override void OnBecameActive()
		{
			positions = new List<Vector3>();
			targetComponent = (Component)target;
			offset = 0;
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
			DrawBox(calculatedBounds.center, calculatedBounds.size, Color.yellow);
			if(offset != 0)
			{
				DrawBox(calculatedBounds.center, calculatedBounds.size - Vector3.one * (offset * 2), Color.yellow.WithAlpha(0.3f));
			}

#if !UNITY_2020_1_OR_NEWER
			window.Repaint();
#endif
		}

		private void DrawBox(Vector3 center, Vector3 size, Color color)
		{
			Handles.matrix = boundsMatrix;
			Handles.color = color.MultiplyAlpha(0.3f);
			Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
			Handles.DrawWireCube(center, size);
			Handles.color = color;
			Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
			Handles.DrawWireCube(center, size);
		}

		public override void OnWindowGUI()
		{
			GUI.enabled = positions.Count > 0;
			if(GUILayout.Button("Clear Points"))
			{
				positions.Clear();
			}
			offset = EditorGUILayout.FloatField("Offset", offset);
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
			calculatedBounds.Expand(offset * 2);
		}
	}
}