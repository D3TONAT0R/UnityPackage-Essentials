using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace UnityEssentialsEditor.Tools
{
	[EditorTool("Prefab Placer")]
	public class PrefabPlacementTool : EditorToolBase
	{
		public enum Axis
		{
			[InspectorName("Y-")]
			YNeg,
			[InspectorName("Y+")]
			YPos,
			[InspectorName("Z-")]
			ZNeg,
			[InspectorName("Z+")]
			ZPos
		}

		public GameObject prefab;
		public Axis direction = Axis.YPos;
		public bool alignToNormal = true;
		public Vector3 rotation;
		public float offset;
		public bool setStatic;
		public float groundOffset;
		public float groundCheckOffset = 0.1f;
		public float snapDistance = 0;

		private GameObject cursorInstance;

		public override bool ShowToolWindow => true;

		protected override void OnWillDeactivate()
		{
			if(cursorInstance) DestroyImmediate(cursorInstance);
		}

		protected override void OnSceneGUI(EditorWindow window, bool enableInteraction)
		{
			bool canPlace = false;
			if(Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out var hit, 100f, SceneView.lastActiveSceneView.camera.cullingMask, QueryTriggerInteraction.Ignore))
			{
				Handles.color = Color.gray;
				DrawCross(hit.point, hit.normal);
				if(prefab)
				{
					canPlace = true;
					if(cursorInstance == null)
					{
						cursorInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
						foreach(var collider in cursorInstance.GetComponentsInChildren<Collider>())
						{
							collider.enabled = false;
						}
						cursorInstance.hideFlags = HideFlags.HideAndDontSave;
						cursorInstance.name = prefab.name;
					}
					cursorInstance.SetActive(true);
					var instPos = SetPosition(hit, SceneView.lastActiveSceneView);
					var s = HandleUtility.GetHandleSize(instPos);
					Handles.DrawWireDisc(instPos, hit.normal, s * 0.1f);
					Handles.DrawDottedLine(instPos, instPos + 0.5f * s * hit.normal, 2);
					instPos += hit.normal * offset;

					cursorInstance.transform.parent = Selection.activeTransform;
					cursorInstance.transform.position = instPos;
					if(!ShiftDown)
					{
						var rot = CalcRotation(hit);
						cursorInstance.transform.rotation = rot;
						cursorInstance.transform.Rotate(rotation);
					}
					Handles.matrix = Matrix4x4.TRS(cursorInstance.transform.position, cursorInstance.transform.rotation, Vector3.one);
					Handles.color = Handles.xAxisColor;
					Handles.DrawLine(Vector3.zero, 0.5f * s * Vector3.right);
					Handles.color = Handles.yAxisColor;
					Handles.DrawLine(Vector3.zero, 0.5f * s * Vector3.up);
					Handles.color = Handles.zAxisColor;
					Handles.DrawLine(Vector3.zero, 0.5f * s * Vector3.forward);
				}
			}
			else
			{
				canPlace = false;
				if(cursorInstance)
				{
					cursorInstance.SetActive(false);
				}
			}
			if(MouseDown(0))
			{
				UseEvent();
				if(canPlace)
				{
					var instance = cursorInstance;
					cursorInstance = null;
					Undo.RegisterCreatedObjectUndo(instance, "Place " + instance.name);
					foreach(var collider in instance.GetComponentsInChildren<Collider>())
					{
						PrefabUtility.RevertObjectOverride(collider, InteractionMode.AutomatedAction);
					}
					instance.hideFlags = HideFlags.None;
				}
			}
			HandleUtility.AddDefaultControl(0);
			window.Repaint();
		}

		private Quaternion CalcRotation(RaycastHit hit)
		{
			Vector3 directionNormal = Vector3.up;
			switch(direction)
			{
				case Axis.YPos: directionNormal = Vector3.up; break;
				case Axis.YNeg: directionNormal = Vector3.down; break;
				case Axis.ZPos: directionNormal = Vector3.forward; break;
				case Axis.ZNeg: directionNormal = Vector3.back; break;
			}
			Vector3 directionNormalPos = new Vector3(Mathf.Abs(directionNormal.x), Mathf.Abs(directionNormal.y), Mathf.Abs(directionNormal.z));
			bool faceInto = direction == Axis.YNeg || direction == Axis.ZNeg;
			Quaternion rot = Quaternion.identity;
			if(!alignToNormal || hit.normal == directionNormal)
			{
				rot = Quaternion.identity;
			}
			else if(hit.normal == -directionNormal)
			{
				switch(direction)
				{
					case Axis.YNeg: rot = Quaternion.Euler(180, 180, 0); break;
					case Axis.YPos: rot = Quaternion.Euler(180, 180, 0); break;
					case Axis.ZNeg: rot = Quaternion.Euler(0, 180, 0); break;
					case Axis.ZPos: rot = Quaternion.Euler(0, 180, 0); break;
				}
			}
			else
			{
				Vector3 upVector = direction == Axis.ZNeg || direction == Axis.ZPos ? Vector3.up : directionNormalPos;
				if(hit.normal == upVector) upVector = Vector3.forward;
				rot = Quaternion.LookRotation(hit.normal, upVector);
				switch(direction)
				{
					case Axis.YNeg: rot *= Quaternion.Euler(-90, 0, 0); break;
					case Axis.YPos: rot *= Quaternion.Euler(-90, 180, 0); break;
					case Axis.ZNeg: rot *= Quaternion.Euler(0, 180, 0); break;
					case Axis.ZPos: rot *= Quaternion.Euler(0, 0, 0); break;
				}
			}
			return rot;
		}

		private void DrawCross(Vector3 pos, Vector3 normal)
		{
			var lMatrix = Handles.matrix;
			Handles.matrix *= Matrix4x4.TRS(pos, Quaternion.LookRotation(normal), Vector3.one);
			var s = HandleUtility.GetHandleSize(Vector3.zero) * 0.2f;
			Handles.DrawLine(new Vector3(0, -s, 0), new Vector3(0, s, 0));
			Handles.DrawLine(new Vector3(-s, 0, 0), new Vector3(s, 0, 0));
			Handles.matrix = lMatrix;
		}

		Vector3 SetPosition(RaycastHit hit, SceneView s)
		{
			if(groundOffset > 0)
			{
				if(Physics.Raycast(new Ray(hit.point + hit.normal * Mathf.Max(0.01f, groundCheckOffset), Vector3.down), out var hit2, 100f, s.camera.cullingMask, QueryTriggerInteraction.Ignore))
				{
					var groundOffsetPos = hit2.point + Vector3.up * groundOffset;
					var pos = hit.point;
					if(pos.y < groundOffsetPos.y)
					{
						pos.y = groundOffsetPos.y;
					}
					return SnapPosition(pos, hit2.normal);
				}
			}
			return SnapPosition(hit.point, hit.normal);
		}

		Vector3 SnapPosition(Vector3 pos, Vector3 normal)
		{
			float snapThreshold = 1f;// Mathf.Sqrt(0.5f);
			if(snapDistance > 0)
			{
				var matrix = Matrix4x4.TRS(pos, Quaternion.LookRotation(normal), Vector3.one);
				if(Mathf.Abs(normal.x) < snapThreshold * 1.01f)
				{
					pos.x = Mathf.Round(pos.x / snapDistance) * snapDistance;
				}
				if(Mathf.Abs(normal.y) < snapThreshold)
				{
					pos.y = Mathf.Round(pos.y / snapDistance) * snapDistance;
				}
				if(Mathf.Abs(normal.z) < snapThreshold)
				{
					pos.z = Mathf.Round(pos.z / snapDistance) * snapDistance;
				}
				pos = matrix.inverse.MultiplyPoint(pos);
				pos.z = 0;
				pos = matrix.MultiplyPoint(pos);
				return pos;
			}
			else
			{
				return pos;
			}
		}

		protected override void OnWindowGUI()
		{
			EditorGUI.BeginChangeCheck();
			prefab = (GameObject)EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
			if(EditorGUI.EndChangeCheck())
			{
				if(cursorInstance) DestroyImmediate(cursorInstance.gameObject);
			}
			alignToNormal = EditorGUILayout.Toggle("Align To Normal", alignToNormal);
			direction = EditorGUIExtras.EnumButtons(new GUIContent("Direction"), direction);
			offset = EditorGUILayout.FloatField("Offset", offset);
			rotation = EditorGUILayout.Vector3Field("Rotation", rotation);
			setStatic = EditorGUILayout.Toggle("Force Static", setStatic);
			groundOffset = Mathf.Max(EditorGUILayout.FloatField("Ground Offset", groundOffset), 0);
			groundCheckOffset = Mathf.Max(EditorGUILayout.FloatField("Ground Check Dist", groundCheckOffset), 0);
			snapDistance = EditorGUILayout.FloatField("Snap Distance", snapDistance);
		}
	}
}