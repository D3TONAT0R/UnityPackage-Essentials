using UnityEditor;
using UnityEngine;
using System.IO;

namespace UnityEssentialsEditor
{
	public static class CreateTexture2DMenuItems
	{

		[MenuItem("Assets/Create/Texture2D/Blank PSD Texture", priority = 301)]
		public static void CreateBlankPSD()
		{
			CreateBlankAsset("psd", "New PSD Texture");
		}

		[MenuItem("Assets/Create/Texture2D/Blank PNG Texture", priority = 301)]
		public static void CreateBlankPNG()
		{
			CreateBlankAsset("png", "New PNG Texture");
		}

		static void CreateBlankAsset(string extension, string newName)
		{
			string templatePath = $"Packages/com.github.d3tonat0r.essentials/Editor/TemplateAssets/template_{extension}.{extension}";
			string newPath = AssetDatabase.GenerateUniqueAssetPath(GetSelectedPathOrFallback() + "/" + newName + "." + extension);
			if (AssetDatabase.CopyAsset(templatePath, newPath))
			{
				Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture2D>(newPath);
				//TODO: start in 'rename' mode
			}
			else
			{
				Debug.LogError("Failed to create blank " + extension + " file.");
			}
		}

		static string GetSelectedPathOrFallback()
		{
			string path = "Assets";

			foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
			{
				path = AssetDatabase.GetAssetPath(obj);
				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					path = Path.GetDirectoryName(path);
					break;
				}
			}
			return path;
		}
	} 
}
