using System;
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
			try
			{
				var stage = PrefabStageUtility.GetCurrentPrefabStage();
				if (stage == null) return;
				if (backIcon == null)
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
				var xCenter = obj.cameraViewport.width / 2;
				var w = 300;
				var h = 30;
				var rect = new Rect(xCenter - w / 2, 0, w, h);
				GUI.BeginGroup(rect, box);
				var prevBtnPos = new Rect(5, 5, 25, 20);
				var nextBtnPos = new Rect(w - 30, 5, 25, 20);
				var namePos = new Rect(30, 0, w - 60, 20);
				var infoPos = new Rect(30, 15, w - 60, 15);
				string currentPrefab = stage.prefabContentsRoot.name;
				if (GUI.Button(prevBtnPos, backIcon))
				{
					PreviousPrefab();
				}
				if (GUI.Button(nextBtnPos, frontIcon))
				{
					NextPrefab();
				}
				if (GUI.Button(namePos, currentPrefab, centerLabel))
				{
					// Select and ping the current prefab asset
					var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
					var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.assetPath);
					Selection.activeObject = prefabAsset;
					EditorGUIUtility.PingObject(prefabAsset);
				}
				// Check whether it's a prefab variant
				Debug.Log(stage.openedFromInstanceRoot);
				bool variant = PrefabUtility.GetPrefabAssetType(stage.prefabContentsRoot) != PrefabAssetType.NotAPrefab;
				string text;
				if (variant)
				{
					text = "Variant of " + PrefabUtility.GetCorrespondingObjectFromSource(stage.prefabContentsRoot).name;
				}
				else
				{
					text = "Prefab Asset";
				}
				GUI.Label(infoPos, text, EditorStyles.centeredGreyMiniLabel);
				//GUILayout.FlexibleSpace();
				GUI.EndGroup();
				Handles.EndGUI();
			}
			catch (Exception e)
			{
				e.LogException();
			}
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
			return AssetDatabase.FindAssets("t:Prefab", new[] { folderPath })
				.Where(guid => CheckSameDirectory(folderPath, guid))
				.ToList();
		}

		private static bool CheckSameDirectory(string directory, string assetGUID)
		{
			return System.IO.Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(assetGUID)) == directory;
		}

		private static void OpenPrefabByGUID(string guid)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			PrefabStageUtility.OpenPrefab(path);
		}
	}
}