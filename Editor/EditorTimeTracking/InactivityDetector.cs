using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.TimeTracking
{
	public static class InactivityDetector
	{
		private const float INACTIVITY_THRESHOLD = 180;

		private static double lastActivityTime;
		private static Vector2Int lastMousePos;
		private static double lastCheckTime;

		public static double InactivityTime => EditorApplication.timeSinceStartup - lastActivityTime;

		public static bool UserIsInactive => InactivityTime > INACTIVITY_THRESHOLD;

		public static void ReportActive()
		{
			lastActivityTime = EditorApplication.timeSinceStartup;
		}

		[InitializeOnLoadMethod]
		private static void Init()
		{
			EditorApplication.update += CheckForMouseMovement;
		}

		private static void CheckForMouseMovement()
		{
			if(EditorApplication.timeSinceStartup < (lastCheckTime + 0.25f))
			{
				return;
			}
			lastCheckTime = EditorApplication.timeSinceStartup;
			Vector2Int mousePos = GetMousePosition();
			if(mousePos != lastMousePos || mousePos == Vector2Int.zero)
			{
				ReportActive();
			}
			lastMousePos = mousePos;
		}

		internal static void Update()
		{
			if(SceneView.lastActiveSceneView == null)
			{
				//No scene view open, unable to check for inactivity
				ReportActive();
			}
		}

#if UNITY_EDITOR_WIN
		//Windows related methods for getting mouse position

		[DllImport("user32.dll")]
		private static extern bool GetCursorPos(out Vector2Int lpPoint);

		private static Vector2Int GetMousePosition()
		{
			if(GetCursorPos(out Vector2Int point))
			{
				return new Vector2Int(point.x, point.y);
			}
			return Vector2Int.zero;
		}

#elif UNITY_EDITOR_OSX
		//MacOS related methods for getting mouse position

		[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
		private static extern IntPtr CGEventSourceCreate(int sourceStateID);

		[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
		private static extern IntPtr CGEventCreate(IntPtr source);

		[DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
		private static extern CGPoint CGEventGetLocation(IntPtr @event);

		[StructLayout(LayoutKind.Sequential)]
		private struct CGPoint
		{
			public double x;
			public double y;
		}

		private static Vector2Int GetMousePosition()
		{
			try
			{
				IntPtr eventSource = CGEventSourceCreate(0);
				IntPtr @event = CGEventCreate(eventSource);
				CGPoint point = CGEventGetLocation(@event);
				// Convert the CGPoint to a Vector2Int
				Vector2Int mousePosition = new Vector2Int((int)point.x, (int)point.y);
				// Release the event source
				Marshal.Release(eventSource);
				Marshal.Release(@event);
				return mousePosition;
			}
			catch
			{
				return Vector2Int.zero;
			}
		}
#else
		//Other platforms, unable to track mouse
		private static Vector2Int GetMousePosition() {
			return Vector2Int.zero;
		}
#endif
	}
}
