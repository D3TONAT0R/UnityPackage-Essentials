using System;
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace D3TEditor
{
	public static class AssetDatabaseUtility
	{
		internal class CreateFromTemplateAssetAction : EndNameEditAction
		{
			private string templateFile;
			private Action<Object> callback;

			public static CreateFromTemplateAssetAction CreateAction(string templateFile, Action<Object> callback)
			{
				var instance = CreateInstance<CreateFromTemplateAssetAction>();
				instance.templateFile = templateFile;
				instance.callback = callback;
				return instance;
			}

			public override void Action(int instanceId, string pathName, string resourceFile)
			{
				string finalPath = AssetDatabase.GenerateUniqueAssetPath(pathName + Path.GetExtension(templateFile));
				AssetDatabase.CopyAsset(templateFile, finalPath);
				var newAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(finalPath);
				ProjectWindowUtil.ShowCreatedAsset(newAsset);
				callback?.Invoke(newAsset);
			}
		}

		/// <summary>
		/// Prompts the user to type a name for a new asset that will be created from the given template file.
		/// </summary>
		public static void BeginAssetCreationFromTemplateFile(string templateAssetPath, string defaultName, Texture2D icon, Action<Object> callback = null)
		{
			var action = CreateFromTemplateAssetAction.CreateAction(templateAssetPath, callback);
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, defaultName, icon, templateAssetPath);
		}
	}
}
