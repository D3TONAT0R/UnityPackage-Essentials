using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEssentials;

namespace UnityEssentialsEditor
{
	[CustomPropertyDrawer(typeof(SceneReference))]
	internal class SceneReferenceDrawer : PropertyDrawer
	{
		private GUIContent errorIcon;
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var buildIndex = GetBuildIndex(property);
			position.SplitHorizontalRight(20, out position, out var indexRect, 2);
			EditorGUI.BeginChangeCheck();
			try
			{
				EditorGUI.PropertyField(position, property.FindPropertyRelative("sceneAsset"), label);
			}
			catch(ExitGUIException)
			{
				//Ignore exit gui exceptions
			}
			var scene = property.FindPropertyRelative("sceneAsset").objectReferenceValue as SceneAsset;
			if(EditorGUI.EndChangeCheck())
			{
				var nameProp = property.FindPropertyRelative("sceneName");
				var indexProp = property.FindPropertyRelative("buildIndex");
				if(scene)
				{
					nameProp.stringValue = scene.name;
					var index = SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(scene));
					indexProp.intValue = index;
				}
				else
				{
					nameProp.stringValue = "";
					indexProp.intValue = -1;
				}
				property.serializedObject.ApplyModifiedProperties();
			}
			if (scene != null)
			{
				if (buildIndex >= 0)
				{
					GUI.Box(indexRect, (buildIndex.HasValue && buildIndex >= 0) ? "#" + buildIndex.Value : "-", EditorStyles.centeredGreyMiniLabel);
				}
				else
				{
					if (errorIcon == null)
					{
						errorIcon = EditorGUIUtility.IconContent("d_console.erroricon.sml");
						errorIcon.tooltip = "Scene is not in build settings";
					}
					GUI.Label(indexRect, errorIcon);
				}
			}
		}

		private int? GetBuildIndex(SerializedProperty property)
		{
			var sceneAssetProp = property.FindPropertyRelative("sceneAsset");
			var sceneAsset = sceneAssetProp.objectReferenceValue as SceneAsset;
			if(sceneAsset != null)
			{
				var buildIndex = SceneUtility.GetBuildIndexByScenePath(AssetDatabase.GetAssetPath(sceneAsset));
				return buildIndex;
			}
			return null;
		}
	}
}
