﻿using UnityEssentials.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEssentialsEditor
{
	internal static class AlwaysIncludeShaderUtility
	{
		[InitializeOnLoadMethod]
		public static void Init()
		{
			if(!EditorApplication.isPlayingOrWillChangePlaymode) EditorApplication.delayCall += CheckAttributes;
		}

		private static void CheckAttributes()
		{
			var attributes = ReflectionUtility.GetClassAndAssemblyAttributes<AlwaysIncludeShaderAttribute>(false);
			if(attributes == null || attributes.Count == 0) return;

			var settingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
			var serializedObj = new SerializedObject(settingsObj);
			var arrayProp = serializedObj.FindProperty("m_AlwaysIncludedShaders");

			bool hasChanges = false;

			foreach(var attr in attributes)
			{
				Shader shader = Shader.Find(attr.shaderName);
				if(shader == null)
				{
					Debug.LogError("Failed to find Shader to include: " + attr.shaderName);
					continue;
				}
				hasChanges |= AddAlwaysIncludedShader(arrayProp, shader);
			}

			if(hasChanges)
			{
				serializedObj.ApplyModifiedProperties();
				EditorApplication.delayCall += () => AssetDatabase.SaveAssets();
			}
		}

		public static bool AddAlwaysIncludedShader(SerializedProperty arrayProp, Shader shader)
		{
			if(shader == null) return false;

			bool hasShader = false;
			for(int i = 0; i < arrayProp.arraySize; ++i)
			{
				var arrayElem = arrayProp.GetArrayElementAtIndex(i);
				if(shader == arrayElem.objectReferenceValue)
				{
					hasShader = true;
					break;
				}
			}

			if(!hasShader)
			{
				int arrayIndex = arrayProp.arraySize;
				arrayProp.InsertArrayElementAtIndex(arrayIndex);
				var arrayElem = arrayProp.GetArrayElementAtIndex(arrayIndex);
				arrayElem.objectReferenceValue = shader;

				Debug.Log($"Added shader '{shader.name}' to the list of always included shaders.");
				return true;
			}
			return false;
		}
	}
}