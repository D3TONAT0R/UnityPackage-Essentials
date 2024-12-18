using UnityEssentials;
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
			var attributeDefs = ReflectionUtility.GetClassAndAssemblyAttributes<AlwaysIncludeShaderAttribute>(false);
			if(attributeDefs == null || attributeDefs.Count == 0) return;

			var settingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
			var serializedObj = new SerializedObject(settingsObj);
			var arrayProp = serializedObj.FindProperty("m_AlwaysIncludedShaders");

			bool hasChanges = false;

			foreach(var def in attributeDefs)
			{
				Shader shader = Shader.Find(def.Attribute.shaderName);
				if(shader == null)
				{
					Debug.LogError($"Failed to find Shader to include: {def.Attribute.shaderName}\nDefined in: {def}");
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