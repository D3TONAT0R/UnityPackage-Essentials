#if UNITY_6000_3_OR_NEWER
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public class MainToolbarEnterPlayModeOptionsSelector
	{
		private const string MENU_PATH = "Editor Controls/Enter Play Mode Options";

		[MainToolbarElement(MENU_PATH, defaultDockPosition = MainToolbarDockPosition.Left)]
		public static MainToolbarElement Create()
		{
			var icon = EditorGUIUtility.IconContent("d_preAudioAutoPlayOff").image as Texture2D;
			var content = new MainToolbarContent("", icon, "Enter Play Mode Options");
			var dropdown = new MainToolbarDropdown(content, OpenDropdown);
			return dropdown;
		}

		private static void OpenDropdown(Rect obj)
		{
			var menu = new GenericMenu();
			menu.AddItem("Enter Play Mode Options", false, false, null);
			menu.AddSeparator("");
			menu.AddItem("Reload Everything", true, EditorSettings.enterPlayModeOptions == EnterPlayModeOptions.None, 
				() => EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.None);
			menu.AddSeparator("");
			menu.AddItem("No Domain Reload", true, EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload),
				() => ToggleEnterPlaymodeOption(EnterPlayModeOptions.DisableDomainReload));
			menu.AddItem("No Scene Reload", true, EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableSceneReload),
				() => ToggleEnterPlaymodeOption(EnterPlayModeOptions.DisableSceneReload));
			menu.DropDown(obj);
		}

		private static void ToggleEnterPlaymodeOption(EnterPlayModeOptions option)
		{
			EditorSettings.enterPlayModeOptions ^= option;
		}
	}
}
#endif