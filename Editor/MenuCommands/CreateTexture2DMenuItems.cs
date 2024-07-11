using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.ProjectWindowCallback;

namespace D3TEditor
{
	internal static class CreateTexture2DMenuItems
	{
		private const int PRIORITY = 303;

		[MenuItem("Assets/Create/Texture2D/Blank PSD Texture", priority = PRIORITY)]
		public static void CreateBlankPSD()
		{
			CreateBlankAsset("psd", "New PSD Texture");
		}

		[MenuItem("Assets/Create/Texture2D/Blank PNG Texture", priority = PRIORITY)]
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
