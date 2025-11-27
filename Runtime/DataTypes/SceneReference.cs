using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEssentials
{
	/// <summary>
	/// Represents a reference to a scene asset.
	/// </summary>
	[System.Serializable]
	public class SceneReference
#if UNITY_EDITOR
		: ISerializationCallbackReceiver
#endif
	{
#if UNITY_EDITOR
		[SerializeField]
		private UnityEditor.SceneAsset sceneAsset;
		private bool resolvedInEditor = false;
#endif

		[SerializeField, HideInInspector]
		private string sceneName;
		[SerializeField, HideInInspector]
		private int buildIndex;

		/// <summary>
		/// Returns true if a scene is referenced.
		/// </summary>
		public bool Exists => !string.IsNullOrWhiteSpace(SceneName);

		/// <summary>
		/// Returns true if a valid scene is referenced that is present in the build settings.
		/// </summary>
		public bool ExistsInBuild => BuildIndex >= 0;

		/// <summary>
		/// The name of the scene this reference points to.
		/// </summary>
		public string SceneName
		{
			get
			{
#if UNITY_EDITOR
				EditorResolveIfRequired(false);
#endif
				return !string.IsNullOrEmpty(sceneName) ? sceneName : null;
			}
		}

		/// <summary>
		/// The build index of the scene this reference points to.
		/// </summary>
		public int BuildIndex
		{
			get
			{
#if UNITY_EDITOR
				EditorResolveIfRequired(false);
#endif
				return buildIndex;
			}
		}

		/// <summary>
		/// Creates a new scene reference from the given scene name.
		/// </summary>
		public SceneReference(string sceneName)
		{
			this.sceneName = sceneName;
			buildIndex = -1;
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				var name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
				if (name == sceneName)
				{
					buildIndex = i;
					break;
				}
			}
#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying) resolvedInEditor = true;
#endif
		}

		/// <summary>
		/// Creates a new scene reference from the given build index.
		/// </summary>
		public SceneReference(int buildIndex)
		{
			if (buildIndex >= SceneManager.sceneCountInBuildSettings)
			{
				throw new System.ArgumentOutOfRangeException(nameof(buildIndex),
					$"Build index out of range: {buildIndex}. Scene count: {SceneManager.sceneCountInBuildSettings}");
			}
			this.buildIndex = buildIndex;
			sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(buildIndex));
#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying) resolvedInEditor = true;
#endif
		}

		/// <summary>
		/// Creates a new scene reference from the given scene.
		/// </summary>
		public SceneReference(Scene scene)
		{
			if (scene.IsValid())
			{
				sceneName = scene.name;
				buildIndex = scene.buildIndex;
			}
			else
			{
				sceneName = "";
				buildIndex = -1;
			}
#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying) resolvedInEditor = true;
#endif
		}

		/// <summary>
		/// Loads the referenced scene.
		/// </summary>
		public void Load(LoadSceneMode mode = LoadSceneMode.Single)
		{
			BeforeLoadCheck();
			SceneManager.LoadScene(BuildIndex, mode);
		}

		/// <summary>
		/// Loads the referenced scene.
		/// </summary>
		public void Load(LoadSceneParameters parameters)
		{
			BeforeLoadCheck();
			SceneManager.LoadScene(BuildIndex, parameters);
		}

		/// <summary>
		/// Loads the referenced scene asynchronously.
		/// </summary>
		public AsyncOperation LoadAsync(LoadSceneMode mode = LoadSceneMode.Single)
		{
			BeforeLoadCheck();
			return SceneManager.LoadSceneAsync(BuildIndex, mode);
		}

		/// <summary>
		/// Loads the referenced scene asynchronously.
		/// </summary>
		public AsyncOperation LoadAsync(LoadSceneParameters parameters)
		{
			BeforeLoadCheck();
			return SceneManager.LoadSceneAsync(BuildIndex, parameters);
		}

		/// <summary>
		/// Unloads the referenced scene asynchronously.
		/// </summary>
		public AsyncOperation UnloadAsync()
		{
			BeforeLoadCheck(true);
			return SceneManager.UnloadSceneAsync(BuildIndex);
		}

		private void BeforeLoadCheck(bool unload = false)
		{
#if UNITY_EDITOR
			EditorResolveIfRequired(true);
#endif
			string action = unload ? "unload" : "load";
			if (!Exists)
			{
				throw new System.InvalidOperationException($"Can not {action} scene because no scene is referenced.");
			}
			if (!ExistsInBuild)
			{
				throw new System.InvalidOperationException($"Can not {action} scene because the scene '{SceneName}' is not in build settings.");
			}
		}

#if UNITY_EDITOR

		public void OnBeforeSerialize()
		{
			EditorResolveIfRequired(false);
		}

		public void OnAfterDeserialize()
		{
		}

		private void EditorResolve()
		{
			if (sceneAsset)
			{
				sceneName = sceneAsset.name;
				buildIndex = SceneUtility.GetBuildIndexByScenePath(UnityEditor.AssetDatabase.GetAssetPath(sceneAsset));
			}
			else
			{
				sceneName = "";
				buildIndex = -1;
			}
			resolvedInEditor = true;
		}

		private void EditorResolveIfRequired(bool force)
		{
			// Don't resolve during play mode but force it during builds
			bool resolveRequired = force || !resolvedInEditor || UnityEditor.BuildPipeline.isBuildingPlayer;
			if (!resolveRequired) return;
			// if(sceneAsset ? sceneAsset.name == sceneName : sceneName == "") return;
			EditorResolve();
		}
#endif

		public static bool operator ==(SceneReference a, SceneReference b)
		{
			if (ReferenceEquals(a, b)) return true;
			if (a is null || b is null) return false;
			return a.SceneName == b.SceneName && a.BuildIndex == b.BuildIndex;
		}

		public static bool operator !=(SceneReference a, SceneReference b)
		{
			return !(a == b);
		}

		public static implicit operator bool(SceneReference sceneReference)
		{
			return sceneReference != null && sceneReference.Exists;
		}

		public override int GetHashCode()
		{
			if (buildIndex >= 0)
			{
				int sceneHash = sceneName?.GetHashCode() ?? 0;
				return (sceneHash * 397) ^ buildIndex;
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is SceneReference other)
			{
				return this == other;
			}
			return false;
		}
	}
}