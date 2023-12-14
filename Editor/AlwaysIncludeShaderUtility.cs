using D3T.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace D3TEditor
{
	internal static class AlwaysIncludeShaderUtility
	{
		[InitializeOnLoadMethod]
		public static void Init()
		{
			EditorApplication.delayCall += CheckAttributes;
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
				AddAlwaysIncludedShader(arrayProp, shader);
				hasChanges = true;
			}

			if(hasChanges)
			{
				serializedObj.ApplyModifiedProperties();
				EditorApplication.delayCall += () => AssetDatabase.SaveAssets();
			}
		}

		public static void AddAlwaysIncludedShader(SerializedProperty arrayProp, Shader shader)
		{
			if(shader == null) return;

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
			}
		}
	}
}