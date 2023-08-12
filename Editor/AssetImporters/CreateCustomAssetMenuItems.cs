using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	internal static class CreateCustomAssetMenuItems
	{
		[MenuItem("Assets/Create/Texture2D/Gradient Texture")]
		public static void CreateGradientTextureAsset() => CreateAsset("New Gradient Texture.gradient");

		[MenuItem("Assets/Create/Texture2D/Checker Texture")]
		public static void CreateCheckerTextureAsset() => CreateAsset("New Checker Texture.checker");

		public static void CreateAsset(string newFileName)
		{
			/*
			string path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if(path == "")
			{
				path = "Assets";
			}
			else if(Path.GetExtension(path) != "")
			{
				path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
			}
			*/
			//TODO: copy from template files instead of creating a dummy object
			var newObj = new TextAsset();
			//path = AssetDatabase.GenerateUniqueAssetPath(path + "/" + newFileName);
			ProjectWindowUtil.CreateAsset(newObj, newFileName);
		}
	}
}
