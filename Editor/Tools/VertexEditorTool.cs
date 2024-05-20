using UnityEssentials.Meshes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
#if UNITY_2020_1_OR_NEWER
using ToolManager = UnityEditor.EditorTools.ToolManager;
#else
using ToolManager = UnityEditor.EditorTools.EditorTools;
#endif

namespace UnityEssentialsEditor.Tools
{
	[EditorTool("Edit Vertices", typeof(ConvexMeshBuilderComponent))]
	internal class VertexEditorTool : EditorToolBase
	{
		List<int> selection = new List<int>();

		float snapDistance = 0.1f;
		bool alwaysSnap = false;
		float scatterDistance = 0.1f;

		private ConvexMeshBuilderComponent component;

		private Vector2 dragStartPos;
		private Vector2 dragEndPos;
		private bool dragEnded;

		public override GUIContent toolbarIcon => EditorGUIUtility.IconContent("d_EditCollider");

		public override bool ShowToolWindow => true;

		protected override void OnBecameActive()
		{
			selection.Clear();
			component = target as ConvexMeshBuilderComponent;
		}

		private void OnDisable()
		{
			component = null;
		}

		protected override void OnSceneGUI(EditorWindow window, bool enableInteraction)
		{
			Undo.RecordObject(component, "Modify Vertices");
			var lMatrix = Handles.matrix;
			Handles.matrix = component.transform.localToWorldMatrix;
			var verts = component.vertices;

			if(verts.Count > 0)
			{
				for(int i = 0; i < verts.Count; i++)
				{
					var size = HandleUtility.GetHandleSize(verts[i]) * 0.03f;

					if(selection.Contains(i))
					{
						Handles.color = new Color(1.0f, 0.6f, 0.2f, 1.0f);
						if(Handles.Button(verts[i], Quaternion.identity, size * 1.5f, 0, Handles.DotHandleCap))
						{
							if(Event.current.shift)
							{
								RemoveFromSelection(i);
							}
							else
							{
								SetSelection(i);
							}
						}
						Handles.color = Color.white;
					}
					else
					{
						if(Handles.Button(verts[i], Quaternion.identity, size, size * 1.5f, Handles.DotHandleCap))
						{
							if(!Event.current.shift)
							{
								ClearSelection();
							}
							AddToSelection(i);
						}
					}
				}
				if(selection.Count > 0 && dragEndPos == Vector2.zero)
				{
					Vector3 handlePos = Vector3.zero;
					foreach(var i in selection)
					{
						handlePos += verts[i];
					}
					handlePos /= selection.Count;
					var moveSnap = EditorSnapSettings.move;
					EditorSnapSettings.move = Vector3.zero;
					var newHandlePos = PositionHandle(handlePos, 1);
					if(handlePos != newHandlePos)
					{
						if(CtrlDown ^ alwaysSnap)
						{
							if(newHandlePos.x != handlePos.x) newHandlePos.x = Snapping.Snap(newHandlePos.x, snapDistance);
							if(newHandlePos.y != handlePos.y) newHandlePos.y = Snapping.Snap(newHandlePos.y, snapDistance);
							if(newHandlePos.z != handlePos.z) newHandlePos.z = Snapping.Snap(newHandlePos.z, snapDistance);
						}
						var delta = newHandlePos - handlePos;
						foreach(var i in selection)
						{
							verts[i] += delta;
						}
						component.Validate();
					}
					EditorSnapSettings.move = moveSnap;
				}
			}

			Handles.color = Color.red;
			Handles.DrawLine(Vector3.zero, Vector3.right * 0.25f);
			Handles.color = Color.green;
			Handles.DrawLine(Vector3.zero, Vector3.up * 0.25f);
			Handles.color = Color.blue;
			Handles.DrawLine(Vector3.zero, Vector3.forward * 0.25f);

			Handles.matrix = lMatrix;

			Handles.BeginGUI();

			DetectMouseDrag();
			DoDragSelection();

			if(MouseUp(0) && !dragEnded)
			{
				UseEvent();
				if(selection.Count > 0)
				{
					ClearSelection();
				}
				else
				{
					ToolManager.RestorePreviousTool();
				}
			}

			HandleUtility.AddDefaultControl(0);
			if(AnyKeyDown(KeyCode.Delete, KeyCode.Backspace))
			{
				Event.current.Use();
				List<Vector3> rm = new List<Vector3>();
				foreach(var s in selection.OrderByDescending(i => i))
				{
					component.vertices.RemoveAt(s);
				}
				ClearSelection();
				component.Validate();
			}
			if(CtrlDown && KeyDown(KeyCode.A))
			{
				Event.current.Use();
				SelectAll();
			}
			if(CtrlDown && KeyDown(KeyCode.D))
			{
				Event.current.Use();
				if(selection.Count > 0)
				{
					int dupeCount = selection.Count;
					foreach(var i in selection)
					{
						component.vertices.Add(component.vertices[i]);
					}
					ClearSelection();
					for(int i = 0; i < dupeCount; i++)
					{
						AddToSelection(component.vertices.Count - dupeCount + i);
					}
				}
			}

			dragEnded = false;
			Handles.EndGUI();

			window.Repaint();
		}

