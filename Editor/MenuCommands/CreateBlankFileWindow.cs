using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	internal class CreateBlankFileWindow : EditorWindow
	{
		private string folderPath;
		private string fileName = "New File";
		private string extension = "";
		private string errorMessage = "";

		[MenuItem("Assets/Create/Blank File ...", priority = 500)]
		public static void CreateBlankFile()
		{
			if(!TryGetActiveFolderPath(out var path)) path = "Assets";
			ShowWindow(path);
		}

		private static bool TryGetActiveFolderPath(out string path)
		{
			var tryGetActiveFolderPathMethod = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
			object[] args = new object[] { null };
			bool found = (bool)tryGetActiveFolderPathMethod.Invoke(null, args);
			path = (string)args[0];
			return found;
		}

		public static void ShowWindow(string folderPath)
		{
			var window = CreateInstance<CreateBlankFileWindow>();
			window.minSize = new Vector2(200, 180);
			window.maxSize = new Vector2(400, 180);
			window.titleContent = new GUIContent("Create Blank File");
			window.folderPath = folderPath;
			window.ShowModal();
		}

		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = 100;
			GUILayout.Label("Create Blank File", EditorStyles.boldLabel);

			EditorGUILayout.LabelField("Folder", folderPath);
			fileName = EditorGUILayout.TextField("File Name", fileName);
			extension = EditorGUILayout.TextField("Extension", extension);

			string fullFileName = extension.Length > 0 ? $"{fileName}.{extension}" : fileName;
			EditorGUILayout.LabelField("Output File", fullFileName);
			string fullPath = Path.Combine(folderPath, fullFileName);

			CheckErrors(fullPath);
			GUILayout.FlexibleSpace();
			if(!string.IsNullOrEmpty(errorMessage))
			{
				EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
				GUI.enabled = false;
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Save", GUILayout.Width(80)))
			{
				SaveFile(fullPath);
				Close();
			}
			GUI.enabled = true;
			if(GUILayout.Button("Cancel", GUILayout.Width(80)))
			{
				Close();
			}
			EditorGUILayout.EndHorizontal();
		}


		private void CheckErrors(string fullPath)
		{
			errorMessage = "";
			if(string.IsNullOrWhiteSpace(fileName))
			{
				errorMessage = "File name cannot be empty.";
				return;
			}
			if(fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || extension.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				errorMessage = "Path contains invalid characters.";
				return;
			}
			if(File.Exists(fullPath))
			{
				errorMessage = "File already exists.";
				return;
			}
		}

		private void SaveFile(string fullPath)
		{
			File.WriteAllBytes(fullPath, Array.Empty<byte>());
			AssetDatabase.ImportAsset(fullPath);
		}
	}
}
