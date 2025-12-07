#if UNITY_6000_3_OR_NEWER
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEssentialsEditor
{
	public class SceneSelectionToolbarDropdown
	{
		private const string MENU_PATH = "Editor Controls/Scene Selector";
		private static string[] scenePaths;

		[MainToolbarElement(MENU_PATH, defaultDockPosition = MainToolbarDockPosition.Middle)]
		public static MainToolbarElement CreateSceneSelectorDropdown()
		{
			var icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
			var content = new MainToolbarContent("Scenes", icon, "Select active scene");
			var dropdown = new MainToolbarDropdown(content, ShowDropdownMenu);
			return dropdown;
		}

		static void ShowDropdownMenu(Rect dropDownRect)
		{
			var menu = new GenericMenu();
			if (scenePaths.Length == 0)
			{
				menu.AddDisabledItem(new GUIContent("No Scenes in Project"));
			}
			if (EditorBuildSettings.scenes.Length > 0)
			{
				for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
				{
					var scene = EditorBuildSettings.scenes[i];
					string sceneName = Path.GetFileNameWithoutExtension(scene.path);
					menu.AddItem(new GUIContent("Build Scenes/ " + sceneName), false, () => { SwitchScene(scene.path); });
				}
				menu.AddSeparator("");
			}
			foreach (string scenePath in scenePaths)
			{
				string sceneName = Path.GetFileNameWithoutExtension(scenePath);
				menu.AddItem(new GUIContent(sceneName), false, () => { SwitchScene(scenePath); });
			}
			menu.DropDown(dropDownRect);
		}

		static void SwitchScene(string scenePath)
		{
			if (Application.isPlaying)
			{
				string sceneName = Path.GetFileNameWithoutExtension(scenePath);
				if (Application.CanStreamedLevelBeLoaded(sceneName))
				{
					Debug.Log($"Switching to scene: {sceneName}");
					SceneManager.LoadScene(sceneName);
				}
				else
				{
					Debug.LogError($"Scene '{sceneName}' is not in the Build Settings.");
				}
			}
			else
			{
				if (File.Exists(scenePath))
				{
					if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
					{
						EditorSceneManager.OpenScene(scenePath);
					}
				}
				else
				{
					Debug.LogError($"Scene at path '{scenePath}' does not exist.");
				}
			}
		}

		static void RefreshSceneList()
		{
			scenePaths = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);
		}

		static void SceneSwitched(Scene oldScene, Scene newScene)
		{
			MainToolbar.Refresh(MENU_PATH);
		}

		static SceneSelectionToolbarDropdown()
		{
			RefreshSceneList();
			EditorApplication.projectChanged += RefreshSceneList;
			SceneManager.activeSceneChanged += SceneSwitched;
			EditorSceneManager.activeSceneChangedInEditMode += SceneSwitched;
		}
	}
}
#endif