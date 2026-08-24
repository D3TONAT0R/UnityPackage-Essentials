using System;
using System.Linq;
using UnityEngine;

namespace UnityEssentials.Reflection
{
	[CreateAssetMenu(fileName = "PlayerAssemblyNames", menuName = "ScriptableObjects/PlayerAssemblyNames")]
	public class AssemblyNames : ScriptableObject
	{
		private static AssemblyNames instance;
		
		public string[] assemblyNames;

		public static string[] GetPlayerAssemblyNames()
		{
			if (instance == null)
			{
#if UNITY_EDITOR
				instance = GeneratePlayerAssembliesAsset();
#else
				instance = Resources.Load<AssemblyNames>("PlayerAssemblyNames");
#endif
				if (instance == null)
				{
					Debug.LogError("PlayerAssemblyNames asset is missing. Reflection-based features will not be available in this build.");
					instance = CreateInstance<AssemblyNames>();
					instance.assemblyNames = Array.Empty<string>();
				}
			}
			return instance.assemblyNames;
		} 
		
		#if UNITY_EDITOR
		private static string[] editorAssemblyNames;
		
		public static string[] GetEditorAssemblyNames()
		{
			if (editorAssemblyNames == null)
			{
				editorAssemblyNames = UnityEditor.Compilation.CompilationPipeline.GetAssemblies(UnityEditor.Compilation.AssembliesType.Editor)
					.Select(a => a.name).ToArray();
			}
			return editorAssemblyNames;
		}

		internal static AssemblyNames GeneratePlayerAssembliesAsset()
		{
			var obj = CreateInstance<AssemblyNames>();
			obj.assemblyNames = UnityEditor.Compilation.CompilationPipeline.GetAssemblies(UnityEditor.Compilation.AssembliesType.Player)
				.Select(a => a.name).ToArray();
			return obj;
		}
		#endif
	}
	
#if UNITY_EDITOR
	internal class AssemblyNamesBuildPreprocessor : UnityEditor.Build.IPreprocessBuildWithReport, UnityEditor.Build.IPostprocessBuildWithReport
	{
		private static bool didCreateResourcesFolder = false;
		
		public int callbackOrder => 0;

		public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
		{
			var resourcesDir = "Assets/Resources";
			if (System.IO.Directory.Exists(resourcesDir))
			{
				didCreateResourcesFolder = false;
			}
			else
			{
				UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
				Debug.Log("Created Resources folder for PlayerAssemblyNames asset.");
				didCreateResourcesFolder = true;
			}
			UnityEditor.AssetDatabase.CreateAsset(AssemblyNames.GeneratePlayerAssembliesAsset(), "Assets/Resources/PlayerAssemblyNames.asset");
		}
		
		public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
		{
			UnityEditor.AssetDatabase.DeleteAsset("Assets/Resources/PlayerAssemblyNames.asset");
			if (didCreateResourcesFolder)
			{
				UnityEditor.AssetDatabase.DeleteAsset("Assets/Resources");
				Debug.Log("Deleted Resources folder created for PlayerAssemblyNames asset.");
			}
		}
	}
#endif
}