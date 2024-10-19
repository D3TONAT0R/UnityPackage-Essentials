using D3T;
using System;
using UnityEngine;

namespace D3TEditor
{
	public class EssentialsProjectSettings : ProjectSettingsAsset
	{
		public override string ProjectAssetName => "EssentialsProjectSettings";

		public static EssentialsProjectSettings Instance
		{
			get
			{
				if(instance == null) instance = CreateInstance<EssentialsProjectSettings>();
				return instance;
			}
		}
		private static EssentialsProjectSettings instance;

		[Header("Scripts")]
		public bool removeDefaultScriptMenu = true;
		public bool useDefaultNamespace = true;
		[EnabledIf(nameof(useDefaultNamespace))]
		public string defaultScriptNamespace = "";
#if UNITY_2020_2_OR_NEWER
		[NonReorderable]
#endif
		public string[] additionalDefaultUsings = Array.Empty<string>();
		[Header("Menu Management")]
		public bool reorganizeAssetMenu = true;

		[Space(20)]
		public bool enableEditorTimeTracking = true;

		protected override void OnCreateNewSettings()
		{
			Debug.Log("Unity Essentials: Creating new project settings asset.");
		}
	}
}
