using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEssentials;

namespace UnityEssentialsEditor
{
	public static class ProjectWindowExtras
	{
		private static GUIStyle borderStyle;
		private static GUIContent prefabModeContent = new GUIContent("", "Currently editing prefab");

		[InitializeOnLoadMethod]
		private static void Init()
		{
			EditorApplication.projectWindowItemOnGUI = ProjectWindowItemOnGUI;
			var borderTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(Path.Combine(EssentialsPackageInfo.ROOT_PATH, "Editor", "EditorAssets", "border_box.png"));
			borderStyle = new GUIStyle
			{
				normal =
				{
					background = borderTexture
				},
				border = new RectOffset(4, 4, 4, 4)
			};
		}

		private static void ProjectWindowItemOnGUI(string guid, Rect pos)
		{
			if(Event.current.type != EventType.Repaint)
			{
				return;
			}
			// Check if we are in prefab mode
			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage != null)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				if (path == prefabStage.assetPath)
				{
					// Draw a border indicating we are in prefab mode
					bool listView = pos.height <= 32;
					if(listView)
					{
						pos = pos.Outset(1);
						pos.width -= 1;
					}
					else
					{
						// Add extra offset when in grid view
						pos = pos.Outset(4);
					}
					GUI.color = new Color(0.24f, 0.37f, 0.59f, 1f).Lighten(0.2f);
					GUI.Label(pos, prefabModeContent, borderStyle);
					// EditorGUI.DrawRect(pos, Color.red.WithAlpha(0.5f));
					GUI.color = Color.white;
				}
			}
		}
	}
}