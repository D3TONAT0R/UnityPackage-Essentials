#if UNITY_6000_3_OR_NEWER
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public class RecompileToolbarButton
	{
		private const string MENU_PATH = "Editor Controls/Recompile Scripts";

		[MainToolbarElement(MENU_PATH, defaultDockPosition = MainToolbarDockPosition.Middle)]
		public static MainToolbarElement Create()
		{
			var icon = EditorGUIUtility.IconContent("d_Refresh").image as Texture2D;
			var content = new MainToolbarContent("", icon, "Recompile Scripts");
			return new MainToolbarButton(content, Recompile);
		}

		private static void Recompile()
		{
			UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
		}
	}
}
#endif