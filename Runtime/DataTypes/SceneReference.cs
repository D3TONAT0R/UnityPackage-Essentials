using UnityEditor;
using UnityEngine;

namespace D3T
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
		private string sceneName;
		[SerializeField, HideInInspector]
		private int buildIndex;

		/// <summary>
		/// <see langword="true"/> if the scene reference points to a scene present in the build settings.
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
				ResolveIfRequired();
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
				ResolveIfRequired();
#endif
				return buildIndex;
			}
		}

#if UNITY_EDITOR

		public void OnBeforeSerialize()
		{
			ResolveIfRequired();
		}

		public void OnAfterDeserialize()
		{
			
		}

		private void ResolveIfRequired()
		{
			if(resolvedInEditor) return;
			if(sceneAsset)
			{
				sceneName = sceneAsset.name;
				buildIndex = -1;
				var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sceneAsset));
				for(int i = 0; i < EditorBuildSettings.scenes.Length; i++)
				{
					if(guid == EditorBuildSettings.scenes[i].guid.ToString())
					{
						buildIndex = i;
					}
				}
			}
			else
			{
				sceneName = null;
				buildIndex = -1;
			}
			resolvedInEditor = true;
		}
#endif
	}
}
