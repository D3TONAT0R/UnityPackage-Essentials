using D3T;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace D3TEditor.Tools
{
	[EditorTool("Transform Randomizer")]
	public class TransformRandomizerTool : EditorToolBase
	{
		[System.Serializable]
		public class Settings
		{
			public bool enablePosition = true;
			public bool positionSeparateAxes;
			public bool positionEquilateral = true;
			public Vector3 positionMin = Vector3.zero;
			public Vector3 positionMax = Vector3.one;
			public Space translationSpace = Space.World;

			public bool enableRotation;
			public Vector3 rotationMin = Vector3.up * -180f;
			public Vector3 rotationMax = Vector3.up * 180f;
			public Space rotationSpace = Space.World;

			public bool enableScale;
			public bool scaleNonUniform = false;
			public Vector3 scaleMin = Vector3.one * 0.5f;
			public Vector3 scaleMax = Vector3.one * 1.5f;
			public bool scaleAdditive = true;
		}

		public override GUIContent toolbarIcon
		{
			get
			{
				if(toolIcon == null)
				{
					var icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.github.d3tonat0r.core/Gizmos/tool_random.png");
					if(icon == null) icon = Texture2D.whiteTexture;
					toolIcon = new GUIContent(icon);
				}
				return toolIcon;
			}
		}
		private static GUIContent toolIcon;

		public override bool ShowToolWindow => true;
		public override string ToolWindowTitle => "Randomize Transform";
		public override int ToolWindowWidth => 280;
		private bool CanApply => Selection.gameObjects.Length > 0 && (settings.enablePosition || settings.enableRotation || settings.enableScale);

		[SerializeField]
		private Settings settings;

		private static GUIStyle iconStyle;
		private static GUIContent lockIcon;
		private static GUIContent lockIconOn;

		protected override void OnBecameActive()
		{
			lockIcon = EditorGUIUtility.IconContent("LockIcon");
			lockIconOn = EditorGUIUtility.IconContent("LockIcon-On");
			if(settings == null) settings = new Settings();
		}

		protected override void OnSceneGUI(EditorWindow wndow, bool enableInteraction)
		{
			foreach(var s in Selection.gameObjects)
			{
				DrawGizmosForTransform(s.transform);
			}
		}

		private void DrawGizmosForTransform(Transform t)
		{
			var size = HandleUtility.GetHandleSize(t.position);
			Handles.SphereHandleCap(-1, t.position, Quaternion.identity, size * 0.15f, EventType.Repaint);
			if(settings.enablePosition)
			{
				Handles.matrix = GetMatrix(t, settings.translationSpace);
				using(new Handles.DrawingScope(Handles.xAxisColor))
				{
					LimiterLine(Vector3.right, Vector3.forward, settings.positionMin.x, settings.positionMax.x);
				}
				using(new Handles.DrawingScope(Handles.yAxisColor))
				{
					LimiterLine(Vector3.up, Vector3.right, settings.positionMin.y, settings.positionMax.y);
				}
				using(new Handles.DrawingScope(Handles.zAxisColor))
				{
					LimiterLine(Vector3.forward, Vector3.right, settings.positionMin.z, settings.positionMax.z);
				}
			}
			if(settings.enableRotation)
			{
				Handles.matrix = GetMatrix(t, settings.rotationSpace);
				var radius = HandleUtility.GetHandleSize(Vector3.zero) * 0.35f;
				using(new Handles.DrawingScope(Handles.xAxisColor))
				{
					Arc(Vector3.right, Vector3.forward, settings.rotationMin.x, settings.rotationMax.x, radius);
				}
				using(new Handles.DrawingScope(Handles.yAxisColor))
				{
					Arc(Vector3.up, Vector3.forward, settings.rotationMin.y, settings.rotationMax.y, radius);
				}
				using(new Handles.DrawingScope(Handles.zAxisColor))
				{
					Arc(Vector3.back, Vector3.up, settings.rotationMin.z, settings.rotationMax.z, radius);
				}
			}
			Handles.matrix = Matrix4x4.identity;
		}

		private void LimiterLine(Vector3 dir, Vector3 d2, float min, float max)
		{
			if(max <= min) return;
			Vector3 p1 = dir * min;
			Vector3 p2 = dir * max;
			Handles.DrawDottedLine(p1, p2, 2);
			var size = HandleUtility.GetHandleSize(p1) * 0.05f;
			Handles.DrawLine(p1 - d2 * size, p1 + d2 * size);
			size = HandleUtility.GetHandleSize(p2) * 0.05f;
			Handles.DrawLine(p2 - d2 * size, p2 + d2 * size);
		}

		private void Arc(Vector3 up, Vector3 dir, float min, float max, float radius)
		{
			Handles.DrawLine(dir * radius * 0.9f, dir * radius * 1.1f);
			using(new Handles.DrawingScope(Handles.matrix * Matrix4x4.Rotate(Quaternion.AngleAxis(min, up))))
			{
				Handles.DrawWireArc(Vector3.zero, up, dir, max - min, radius);
			}
		}

		private Matrix4x4 GetMatrix(Transform t, Space space)
		{
			if(space == Space.Self)
			{
				return Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
			}
			else
			{
				return Matrix4x4.TRS(t.position, Quaternion.identity, Vector3.one);
			}
		}

		protected override void OnWindowGUI()
		{
			if(iconStyle == null)
			{
				iconStyle = "IconButton";
				iconStyle.alignment = TextAnchor.MiddleCenter;
			}
			EditorGUIUtility.wideMode = true;
			if(settings == null) settings = new Settings();
			settings.enablePosition = EditorGUILayout.Toggle("Position", settings.enablePosition);
			GUI.enabled = settings.enablePosition;
			settings.positionSeparateAxes = EditorGUILayout.Toggle("Separate Axes", settings.positionSeparateAxes);
			var lockRect = GUILayoutUtility.GetLastRect();
			lockRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			lockRect.width = lockRect.height;
			lockRect.x += EditorGUIUtility.labelWidth - lockRect.width - 2;
			if(GUI.Button(lockRect, settings.positionEquilateral ? lockIconOn : lockIcon, iconStyle))
			{
				settings.positionEquilateral = !settings.positionEquilateral;
			}
			EditorGUIUtility.AddCursorRect(lockRect, MouseCursor.Arrow);
			if(!settings.positionSeparateAxes)
			{
				bool e = GUI.enabled;
				if(settings.positionEquilateral)
				{
					settings.positionMin = -settings.positionMax;
					GUI.enabled = false;
				}
				settings.positionMin = Vector3.one * Mathf.Min(EditorGUILayout.FloatField("Min", settings.positionMin.x), 0);
				GUI.enabled = e;
				settings.positionMax = Vector3.one * Mathf.Max(EditorGUILayout.FloatField("Max", settings.positionMax.x), 0);
			}
			else
			{
				bool e = GUI.enabled;
				if(settings.positionEquilateral)
				{
					settings.positionMin = -settings.positionMax;
					GUI.enabled = false;
				}
				settings.positionMin = Vector3.Min(EditorGUILayout.Vector3Field("Min", settings.positionMin), Vector3.zero);
				GUI.enabled = e;
				settings.positionMax = Vector3.Max(EditorGUILayout.Vector3Field("Max", settings.positionMax), Vector3.zero);
				PresetButtons(ref settings.positionMin, ref settings.positionMax, "0", "1", Vector2.zero, new Vector2(-1f, 1f));
			}
			settings.translationSpace = (Space)EditorGUIExtras.IntButtonField(new GUIContent("Space"), (int)settings.translationSpace, "World", "Local");

			EditorGUILayout.Separator();
			GUI.enabled = true;
			settings.enableRotation = EditorGUILayout.Toggle("Rotation", settings.enableRotation);
			GUI.enabled = settings.enableRotation;
			settings.rotationMin = EditorGUILayout.Vector3Field("Min", settings.rotationMin);
			settings.rotationMax = EditorGUILayout.Vector3Field("Max", settings.rotationMax);
			PresetButtons(ref settings.rotationMin, ref settings.rotationMax, "0", "F", Vector2.zero, new Vector2(-180, 180));
			settings.rotationSpace = (Space)EditorGUIExtras.IntButtonField(new GUIContent("Space"), (int)settings.rotationSpace, "World", "Local");

			EditorGUILayout.Separator();
			GUI.enabled = true;
			settings.enableScale = EditorGUILayout.Toggle("Scale", settings.enableScale);
			GUI.enabled = settings.enableScale;
			settings.scaleNonUniform = EditorGUILayout.Toggle("Non Uniform", settings.scaleNonUniform);
			if(settings.scaleNonUniform)
			{
				settings.scaleMin = EditorGUILayout.Vector3Field("Min", settings.scaleMin);
				settings.scaleMax = EditorGUILayout.Vector3Field("Max", settings.scaleMax);
				PresetButtons(ref settings.scaleMin, ref settings.scaleMax, "N", ".5", Vector2.one, new Vector2(0.5f, 1.5f));
			}
			else
			{
				settings.scaleMin = Vector3.one * Mathf.Max(EditorGUILayout.FloatField("Min", settings.scaleMin.x), 0);
				settings.scaleMax = Vector3.one * Mathf.Max(EditorGUILayout.FloatField("Max", settings.scaleMax.x), 0);
			}
			settings.scaleAdditive = EditorGUILayout.Toggle("Additive", settings.scaleAdditive);

			EditorGUILayout.Separator();
			GUI.enabled = true;
			GUILayout.Label("Selected GameObjects: " + Selection.gameObjects.Length);
			GUI.enabled = CanApply;
			if(GUILayout.Button("Apply"))
			{
				Apply();
			}
		}

		private void PresetButtons(ref Vector3 targetMin, ref Vector3 targetMax, string option1, string option2, Vector2 func1, Vector2 func2)
		{
			var rect = EditorGUILayout.GetControlRect();
			rect.xMin += EditorGUIUtility.labelWidth;
			var div = rect.DivideHorizontal(3);
			for(int i = 0; i < 3; i++)
			{
				div[i].xMin += 12;
				div[i].SplitHorizontalRelative(0.5f, out var lr, out var rr);
				if(GUI.Button(lr, option1, EditorStyles.miniButtonLeft))
				{
					targetMin[i] = func1.x;
					targetMax[i] = func1.y;
				}
				if(GUI.Button(rr, option2, EditorStyles.miniButtonRight))
				{
					targetMin[i] = func2.x;
					targetMax[i] = func2.y;
				}
			}
		}

		private void Apply()
		{
			foreach(var go in Selection.gameObjects)
			{
				var transform = go.transform;
				Undo.RecordObject(transform, "Randomize Transform");
				if(settings.enablePosition)
				{
					Vector3 translation = new Vector3(
						Random.Range(settings.positionMin.x, settings.positionMax.x),
						Random.Range(settings.positionMin.y, settings.positionMax.y),
						Random.Range(settings.positionMin.z, settings.positionMax.z)
					);
					transform.Translate(translation, settings.translationSpace);
				}
				if(settings.enableRotation)
				{
					Vector3 rotation = new Vector3(
						Random.Range(settings.rotationMin.x, settings.rotationMax.x),
						Random.Range(settings.rotationMin.y, settings.rotationMax.y),
						Random.Range(settings.rotationMin.z, settings.rotationMax.z)
					);
					transform.Rotate(rotation, settings.rotationSpace);
				}
				if(settings.enableScale)
				{
					Vector3 scale = settings.scaleAdditive ? transform.localScale : Vector3.one;
					if(settings.scaleNonUniform)
					{
						scale.x *= Random.Range(settings.scaleMin.x, settings.scaleMax.x);
						scale.y *= Random.Range(settings.scaleMin.y, settings.scaleMax.y);
						scale.z *= Random.Range(settings.scaleMin.z, settings.scaleMax.z);
					}
					else
					{
						scale *= Random.Range(settings.scaleMin.x, settings.scaleMax.x);
					}
					transform.localScale = scale;
				}
			}
		}
	}
}
