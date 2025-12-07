#if UNITY_6000_3_OR_NEWER
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public class ErrorPauseToolbarToggle
	{
		public static readonly Type consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
		
		private const string MENU_PATH = "Editor Controls/Error Pause Toggle";

		[MainToolbarElement(MENU_PATH, defaultDockPosition = MainToolbarDockPosition.Middle)]
		public static MainToolbarElement CreateSceneSelectorDropdown()
		{
			var icon = EditorGUIUtility.IconContent("d_console.erroricon.inactive.sml").image as Texture2D;
			var content = new MainToolbarContent("", icon, "Click to toggle error pause");
			var dropdown = new MainToolbarToggle(content, GetErrorPause(), SetErrorPause);
			return dropdown;
		}
		
		private static bool GetErrorPause()
		{
			const BindingFlags PUBLIC_STATIC_FLAGS = BindingFlags.Static | BindingFlags.Public;
			var getMethod = consoleWindowType.GetMethod("GetConsoleErrorPause", PUBLIC_STATIC_FLAGS);
			return getMethod != null && (bool)getMethod.Invoke(null, null);
		}
		
		private static void SetErrorPause(bool state)
		{
			const BindingFlags PUBLIC_STATIC_FLAGS = BindingFlags.Static | BindingFlags.Public;
			var setMethod = consoleWindowType.GetMethod("SetConsoleErrorPause", PUBLIC_STATIC_FLAGS);
			if (setMethod != null) setMethod.Invoke(null, new object[] { state });
			MainToolbar.Refresh(MENU_PATH);
			if (Resources.FindObjectsOfTypeAll(consoleWindowType).Length > 0)
			{
				EditorWindow.GetWindow(consoleWindowType, false, null, false)?.Repaint();
			}
		}
	}
}
#endif