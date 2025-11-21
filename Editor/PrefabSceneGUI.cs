using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor
{
	public class PrefabSceneGUI
	{
		private static GUIContent backIcon;
		private static GUIContent frontIcon;
		private static GUIStyle box;
		private static GUIStyle centerLabel;

		[InitializeOnLoadMethod]
		private static void Init()
		{
			SceneView.duringSceneGui += OnSceneGUI;
		}

		//TODO: Exclude subfolders when searching for prefabs in the same folder
		private static void OnSceneGUI(SceneView obj)
		{
			if(PrefabStageUtility.GetCurrentPrefabStage() == null) return;
			if(backIcon == null)
			{
				backIcon = EditorGUIUtility.IconContent("d_back");
				frontIcon = EditorGUIUtility.IconContent("d_forward");
				box = "FrameBox";
				centerLabel = new GUIStyle(EditorStyles.boldLabel)
				{
					alignment = TextAnchor.MiddleCenter
				};
			}
			Handles.BeginGUI();
			var x = obj.cameraViewport.width / 2;
			var w = 300;
			var h = 30;
			var rect = new Rect(x - w / 2, 0, w, h);
			GUILayout.BeginArea(rect);
			GUILayout.BeginHorizontal(box, GUILayout.ExpandHeight(true));
			string currentPrefab = PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot.name;
			if(GUILayout.Button(backIcon, GUILayout.ExpandWidth(false), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
			{
				PreviousPrefab();
			}
			//GUILayout.FlexibleSpace();
			if(GUILayout.Button(currentPrefab, centerLabel, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
			{
				// Select and ping the current prefab asset
				var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
				var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.assetPath);
				Selection.activeObject = prefabAsset;
				EditorGUIUtility.PingObject(prefabAsset);
			}
			//GUILayout.FlexibleSpace();
			if(GUILayout.Button(frontIcon, GUILayout.ExpandWidth(false), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
			{
				NextPrefab();
			}
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.EndArea();
			Handles.EndGUI();
		}

		private static void PreviousPrefab()
		{
			var prefabs = ListPrefabsGUIDsInSameFolder();
			var index = prefabs.IndexOf(AssetDatabase.AssetPathToGUID(PrefabStageUtility.GetCurrentPrefabStage().assetPath));
			if(index == -1) return;
			index = (index + prefabs.Count - 1) % prefabs.Count;
			OpenPrefabByGUID(prefabs[index]);
		}

		private static void NextPrefab()
		{
			var prefabs = ListPrefabsGUIDsInSameFolder();
			var index = prefabs.IndexOf(AssetDatabase.AssetPathToGUID(PrefabStageUtility.GetCurrentPrefabStage().assetPath));
			if(index == -1) return;
			index = (index + 1) % prefabs.Count;
			OpenPrefabByGUID(prefabs[index]);
		}

		private static List<string> ListPrefabsGUIDsInSameFolder()
		{
			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			var prefabPath = prefabStage.assetPath;
			var folderPath = System.IO.Path.GetDirectoryName(prefabPath);
			var guid = AssetDatabase.AssetPathToGUID(prefabPath);
			return AssetDatabase.FindAssets("t:Prefab", new[] { folderPath }).ToList();
		}

		private static void OpenPrefabByGUID(string guid)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			PrefabStageUtility.OpenPrefab(path);
		}
	}
}