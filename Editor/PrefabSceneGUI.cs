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
		
		private static PrefabStage currentPrefabStage;
		private static string infoText;

		[InitializeOnLoadMethod]
		private static void Init()
		{
			SceneView.duringSceneGui += OnSceneGUI;
			PrefabStage.prefabStageOpened += OnPrefabStageOpened;
			PrefabStage.prefabStageClosing += OnPrefabStageClosing;
		}

		private static void OnPrefabStageOpened(PrefabStage stage)
		{
			EditorApplication.delayCall += () =>
			{
				currentPrefabStage = stage;
				bool variant = PrefabUtility.GetPrefabAssetType(currentPrefabStage.prefabContentsRoot) != PrefabAssetType.NotAPrefab;
				if (variant)
				{
					infoText = "Variant of " + PrefabUtility.GetCorrespondingObjectFromSource(currentPrefabStage.prefabContentsRoot).name;
				}
				else
				{
					infoText = "Prefab Asset";
				}
			};
		}

		private static void OnPrefabStageClosing(PrefabStage obj)
		{
			currentPrefabStage = null;
		}

		private static void OnSceneGUI(SceneView obj)
		{
			if(!EssentialsProjectSettings.Instance.showPrefabStageGUI || currentPrefabStage == null) return;
			try
			{
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
				string currentPrefab = currentPrefabStage.prefabContentsRoot.name;
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
				GUI.Label(infoPos, infoText, EditorStyles.centeredGreyMiniLabel);
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