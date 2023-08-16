using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
    public static class CreateScriptMenuUtility
    {
		public const string menuRoot = "Assets/Create/Script/";
		const string packageTemplateScriptRoot = "Packages/com.github.d3tonat0r.essentials/Editor/TemplateAssets/ScriptTemplates/";

		//TODO: Create project setting for default namespace
		public static string DefaultNamespace => "MyNamespace";

		[InitializeOnLoadMethod]
		private static void Init()
		{
			MenuUtility.RemoveMenuItem("Assets/Create/C# Script");
		}

		public static void CreateScriptAsset(string templatePath, string defaultFileName = null)
        {
			if(defaultFileName == null) defaultFileName = "New "+Path.GetFileNameWithoutExtension(templatePath);
			defaultFileName = defaultFileName.Replace(" ", "");
			var icon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAction>(), defaultFileName, icon, templatePath);
		}

        [MenuItem(menuRoot + "Behaviour Script", priority = 80)]
        private static void CreateBehaviourScript() => CreateScriptAsset(packageTemplateScriptRoot + "BehaviourScript.txt");

		[MenuItem(menuRoot + "ScriptableObject", priority = 80)]
		private static void CreateScriptableObject() => CreateScriptAsset(packageTemplateScriptRoot + "ScriptableObject.txt");

		[MenuItem(menuRoot + "Class", priority = 80)]
		private static void CreateClass() => CreateScriptAsset(packageTemplateScriptRoot + "SerializableClass.txt", "Class");

		[MenuItem(menuRoot + "Interface", priority = 80)]
		private static void CreateInterface() => CreateScriptAsset(packageTemplateScriptRoot + "Interface.txt", "INewInterface");

		[MenuItem(menuRoot + "Static Class", priority = 80)]
		private static void CreateStaticClass() => CreateScriptAsset(packageTemplateScriptRoot + "StaticClass.txt");
	} 
}
