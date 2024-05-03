using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
    internal static class CreateScriptMenuUtility
    {
		public const string menuRoot = "Assets/Create/Script/";
		const string packageTemplateScriptRoot = "Packages/com.github.d3tonat0r.essentials/Editor/TemplateAssets/ScriptTemplates/";

		public static string DefaultNamespace
		{
			get
			{
				string namespaceString;
				if(!string.IsNullOrWhiteSpace(EssentialsProjectSettings.Instance.defaultScriptNamespace))
				{
					namespaceString = EssentialsProjectSettings.Instance.defaultScriptNamespace;
				}
				else
				{
					namespaceString = Directory.GetParent(Application.dataPath).Name;
				}

				namespaceString = Regex.Replace(namespaceString, @"[^a-zA-Z0-9]+", "_");

				if(!char.IsLetter(namespaceString[0]))
				{
					namespaceString = "_" + namespaceString;
				}

				return namespaceString;
			}
		}

		[InitializeOnLoadMethod]
		private static void Init()
		{
			if(EssentialsProjectSettings.Instance.removeDefaultScriptMenu && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.delayCall += () => MenuUtility.RemoveMenuItem("Assets/Create/C# Script");
			}
		}

		public static void CreateScriptAsset(string templatePath, string defaultFileName)
        {
			defaultFileName = defaultFileName.Replace(" ", "");
			var icon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAction>(), defaultFileName, icon, templatePath);
		}

        [MenuItem(menuRoot + "Behaviour Script", priority = 80)]
        private static void CreateBehaviourScript() => CreateScriptAsset(packageTemplateScriptRoot + "BehaviourScript.txt", "NewBehaviourScript");

		[MenuItem(menuRoot + "ScriptableObject", priority = 80)]
		private static void CreateScriptableObject() => CreateScriptAsset(packageTemplateScriptRoot + "ScriptableObject.txt", "NewScriptableObject");

		[MenuItem(menuRoot + "Class", priority = 80)]
		private static void CreateClass() => CreateScriptAsset(packageTemplateScriptRoot + "SerializableClass.txt", "NewClass");

		[MenuItem(menuRoot + "Interface", priority = 80)]
		private static void CreateInterface() => CreateScriptAsset(packageTemplateScriptRoot + "Interface.txt", "INewInterface");

		[MenuItem(menuRoot + "Static Class", priority = 80)]
		private static void CreateStaticClass() => CreateScriptAsset(packageTemplateScriptRoot + "StaticClass.txt", "NewStaticClass");
	} 
}
