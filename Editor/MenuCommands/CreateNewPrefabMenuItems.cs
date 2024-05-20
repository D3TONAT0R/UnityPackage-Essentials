using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	internal static class CreateNewPrefabMenuItems
	{
#if !UNITY_2020_1_OR_NEWER
		[MenuItem("Assets/Create/Empty Prefab", priority = 302)]
		public static void CreateBlankPrefab()
		{
			string newPath = AssetDatabase.GenerateUniqueAssetPath(GetSelectedPathOrFallback() + "/" + "New Prefab" + ".prefab");
			var prefabGameObject = new GameObject("New Prefab");
			PrefabUtility.SaveAsPrefabAsset(prefabGameObject, newPath);
			Object.DestroyImmediate(prefabGameObject);
			Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(newPath);
		}

		static string GetSelectedPathOrFallback()
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
#endif
	}
}