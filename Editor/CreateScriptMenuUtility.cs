using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public static class CreateScriptMenuUtility
	{
#if UNITY_6000_0_OR_NEWER
		public const string MENU_ROOT = "Assets/Create/Scripting/";
#else
		public const string MENU_ROOT = "Assets/Create/Script/";
#endif

#if UNITY_6000_0_OR_NEWER
		public const int PRIORITY = -145;
#else
		public const int PRIORITY = 80;
#endif

		private const string PACKAGE_TEMPLATE_SCRIPT_ROOT = "Packages/com.github.d3tonat0r.essentials/Editor/TemplateAssets/ScriptTemplates/";

		[InitializeOnLoadMethod]
		private static void Init()
		{
			if(EssentialsProjectSettings.Instance.removeDefaultScriptMenu && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
#if UNITY_6000_0_OR_NEWER
				EditorApplication.delayCall += () =>
				{
					MenuUtility.RemoveMenuItem("Assets/Create/MonoBehaviour Script");
					MenuUtility.RemoveMenuItem("Assets/Create/Scripting/MonoBehaviour Script");
					MenuUtility.RemoveMenuItem("Assets/Create/Scripting/ScriptableObject Script");
					MenuUtility.RemoveMenuItem("Assets/Create/Scripting/Empty C# Script");
				};
#else
				EditorApplication.delayCall += () => MenuUtility.RemoveMenuItem("Assets/Create/C# Script");
#endif
			}
		}

		public static void CreateScriptAsset(string templatePath, string defaultFileName)
		{
			defaultFileName = defaultFileName.Replace(" ", "");
			var icon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAction>(), defaultFileName, icon, templatePath);
		}

		[MenuItem(MENU_ROOT + "Behaviour Script", priority = PRIORITY)]
		private static void CreateBehaviourScript() => CreateScriptAsset(PACKAGE_TEMPLATE_SCRIPT_ROOT + "BehaviourScript.txt", "NewBehaviourScript");

		[MenuItem(MENU_ROOT + "ScriptableObject", priority = PRIORITY)]
		private static void CreateScriptableObject() => CreateScriptAsset(PACKAGE_TEMPLATE_SCRIPT_ROOT + "ScriptableObject.txt", "NewScriptableObject");

		[MenuItem(MENU_ROOT + "Class", priority = PRIORITY)]
		private static void CreateClass() => CreateScriptAsset(PACKAGE_TEMPLATE_SCRIPT_ROOT + "SerializableClass.txt", "NewClass");

		[MenuItem(MENU_ROOT + "Interface", priority = PRIORITY)]
		private static void CreateInterface() => CreateScriptAsset(PACKAGE_TEMPLATE_SCRIPT_ROOT + "Interface.txt", "INewInterface");

		[MenuItem(MENU_ROOT + "Static Class", priority = PRIORITY)]
		private static void CreateStaticClass() => CreateScriptAsset(PACKAGE_TEMPLATE_SCRIPT_ROOT + "StaticClass.txt", "NewStaticClass");
	}
}
