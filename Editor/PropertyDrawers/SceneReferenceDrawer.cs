using D3T;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace D3TEditor
{
	[CustomPropertyDrawer(typeof(SceneReference))]
	internal class SceneReferenceDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var buildIndex = GetBuildIndex(property);
			if(buildIndex == -1)
			{
				var boxRect = position;
				boxRect.height = 2 * EditorGUIUtility.singleLineHeight;
				boxRect.xMin += EditorGUIUtility.labelWidth + 1;
				EditorGUI.HelpBox(boxRect, "Scene is not in build settings.", MessageType.Error);
				position.y += boxRect.height + EditorGUIUtility.standardVerticalSpacing;
				position.height = EditorGUIUtility.singleLineHeight;
			}
			position.SplitHorizontalRight(60, out position, out var indexRect, 2);
			EditorGUI.PropertyField(position, property.FindPropertyRelative("sceneAsset"), label);
			GUI.Box(indexRect, "Index: " + ((buildIndex.HasValue && buildIndex >= 0) ? buildIndex.Value.ToString() : "N/A"), EditorStyles.helpBox);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if(GetBuildIndex(property) == -1)
			{
				return base.GetPropertyHeight(property, label) + 2 * EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}
			return base.GetPropertyHeight(property, label);
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