		private void DoDragSelection()
		{
			if(dragStartPos != Vector2.zero && dragEndPos != Vector2.zero)
			{
				var svc = SceneView.lastActiveSceneView.camera;
				var pos = new Vector2(Mathf.Min(dragStartPos.x, dragEndPos.x), Mathf.Min(dragStartPos.y, dragEndPos.y));
				var size = new Vector2(Mathf.Abs(dragEndPos.x - dragStartPos.x), Mathf.Abs(dragEndPos.y - dragStartPos.y));
				Rect r = new Rect(pos, size);
				selection.Clear();
				for(int i = 0; i < component.vertices.Count; i++)
				{
					if(selection.Contains(i)) continue;
					var vertSP = svc.WorldToScreenPoint(component.transform.localToWorldMatrix.MultiplyPoint(component.vertices[i]));
					vertSP.y = svc.pixelHeight - vertSP.y;
					if(r.Contains(vertSP))
					{
						selection.Add(i);
					}
				}
				EditorGUI.DrawRect(r, new Color(0.7f, 0.7f, 0.7f, 0.4f));
			}
		}

		private void DetectMouseDrag()
		{
			if(MouseDown(0))
			{
				dragStartPos = Event.current.mousePosition;
			}
			if(MouseUp(0))
			{
				if(dragEndPos != Vector2.zero) dragEnded = true;
				dragStartPos = Vector2.zero;
				dragEndPos = Vector2.zero;
			}
			if(Event.current.type == EventType.MouseDrag && Event.current.button == 0 && dragStartPos != Vector2.zero && !dragEnded)
			{
				if(dragEndPos != null || Vector2.Distance(Event.current.mousePosition, dragStartPos) > 10)
				{
					dragEndPos = Event.current.mousePosition;
				}
			}
		}

		protected override void OnWindowGUI()
		{
			if(GUILayout.Button("Add Vertex"))
			{
				component.AddVertex();
				SetSelection(component.vertices.Count - 1);
				component.Validate();
			}
			if(GUILayout.Button("Add Cube"))
			{
				float f = 0.5f;
				component.AddVertex(-f, -f, -f);
				component.AddVertex(f, -f, -f);
				component.AddVertex(-f, -f, f);
				component.AddVertex(f, -f, f);
				component.AddVertex(-f, f, -f);
				component.AddVertex(f, f, -f);
				component.AddVertex(-f, f, f);
				component.AddVertex(f, f, f);
				ClearSelection();
				for(int i = 0; i < 8; i++)
				{
					AddToSelection(component.vertices.Count - 8 + i);
				}
				component.Validate();
			}
			GUILayout.Space(5);
			GUI.enabled = selection.Count > 0;
			if(GUILayout.Button("Delete Selected"))
			{
				foreach(var s in selection) component.vertices.RemoveAt(s);
				ClearSelection();
				component.Validate();
			}
			GUI.enabled = true;
			GUILayout.Space(10);
			GUILayout.Label("Select");
			if(GUILayout.Button("All")) SelectAll();
			GUI.enabled = selection.Count == 1;
			Vector3 comparison = selection.Count == 1 ? component.vertices[selection[0]] : Vector3.zero;
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("X+", EditorStyles.miniButtonLeft)) SelectConditional(p => p.x >= comparison.x);
			if(GUILayout.Button("Y+", EditorStyles.miniButtonMid)) SelectConditional(p => p.y >= comparison.y);
			if(GUILayout.Button("Z+", EditorStyles.miniButtonRight)) SelectConditional(p => p.z >= comparison.z);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("X-", EditorStyles.miniButtonLeft)) SelectConditional(p => p.x <= comparison.x);
			if(GUILayout.Button("Y-", EditorStyles.miniButtonMid)) SelectConditional(p => p.y <= comparison.y);
			if(GUILayout.Button("Z-", EditorStyles.miniButtonRight)) SelectConditional(p => p.z <= comparison.z);
			GUILayout.EndHorizontal();
			GUI.enabled = true;

