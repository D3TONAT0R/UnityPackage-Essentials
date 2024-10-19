using D3T;
using System;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public class EssentialsProjectSettings : ProjectSettingsAsset
	{
		public override string ProjectAssetName => "EssentialsProjectSettings";

		public static EssentialsProjectSettings Instance
		{
			get
			{
				if(instance == null) instance = CreateSettingsAsset<EssentialsProjectSettings>();
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
		[Header("Menu Management", order = 0)]
		[HelpBox("Changing menu items may require a restart of the Unity Editor to take effect.", HelpBoxType.Info, order = 1), SerializeField]
#if !UNITY_6000_0_OR_NEWER
		public bool reorganizeAssetMenu = true;
#endif
		public string[] menuItemsToRemove;

		[Space(20)]
		public bool enableEditorTimeTracking = true;

		protected override void OnCreateNewSettings()
		{
			Debug.Log("Unity Essentials: Creating new project settings asset.");
		}

		protected override void OnInitialize()
		{
			if(menuItemsToRemove != null)
			{
				foreach(var item in menuItemsToRemove)
				{
					if(!string.IsNullOrWhiteSpace(item)) MenuUtility.RemoveMenuItem(item);
				}
			}
		}
	}
}
