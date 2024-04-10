using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;
using D3T;
using UnityEditor.Compilation;

namespace D3TEditor
{
	public static class MenuUtility
	{
		private static MethodInfo addMenuItemMethod;
		private static MethodInfo removeMenuItemMethod;

#if UNITY_2021_1_OR_NEWER
		private static IDictionary menuItemsDictionary;
		private static IEnumerable<string> menuItemsDictionaryKeys;
#endif

		public static void RemoveMenuItem(string menuPath)
		{
			if(removeMenuItemMethod == null) removeMenuItemMethod = typeof(Menu).GetMethod("RemoveMenuItem", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			removeMenuItemMethod.Invoke(null, new object[] { menuPath });
		}

		public static void AddMenuItem(string name, string shortcut, int priority, Action execute, bool isChecked = false, Func<bool> validate = null)
		{
			if(addMenuItemMethod == null) addMenuItemMethod = typeof(Menu).GetMethod("AddMenuItem", BindingFlags.NonPublic | BindingFlags.Static);
			addMenuItemMethod.Invoke(null, new object[] { name, shortcut, isChecked, priority, execute, validate });
		}

		public static void ReplaceCreateAssetMenu(string oldPath, string newPath, int priority, Func<UnityEngine.Object> creator, string extension = "asset")
		{
#if UNITY_2020_1_OR_NEWER
			bool moved = MoveAssetMenu("Assets/Create/" + oldPath, "Assets/Create/" + newPath, priority);
#else
			bool moved = false;
#endif
			if(!moved)
			{
				
				RemoveMenuItem("Assets/Create/" + oldPath);
				AddMenuItem("Assets/Create/" + newPath, "", priority, () =>
				{
					var instance = creator.Invoke();
					ProjectWindowUtil.CreateAsset(instance, $"New {instance.GetType().Name}.{extension}");
				});
				
			}
		}

#if UNITY_2021_1_OR_NEWER
		public static bool MoveAssetMenu(string oldPath, string newPath, int newPriority)
		{
			if(menuItemsDictionary == null)
			{
				var type = Type.GetType("UnityEditor.MenuService, UnityEditor", true);
				menuItemsDictionary = (IDictionary)type.GetField("s_MenuItemsDefaultMode", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				menuItemsDictionaryKeys = menuItemsDictionary.Keys.Cast<string>();
			}

			var key = menuItemsDictionaryKeys.FirstOrDefault(k => k.StartsWith(oldPath));
			if(key != null)
			{
				var groupingMenuItemCommands = menuItemsDictionary[key];
				var menuItem = groupingMenuItemCommands.GetType().GetField("menuItem").GetValue(groupingMenuItemCommands);

				var menuItemType = menuItem.GetType();
				MethodInfo executeMethodInfo = menuItemType.GetField("execute").GetValue(menuItem) as MethodInfo;
				MethodInfo validateMethodInfo = menuItemType.GetField("validate").GetValue(menuItem) as MethodInfo;
				Delegate executeDelegate = menuItemType.GetField("commandExecute").GetValue(menuItem) as Delegate;
				Delegate validateDelegate = menuItemType.GetField("commandValidate").GetValue(menuItem) as Delegate;
				string shortcut = menuItemType.GetField("shortcut").GetValue(menuItem) as string;

				var execute = executeMethodInfo != null
					? (Action)Delegate.CreateDelegate(typeof(Action), executeMethodInfo, true)
					: (Action)executeDelegate;
				var validate = validateMethodInfo != null
					? (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), validateMethodInfo, true)
					: validateDelegate != null
						? (Func<bool>)validateDelegate
						: null;

				RemoveMenuItem(oldPath);
				AddMenuItem(newPath, shortcut, newPriority, execute, false, validate);
				return true;
			}
			else
			{
				return false;
			}
		}
#endif

		[InitializeOnLoadMethod]
		private static void Init()
		{
			if(EssentialsProjectSettings.Instance.reorganizeAssetMenu && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.delayCall += ReorganizeAssetMenu;
			}
		}

		public static void ReorganizeAssetMenu()
		{
			//TODO: remove unity menus & add them manually again

			RemoveMenuItem("Assets/Create/Playables/Playable Behaviour C# Script");
			RemoveMenuItem("Assets/Create/Playables/Playable Asset C# Script");

			ReplaceCreateAssetMenu("Shader Variant Collection", "Shader/Shader Variant Collection", 20000,
				() => new ShaderVariantCollection());

			ReplaceCreateAssetMenu("Render Texture", "Rendering/Render Texture", 101,
				() => new RenderTexture(256, 256, 0));
			ReplaceCreateAssetMenu("Custom Render Texture", "Rendering/Custom Render Texture", 102,
				() => new CustomRenderTexture(256, 256));
			if(AssemblyExists("Unity.RenderPipelines.Core.Runtime"))
			{
				RemoveMenuItem("Assets/Create/Lens Flare");
				ReplaceCreateAssetMenu("Lens Flare (SRP)", "Rendering/Lens Flare (SRP)", 103,
					() => CreateAssetOfType("UnityEngine.Rendering.LensFlareDataSRP, Unity.RenderPipelines.Core.Runtime"));
			}
			else
			{
				ReplaceCreateAssetMenu("Lens Flare", "Rendering/Lens Flare", 103,
					() => new Flare());
			}

			ReplaceCreateAssetMenu("Lightmap Parameters", "Rendering/Lightmap Parameters", 200,
				() => new LightmapParameters());
#if UNITY_2020_1_OR_NEWER
			ReplaceCreateAssetMenu("Lighting Settings", "Rendering/Lighting Settings", 201,
				() => new LightingSettings());
#endif

			ReplaceCreateAssetMenu("Animation", "Animation/Animation Clip", 200,
				() => new AnimationClip(), "anim");
			ReplaceCreateAssetMenu("Animator Controller", "Animation/Animator Controller", 201,
				() => new UnityEditor.Animations.AnimatorController(), "controller");
			ReplaceCreateAssetMenu("Animator Override Controller", "Animation/Animator Override Controller", 202,
				() => new AnimatorOverrideController(), "overrideController");
			ReplaceCreateAssetMenu("Avatar Mask", "Animation/Avatar Mask", 203,
				() => new AvatarMask(), "mask");

			ReplaceCreateAssetMenu("Custom Font", "Text/Custom Font", 500,
				() => new Font(), "fontsettings");

			if(AssemblyExists("Unity.Timeline"))
			{
				ReplaceCreateAssetMenu("Timeline", "Animation/Timeline", 310,
					() => CreateAssetOfType("UnityEngine.Timeline.TimelineAsset, Unity.Timeline"),
					"playable");
				ReplaceCreateAssetMenu("Signal", "Animation/Signal", 311,
					() => CreateAssetOfType("UnityEngine.Timeline.TimelineSignal, Unity.Timeline"),
					"signal");
			}
		}

		private static bool AssemblyExists(string assemblyName)
		{
			foreach(var assembly in CompilationPipeline.GetAssemblies())
			{
				if(assembly.name == assemblyName)
				{
					return true;
				}
			}
			return false;
		}

		private static UnityEngine.Object CreateAssetOfType(string qualifiedTypeName)
		{
			return (UnityEngine.Object)Activator.CreateInstance(Type.GetType(qualifiedTypeName, true));
		}
	}
}