			GUILayout.Space(10);
			GUILayout.Label("Snap Distance");
			using(new EditorGUILayout.HorizontalScope())
			{
				snapDistance = EditorGUILayout.FloatField(snapDistance, GUILayout.ExpandWidth(true));
				snapDistance = Mathf.Max(snapDistance, 0.001f);
				alwaysSnap = GUILayout.Toggle(alwaysSnap, "Force");
			}
			GUI.enabled = selection.Count > 0;
			if(GUILayout.Button("Snap Selected"))
			{
				foreach(var i in selection)
				{
					component.vertices[i] = Snapping.Snap(component.vertices[i], Vector3.one * snapDistance);
				}
				component.Validate();
			}

			GUILayout.Space(10);
			GUILayout.Label("Scattering");
			using(new EditorGUILayout.HorizontalScope())
			{
				scatterDistance = EditorGUILayout.FloatField(scatterDistance, GUILayout.ExpandWidth(true));
				scatterDistance = Mathf.Max(scatterDistance, 0.001f);
				GUI.enabled = selection.Count > 0;
				if(GUILayout.Button("Scatter"))
				{
					foreach(var i in selection)
					{
						component.vertices[i] += Random.insideUnitSphere * scatterDistance;
					}
					component.Validate();
				}
			}

			GUILayout.Space(10);
			if(GUILayout.Button("Remove duplicates"))
			{
				ClearSelection();
				component.vertices = component.vertices.Distinct().ToList();
				component.Validate();
			}

			//GUI.DragWindow(new Rect(0, 0, 1000, 20));
		}

		private void SelectConditional(System.Func<Vector3, bool> predicate)
		{
			SetSelection();
			for(int i = 0; i < component.vertices.Count; i++)
			{
				if(predicate.Invoke(component.vertices[i])) selection.Add(i);
			}
		}

		private void SetSelection(params int[] indices)
		{
			selection.Clear();
			AddToSelection(indices);
		}

		private void ClearSelection()
		{
			SetSelection();
		}

		private void SelectAll()
		{
			ClearSelection();
			for(int i = 0; i < component.vertices.Count; i++)
			{
				selection.Add(i);
			}
		}

		private void AddToSelection(params int[] indices)
		{
			foreach(var i in indices) selection.Add(i);
		}

		private void RemoveFromSelection(params int[] indices)
		{
			foreach(var i in indices) selection.Remove(i);
		}

		private Vector3 PositionHandle(Vector3 pos, float handleScale)
		{
			var lastMatrix = Handles.matrix;

			Vector3 scale = new Vector3(
				lastMatrix.GetColumn(0).magnitude,
				lastMatrix.GetColumn(1).magnitude,
				lastMatrix.GetColumn(2).magnitude
			);

			var inverseScaleMatrix = Matrix4x4.Scale(scale / handleScale).inverse;
			var pos2 = inverseScaleMatrix.inverse.MultiplyPoint(pos);

			Handles.matrix *= inverseScaleMatrix;
			//Handles.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(lastMatrix.GetColumn(2), lastMatrix.GetColumn(1)), Vector3.one);
			EditorGUI.BeginChangeCheck();
			pos2 = Handles.PositionHandle(pos2, Quaternion.identity);
			if(EditorGUI.EndChangeCheck())
			{
				pos = inverseScaleMatrix.MultiplyPoint(pos2);
			}
			Handles.matrix = lastMatrix;
			return pos;
		}
	}
}