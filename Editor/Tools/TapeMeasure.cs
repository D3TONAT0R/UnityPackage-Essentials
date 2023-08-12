using D3T;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.Tools {
	public class TapeMeasure : EditorWindow {

		public bool enable;
		public Vector3 p1;
		public Vector3 p2;
		public Vector3 distanceVec;
		public float distance3d;
		public float distance2d;
		public float angle2d;
		public int setPoint;

		Color color1 = Color.red;
		Color color2 = Color.blue;

		[MenuItem("Window/Tape Measure")]
		static void Init() {
			TapeMeasure window = (TapeMeasure)GetWindow(typeof(TapeMeasure));
			window.Show();
		}
		// draw lines between a chosen game object
		// and a selection of added game objects

		// Window has been selected
		void OnFocus() {
			// Remove delegate listener if it has previously
			// been assigned.
			SceneView.duringSceneGui -= OnSceneGUI;
			// Add (or re-add) the delegate.
			SceneView.duringSceneGui += OnSceneGUI;
			Repaint();
		}

		private void OnEnable() {
			//EditorApplication.update += OnGUI;
		}

		void OnDestroy() {
			// When the window is destroyed, remove the delegate
			// so that it will no longer do any drawing.
			//EditorApplication.update -= OnGUI;
			SceneView.duringSceneGui -= OnSceneGUI;
		}

		void OnGUI() {
			enable = EditorGUILayout.Toggle("Enable Tool", enable);
			GUILayout.Label("Set Point: " + setPoint);
			p1 = EditorGUILayout.Vector3Field("Point 1", p1);
			p2 = EditorGUILayout.Vector3Field("Point 2", p2);
			GUILayout.Label("Results:", EditorStyles.boldLabel);
			if(p1 != Vector3.zero && p2 != Vector3.zero) {
				distanceVec = p2 - p1;
				distance3d = Vector3.Distance(p1, p2);
				distance2d = Vector2.Distance(p1.XZ(), p2.XZ());
				var vec = p2 - p1;
				if(vec != Vector3.zero) {
					angle2d = Quaternion.LookRotation(p2 - p1).eulerAngles.y;
				} else {
					angle2d = 0;
				}
			} else {
				distanceVec = Vector3.zero;
				distance3d = 0;
				distance2d = 0;
				angle2d = 0;
				GUI.enabled = false;
			}
			EditorGUILayout.Vector3Field("Distance vector", distanceVec);
			EditorGUILayout.FloatField("Total distance", distance3d);
			EditorGUILayout.FloatField("Total distance (2D)", distance2d);
			EditorGUILayout.FloatField("Angle (2D)", angle2d);
			GUI.enabled = true;
			if(Event.current.type == EventType.Repaint) {
				SceneView.lastActiveSceneView.Repaint();
			}
		}

		void DrawPoint(Vector3 vec, Color c) {
			Handles.color = Color.red;
			Handles.DrawDottedLine(vec + Vector3.left, vec + Vector3.right, 1);
			Handles.color = Color.green;
			Handles.DrawDottedLine(vec + Vector3.down, vec + Vector3.up, 1);
			Handles.color = Color.blue;
			Handles.DrawDottedLine(vec + Vector3.back, vec + Vector3.forward, 1);
			Handles.color = c;
			Handles.DrawWireCube(vec, 0.1f.ToUVector3());
		}

		void OnSceneGUI(SceneView sceneView) {
			if(!enable) return;
			Repaint();
			Handles.BeginGUI();
			Handles.color = Color.green;
			GUI.color = Color.red;
			GUI.Label(new Rect(16, 16, 300, 100), "Tape Measure Tool\nis active!", EditorStyles.whiteLargeLabel);
			if(GUI.Button(new Rect(16, 64, 100, 16), "Disable")) enable = false;
			Handles.EndGUI();
			//if(Event.current.mousePosition.x < sceneView.position.xMin || Event.current.mousePosition.y < sceneView.position.yMin || Event.current.mousePosition.x > sceneView.position.xMax || Event.current.mousePosition.y > sceneView.position.yMax) return;
			RaycastHit hit;
			if(Physics.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit, 100f, sceneView.camera.cullingMask, QueryTriggerInteraction.Ignore)) {
				Handles.color = setPoint == 1 ? color1 : color2;
				Handles.DrawWireDisc(hit.point, hit.normal, 0.1f);
			} else {
				return;
			}
			if(p1 != Vector3.zero && setPoint == 1) {
				DrawPoint(p1, color1);
			}
			if(p2 != Vector3.zero && setPoint == 2) {
				DrawPoint(p2, color2);
			}
			if(p1 != Vector3.zero && p2 != Vector3.zero) {
				Handles.color = Color.yellow;
				Handles.DrawDottedLine(p1, p2, 4);
				if(setPoint == 0) {
					float scale = 0.5f;
					Handles.matrix = Matrix4x4.Scale(Vector3.one * scale);
					p1 = Handles.PositionHandle(p1 / scale, Quaternion.identity) * scale;
					p2 = Handles.PositionHandle(p2 / scale, Quaternion.identity) * scale;
					Handles.matrix = Matrix4x4.identity;
				}
			}
			if(setPoint == 1) {
				p1 = hit.point;
			} else if(setPoint == 2) {
				p2 = hit.point;
			}
			// Retrieve the control Id
			// Start treating your event
			if(Event.current.keyCode == KeyCode.Escape) {
				enable = false;
				return;
			}
			if(Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.modifiers != EventModifiers.Shift) {
				int controlId = GUIUtility.GetControlID(FocusType.Passive);
				GUIUtility.hotControl = controlId;
				if(setPoint == 0) {
					p1 = Vector3.zero;
					p2 = Vector3.zero;
				}
				setPoint++;
				setPoint %= 3;
				Event.current.Use();
				GUIUtility.hotControl = 0;
			} else {
				GUIUtility.hotControl = GUIUtility.hotControl;
			}
		}
	} 
}