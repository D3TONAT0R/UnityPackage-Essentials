using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	public class AutoRefreshMenuItems
	{
		/*
		kAutoRefresh has two posible values
		0 = Auto Refresh Disabled
		1 = Auto Refresh Enabled
		*/

		const string menuItem = "Edit/Toggle Auto Asset Refresh";
		const string shortcut = " %#&r";

		//This is called when you click on the 'Tools/Auto Refresh' and toggles its value
		[MenuItem(menuItem + shortcut, false)]
		private static void AutoRefreshToggle()
		{
			var status = EditorPrefs.GetInt("kAutoRefresh");
			EditorPrefs.SetInt("kAutoRefresh", status == 0 ? 1 : 0);
			Debug.Log($"Automatic asset refresh / script recompilation has been {(status == 0 ? "enabled" : "disabled")}.");
		}


		//This is called before 'Tools/Auto Refresh' is shown to check the current value
		//of kAutoRefresh and update the checkmark
		[MenuItem(menuItem + shortcut, true)]
		private static bool AutoRefreshToggleValidation()
		{
			var status = EditorPrefs.GetInt("kAutoRefresh");
			Menu.SetChecked(menuItem, status > 0);
			return true;
		}

		[MenuItem("Edit/Recompile Scripts")]
		private static void RequestScriptCopmpilation()
		{
			UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
		}
	} 
}