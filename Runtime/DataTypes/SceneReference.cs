using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEssentials
{
	/// <summary>
	/// Represents a reference to a scene asset.
	/// </summary>
	[System.Serializable]
#if UNITY_EDITOR
	public class SceneReference : ISerializationCallbackReceiver
#else
	public class SceneReference
#endif
	{

#if UNITY_EDITOR
		[SerializeField]
		private UnityEditor.SceneAsset sceneAsset;

		private bool resolvedInEditor = false;
#endif
		[SerializeField, HideInInspector]
		private string sceneName = "";
		[SerializeField, HideInInspector]
		private int buildIndex = -1;

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
				EditorResolveIfRequired();
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
				EditorResolveIfRequired();
#endif
				return buildIndex;
			}
		}

		public void Load(LoadSceneMode mode = LoadSceneMode.Single)
		{
			if(!BeforeLoadCheck()) return;
			SceneManager.LoadScene(BuildIndex, mode);
		}

		public void Load(LoadSceneParameters parameters)
		{
			if(!BeforeLoadCheck()) return;
			SceneManager.LoadScene(BuildIndex, parameters);
		}

		public AsyncOperation LoadAsync(LoadSceneMode mode = LoadSceneMode.Single)
		{
			if(!BeforeLoadCheck()) return null;
			return SceneManager.LoadSceneAsync(BuildIndex, mode);
		}

		public AsyncOperation LoadAsync(LoadSceneParameters parameters)
		{
			if(!BeforeLoadCheck()) return null;
			return SceneManager.LoadSceneAsync(BuildIndex, parameters);
		}

		private bool BeforeLoadCheck()
		{
			if(!Exists)
			{
				Debug.LogError("Could not load scene because no scene is referenced.");
				return false;
			}
			if(!ExistsInBuild)
			{
				Debug.LogError($"Could not load scene because the scene '{SceneName}' is not in build settings.");
				return false;
			}
			return true;
		}

#if UNITY_EDITOR

		public void OnBeforeSerialize()
		{
			EditorResolveIfRequired();
		}

		public void OnAfterDeserialize()
		{
			
		}

		private void EditorResolve()
		{
			if(sceneAsset)
			{
				sceneName = sceneAsset.name;
				buildIndex = -1;
				var guid = UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(sceneAsset));
				for(int i = 0; i < UnityEditor.EditorBuildSettings.scenes.Length; i++)
				{
					if(guid == UnityEditor.EditorBuildSettings.scenes[i].guid.ToString())
					{
						buildIndex = i;
					}
				}
			}
			else
			{
				sceneName = "";
				buildIndex = -1;
			}
			resolvedInEditor = true;
		}

		private void EditorResolveIfRequired()
		{
			if(Application.isPlaying && resolvedInEditor) return;
			if(sceneAsset ? sceneAsset.name == sceneName : sceneName == "") return;
			EditorResolve();
		}
#endif

		public static bool operator ==(SceneReference a, SceneReference b)
		{
			if(ReferenceEquals(a, b)) return true;
			if(a is null || b is null) return false;
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
			if(buildIndex >= 0)
			{
				int sceneHash = sceneName?.GetHashCode() ?? 0;
				return (sceneHash * 397) ^ buildIndex;
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			if(obj is SceneReference other)
			{
				return this == other;
			}
			return false;
		}
	}
}
