using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor
{
	public class PrefabSceneGUI
	{
		private class PrefabRef
		{
			public string name;
			public string guid;

			public PrefabRef(string guid)
			{
				this.guid = guid;
				name = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid));
			}
		}
		
#if UNITY_2021_2_OR_NEWER
		private static GUIContent backIcon;
		private static GUIContent forwardIcon;
		private static GUIStyle box;
		private static GUIStyle centerLabel;

		private static PrefabStage currentPrefabStage;
		private static string infoText;

		private static PrefabRef previousPrefab;
		private static PrefabRef nextPrefab;

		[InitializeOnLoadMethod]
		private static void Init()
		{
			SceneView.duringSceneGui += OnSceneGUI;
			PrefabStage.prefabStageOpened += OnPrefabStageOpened;
			PrefabStage.prefabStageClosing += OnPrefabStageClosing;
			var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
			if (currentStage != null)
			{
				OnPrefabStageOpened(currentStage);
			}
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
				var prefabs = ListPrefabsGUIDsInSameFolder();
				var index = prefabs.IndexOf(AssetDatabase.AssetPathToGUID(PrefabStageUtility.GetCurrentPrefabStage().assetPath));
				previousPrefab = null;
				nextPrefab = null;
				if (index > -1 && prefabs.Count > 1)
				{
					var previousIndex = (index + prefabs.Count - 1) % prefabs.Count;
					var nextIndex = (index + 1) % prefabs.Count;
					previousPrefab = new PrefabRef(prefabs[previousIndex]);
					nextPrefab = new PrefabRef(prefabs[nextIndex]);
				}
			};
		}

		private static void OnPrefabStageClosing(PrefabStage obj)
		{
			currentPrefabStage = null;
		}

		private static void OnSceneGUI(SceneView obj)
		{
			if (!EssentialsProjectSettings.Instance.showPrefabStageSceneGUI || currentPrefabStage == null) return;
			try
			{
				if (backIcon == null)
				{
					backIcon = EditorGUIUtility.IconContent("d_back");
					forwardIcon = EditorGUIUtility.IconContent("d_forward");
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
				//TODO: tooltips are not positioned correctly
				GUI.enabled = previousPrefab != null;
				backIcon.tooltip = "Previous Prefab: " + previousPrefab?.name ?? "";
				if (GUI.Button(prevBtnPos, backIcon))
				{
					PreviousPrefab();
				}
				GUI.enabled = nextPrefab != null;
				forwardIcon.tooltip = "Next Prefab: " + nextPrefab?.name ?? "";
				if (GUI.Button(nextBtnPos, forwardIcon))
				{
					NextPrefab();
				}
				GUI.enabled = true;
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
			if(previousPrefab != null) OpenPrefabByGUID(previousPrefab.guid);
		}

		private static void NextPrefab()
		{
			if(nextPrefab != null) OpenPrefabByGUID(nextPrefab.guid);
		}

		private static List<string> ListPrefabsGUIDsInSameFolder()
		{
			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			var prefabPath = prefabStage.assetPath;
			var folderPath = Path.GetDirectoryName(prefabPath);
			return AssetDatabase.FindAssets("t:Prefab", new[] { folderPath })
				.Where(guid => CheckSameDirectory(folderPath, guid))
				.OrderBy(guid => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid)))
				.ToList();
		}

		private static bool CheckSameDirectory(string directory, string assetGUID)
		{
			return System.IO.Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(assetGUID)) == directory;
		}

		private static void OpenPrefabByGUID(string guid)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			if (PrefabStageUtility.GetCurrentPrefabStage() != null)
			{
				StageUtility.GoBackToPreviousStage();
			}
			PrefabStageUtility.OpenPrefab(path);
			SceneView.RepaintAll();
		}
#endif
	}
}