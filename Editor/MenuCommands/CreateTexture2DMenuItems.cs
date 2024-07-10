using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.ProjectWindowCallback;

namespace D3TEditor
{
	internal static class CreateTexture2DMenuItems
	{
		internal class CreateTexture2DAssetAction : EndNameEditAction
		{
			private string templateFile;

			public static CreateTexture2DAssetAction CreateAction(string templateFile)
			{
				var instance = CreateInstance<CreateTexture2DAssetAction>();
				instance.templateFile = templateFile;
				return instance;
			}

			public override void Action(int instanceId, string pathName, string resourceFile)
			{
				string finalPath = AssetDatabase.GenerateUniqueAssetPath(pathName + Path.GetExtension(templateFile));
				AssetDatabase.CopyAsset(templateFile, finalPath);
				var newAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(finalPath);
				ProjectWindowUtil.ShowCreatedAsset(newAsset);
			}
		}

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
			var icon = (EditorGUIUtility.IconContent("Texture2D Icon").image as Texture2D);
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateTexture2DAssetAction.CreateAction(templatePath), newName, icon, templatePath);
		}

		private static string GetSelectedPathOrFallback()
		{
			string path = "Assets";

			foreach(Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
			{
				path = AssetDatabase.GetAssetPath(obj);
				if(!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					path = Path.GetDirectoryName(path);
					break;
				}
			}
			return path;
		}
	}
}
