using D3T;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
#if UNITY_2020_1_OR_NEWER
using ToolManager = UnityEditor.EditorTools.ToolManager;
#else
#endif

namespace D3TEditor.Tools
{

	//TODO: needs some rework
	[EditorTool("Measure")]
	internal class MeasuringTool : EditorToolBase
	{

		public Vector3 p1;
		public Vector3 p2;
		public Vector3 distanceVec;
		public float distance3d;
		public float distance2d;
		public float angle2d;
		public int setPoint = 1;
		private Color color1 = Color.red;
		private Color color2 = Color.blue;

		public override bool ShowToolWindow => true;

		public override int ToolWindowWidth => 300;

		protected override void OnWindowGUI()
		{
			EditorGUIUtility.labelWidth = 120;
			p1 = EditorGUILayout.Vector3Field("Point 1", p1);
			p2 = EditorGUILayout.Vector3Field("Point 2", p2);
			EditorGUILayout.Space(10);
			if(p1 != Vector3.zero && p2 != Vector3.zero)
			{
				distanceVec = p2 - p1;
				distance3d = Vector3.Distance(p1, p2);
				distance2d = Vector2.Distance(p1.XZ(), p2.XZ());
				var vec = p2 - p1;
				if(vec != Vector3.zero)
				{
					angle2d = Quaternion.LookRotation(p2 - p1).eulerAngles.y;
				}
				else
				{
					angle2d = 0;
				}
			}
			else
			{
				distanceVec = Vector3.zero;
				distance3d = 0;
				distance2d = 0;
				angle2d = 0;
				GUI.enabled = false;
			}
			GUI.enabled = false;
			EditorGUILayout.LabelField("Distance (Per Axis)", $"X {distanceVec.x:F3},    Y {distanceVec.y:F3},    Z {distanceVec.z:F3}");
			EditorGUILayout.LabelField("Distance (2D)", distance2d.ToString("F3"));
			EditorGUILayout.LabelField("Distance", distance3d.ToString("F3"));
			EditorGUILayout.LabelField("Angle (2D)", angle2d.ToString("F1"));
			GUI.enabled = true;
			if(Event.current.type == EventType.Repaint)
			{
				SceneView.lastActiveSceneView.Repaint();
			}
		}

		protected override void OnSceneGUI(EditorWindow window, bool enableInteraction)
		{
			RaycastHit hit;
			if(Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit, 100f, SceneView.lastActiveSceneView.camera.cullingMask, QueryTriggerInteraction.Ignore))
			{
				Handles.color = setPoint == 1 ? color1 : color2;
				Handles.DrawWireDisc(hit.point, hit.normal, 0.1f);
			}
			if(p1 != Vector3.zero && setPoint == 1)
			{
				DrawPoint(p1, color1);
			}
			if(p2 != Vector3.zero && setPoint == 2)
			{
				DrawPoint(p2, color2);
			}
			if(p1 != Vector3.zero && p2 != Vector3.zero)
			{
				Handles.color = Color.yellow;
				Handles.DrawDottedLine(p1, p2, 4);
				if(setPoint == 0)
				{
					float scale = 0.5f;
					Handles.matrix = Matrix4x4.Scale(Vector3.one * scale);
					p1 = Handles.PositionHandle(p1 / scale, Quaternion.identity) * scale;
					p2 = Handles.PositionHandle(p2 / scale, Quaternion.identity) * scale;
					Handles.matrix = Matrix4x4.identity;
				}
			}
			if(setPoint == 1)
			{
				p1 = hit.point;
			}
			else if(setPoint == 2)
			{
				p2 = hit.point;
			}

			if(Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.modifiers != EventModifiers.Shift)
			{
				int controlId = GUIUtility.GetControlID(FocusType.Passive);
				GUIUtility.hotControl = controlId;
				if(setPoint == 0)
				{
					p1 = Vector3.zero;
					p2 = Vector3.zero;
				}
				setPoint++;
				setPoint %= 3;
				Event.current.Use();
				GUIUtility.hotControl = 0;
			}
		}

		private void DrawPoint(Vector3 vec, Color c)
		{
			Handles.color = Color.red;
			Handles.DrawDottedLine(vec + Vector3.left, vec + Vector3.right, 1);
			Handles.color = Color.green;
			Handles.DrawDottedLine(vec + Vector3.down, vec + Vector3.up, 1);
			Handles.color = Color.blue;
			Handles.DrawDottedLine(vec + Vector3.back, vec + Vector3.forward, 1);
			Handles.color = c;
			Handles.DrawWireCube(vec, Vector3.one * 0.1f);
		}
	}
}