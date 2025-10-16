using UnityEssentials;
using System;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public class EssentialsProjectSettings : ProjectSettingsAsset
	{
		public enum InspectorMode
		{
			Disabled,
			Foldout,
			AlwaysVisible
		}

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
		[Tooltip("If checked, the default script menu will be removed in favor of custom script templates.")]
		public bool removeDefaultScriptMenu = true;
		[Tooltip("If checked, all newly created scripts are placed in the namespace given below.")]
		public bool useDefaultNamespace = true;
		[EnabledIf(nameof(useDefaultNamespace))]
		[Tooltip("The namespace to use for new scripts. If empty, the unity project name is used.")]
		public string defaultScriptNamespace = "";
#if UNITY_2020_2_OR_NEWER
		[NonReorderable]
#endif
		[Tooltip("Additional usings that are added on top of new scripts.")]
		public string[] additionalDefaultUsings = Array.Empty<string>();
		[Header("Menu Management", order = 0)]
		[HelpBox("Changing menu items may require a restart of the Unity Editor to take effect.", HelpBoxType.Info, order = 1), SerializeField]
#if !UNITY_6000_0_OR_NEWER
		public bool reorganizeAssetMenu = true;
#endif
		[Tooltip("Menu item paths that should be removed (may require an editor restart).")]
		public string[] menuItemsToRemove;

		[Space(20)]
		[Tooltip("Optional name for a shortcut profile to apply when entering playmode.")]
		public string playmodeShortcutProfileName = "";

		[Header("Transform Inspector")]
		[Tooltip("Specifies how extra properties are displayed in the Transform inspector.")]
		public InspectorMode extraProperties = InspectorMode.Foldout;
		[Tooltip("Specifies how the extra toolbar is displayed in the Transform inspector.")]
		public InspectorMode toolbar = InspectorMode.Foldout;

		protected override void OnCreateNewSettings()
		{
			Debug.Log("Essentials: Creating new project settings asset.");
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
