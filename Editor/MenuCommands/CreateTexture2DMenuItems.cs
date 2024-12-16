using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	internal static class CreateTexture2DMenuItems
	{
		private const string MENU_ROOT = "Assets/Create/Texture/";

#if UNITY_6000_0_OR_NEWER
		private const int PRIORITY = -125;
#else
		private const int PRIORITY = 303;
#endif

		[MenuItem(MENU_ROOT + "Blank PSD Texture", priority = PRIORITY)]
		public static void CreateBlankPSD()
		{
			CreateBlankAsset("psd", "New PSD Texture");
		}

		[MenuItem(MENU_ROOT + "Blank PNG Texture", priority = PRIORITY)]
		public static void CreateBlankPNG()
		{
			CreateBlankAsset("png", "New PNG Texture");
		}

		private static void CreateBlankAsset(string extension, string newName)
		{
			string templatePath = $"Packages/com.github.d3tonat0r.essentials/Editor/TemplateAssets/template_{extension}.{extension}";
			var icon = EditorGUIUtility.IconContent("d_Texture2D Icon").image as Texture2D;
			AssetDatabaseUtility.BeginAssetCreationFromTemplateFile(templatePath, newName, icon);
		}
	}
}
