using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor.Tools
{
#if UNITY_2021_2_OR_NEWER
	[UnityEditor.Overlays.Overlay(typeof(SceneView), "Scene View Screenshots")]
	internal class SceneViewScreenshotToolOverlay : ToolOverlayBase<SceneViewScreenshotTool>
	{
		
	}
#endif
	
	[EditorTool("Scene View Screenshots")]
	public class SceneViewScreenshotTool : EditorToolBase
	{
		public bool useCustomResolution = false;
		public Vector2Int customResolution = new Vector2Int(1920, 1080);
		public bool drawGizmos = true;

		private static FieldInfo s_SceneTargetTextureField;
		private static EventInfo s_OnGUIEndedEvent;
		private static Delegate s_OnGUIEndedHandler;
		private static SceneView s_PendingCaptureSceneView;
		private static bool s_PendingCapture;
		private static bool s_Hooked;
		
		public override bool ShowToolWindow => true;

		public override void OnWindowGUI()
		{
			useCustomResolution = EditorGUILayout.Toggle("Use Custom Resolution", useCustomResolution);
			if (useCustomResolution)
			{
				customResolution = EditorGUILayout.Vector2IntField("Custom Resolution", customResolution);
				customResolution.x = Mathf.Clamp(customResolution.x, 1, 8192);
				customResolution.y = Mathf.Clamp(customResolution.y, 1, 8192);
			}
			GUI.enabled = !useCustomResolution;
			if (useCustomResolution) drawGizmos = false;
			drawGizmos = EditorGUILayout.Toggle("Draw Gizmos", drawGizmos);
			GUI.enabled = true;
			if(GUILayout.Button("Take Screenshot"))
			{
				TakeScreenshot();
			}
		}

		private void TakeScreenshot()
		{
			SceneView sceneView = SceneView.lastActiveSceneView;
			if(sceneView == null || sceneView.camera == null)
			{
				Debug.LogWarning("No active Scene view camera is available for screenshot capture.");
				return;
			}

			if (!drawGizmos)
			{
				Camera sceneCamera = sceneView.camera;
				int width = Mathf.Max(1, useCustomResolution ? customResolution.x : sceneCamera.pixelWidth);
				int height = Mathf.Max(1, useCustomResolution ? customResolution.y : sceneCamera.pixelHeight);

				RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
				RenderTexture previousCameraTargetTexture = sceneCamera.targetTexture;
				
				sceneCamera.targetTexture = renderTexture;
				sceneCamera.Render();
				sceneCamera.targetTexture = previousCameraTargetTexture;

				RenderTexture previousActiveRenderTexture = RenderTexture.active;
				SaveRenderTextureToPNG(renderTexture, true, previousActiveRenderTexture);
				return;
			}

			EnsureSceneViewHook();
			s_PendingCaptureSceneView = sceneView;
			s_PendingCapture = true;
			sceneView.Repaint();
		}

		private static void EnsureSceneViewHook()
		{
			if (s_SceneTargetTextureField == null)
				s_SceneTargetTextureField = typeof(SceneView).GetField("m_SceneTargetTexture", BindingFlags.NonPublic | BindingFlags.Instance);

			if (s_OnGUIEndedEvent == null)
				s_OnGUIEndedEvent = typeof(SceneView).GetEvent("onGUIEnded", BindingFlags.NonPublic | BindingFlags.Static);

			if (s_OnGUIEndedEvent == null || s_Hooked)
				return;

			MethodInfo handlerMethod = typeof(SceneViewScreenshotTool).GetMethod(nameof(OnSceneViewGUIEndedCapture), BindingFlags.NonPublic | BindingFlags.Static);
			if (handlerMethod == null)
				return;
			s_OnGUIEndedHandler = Delegate.CreateDelegate(s_OnGUIEndedEvent.EventHandlerType, handlerMethod);
			s_OnGUIEndedEvent.AddEventHandler(null, s_OnGUIEndedHandler);
			s_Hooked = true;
		}

		private static void OnSceneViewGUIEndedCapture(SceneView sceneView)
		{
			if (!s_PendingCapture || sceneView != s_PendingCaptureSceneView)
				return;
			if (Event.current == null || Event.current.type != EventType.Repaint)
			{
				return;
			}

			if (s_SceneTargetTextureField == null)
				s_SceneTargetTextureField = typeof(SceneView).GetField("m_SceneTargetTexture", BindingFlags.NonPublic | BindingFlags.Instance);

			RenderTexture sceneTargetTexture = s_SceneTargetTextureField != null
				? s_SceneTargetTextureField.GetValue(sceneView) as RenderTexture
				: null;
			if (sceneTargetTexture == null || sceneTargetTexture.width <= 0 || sceneTargetTexture.height <= 0)
			{
				Debug.LogWarning("SceneView render texture was not ready during repaint capture.");
				ClearPendingCapture();
				return;
			}

			RenderTexture previousActive = RenderTexture.active;
			try
			{
				SaveRenderTextureToPNG(sceneTargetTexture, false, previousActive);
			}
			finally
			{
				ClearPendingCapture();
			}
		}

		private static void ClearPendingCapture()
		{
			s_PendingCaptureSceneView = null;
			s_PendingCapture = false;
			if (s_Hooked && s_OnGUIEndedEvent != null && s_OnGUIEndedHandler != null)
			{
				s_OnGUIEndedEvent.RemoveEventHandler(null, s_OnGUIEndedHandler);
				s_Hooked = false;
				s_OnGUIEndedHandler = null;
			}
		}

		// Helper: read a RenderTexture into a Texture2D and prompt the user to save it as PNG.
		private static void SaveRenderTextureToPNG(RenderTexture renderTexture, bool releaseWhenDone, RenderTexture previousActiveRT)
		{
			if (renderTexture == null)
			{
				Debug.LogWarning("No render texture available for saving.");
				return;
			}
			RenderTexture.active = renderTexture;
			Texture2D sceneViewTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
			sceneViewTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
			sceneViewTexture.Apply();
			// Save file as prompt
			var fileName = $"SceneView_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
			var projectRoot = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
			var path = EditorUtility.SaveFilePanel("Save Screenshot", projectRoot, fileName, "png");
			if (!string.IsNullOrEmpty(path))
			{
				File.WriteAllBytes(path, sceneViewTexture.EncodeToPNG());
				Debug.Log($"Screenshot saved to: {path}");
			}
			// restore previous active RT and release temporary if requested
			RenderTexture.active = previousActiveRT;
			if (releaseWhenDone) RenderTexture.ReleaseTemporary(renderTexture);
		}

		protected override void OnSceneGUI(EditorWindow window, bool enableInteraction)
		{
		}
	}
}
