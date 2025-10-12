using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	internal static class ExtraContextMenuItems
	{
		private static SerializedObject replaceScriptTarget;
		private static MonoScript replaceScriptWith;

		[MenuItem("CONTEXT/Transform/Apply Position")]
		public static void ApplyPosition(MenuCommand cmd)
		{
			var transform = (Transform)cmd.context;
			Vector3[] childWorldPositions = new Vector3[transform.childCount];
			for(int i = 0; i < transform.childCount; i++)
			{
				childWorldPositions[i] = transform.GetChild(i).position;
			}
			Undo.RecordObject(transform, "Apply Position");
			transform.localPosition = Vector3.zero;
			for(int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);
				Undo.RecordObject(child, "Apply Position");
				child.position = childWorldPositions[i];
			}
		}

		[MenuItem("CONTEXT/Transform/Apply Rotation")]
		public static void ApplyRotation(MenuCommand cmd)
		{
			var transform = (Transform)cmd.context;
			Vector3[] childWorldPositions = new Vector3[transform.childCount];
			Quaternion[] childWorldRotations = new Quaternion[transform.childCount];
			for(int i = 0; i < transform.childCount; i++)
			{
				childWorldPositions[i] = transform.GetChild(i).position;
				childWorldRotations[i] = transform.GetChild(i).rotation;
			}
			Undo.RecordObject(transform, "Apply Rotation");
			transform.localRotation = Quaternion.identity;
			for(int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);
				Undo.RecordObject(child, "Apply Rotation");
				child.position = childWorldPositions[i];
				child.rotation = childWorldRotations[i];
			}
		}

		[MenuItem("CONTEXT/Transform/Apply Scale")]
		public static void ApplyScale(MenuCommand cmd)
		{
			var transform = (Transform)cmd.context;
			Vector3 ratio = transform.localScale;// new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y, 1f / transform.localScale.z);
			Undo.RecordObject(transform, "Apply Scale");
			transform.localScale = Vector3.one;
			for(int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);
				Undo.RecordObject(child, "Apply Scale");
				child.localPosition = Vector3.Scale(child.localPosition, ratio);
				child.localScale = Vector3.Scale(child.localScale, ratio);
			}
		}

		[MenuItem("CONTEXT/Transform/Apply Full Transform")]
		public static void ApplyFullTransform(MenuCommand cmd)
		{
			Undo.SetCurrentGroupName("Apply Full Transform");
			int group = Undo.GetCurrentGroup();
			ApplyPosition(cmd);
			ApplyRotation(cmd);
			ApplyScale(cmd);
			Undo.CollapseUndoOperations(group);
		}

		[MenuItem("CONTEXT/Transform/Apply Position", validate = true)]
		[MenuItem("CONTEXT/Transform/Apply Rotation", validate = true)]
		[MenuItem("CONTEXT/Transform/Apply Scale", validate = true)]
		public static bool ValidateTransformApplyCommand(MenuCommand cmd)
		{
			var transform = (Transform)cmd.context;
			return transform.childCount > 0;
		}

		[MenuItem("CONTEXT/MeshFilter/Export Mesh As Asset...")]
		public static void SaveMeshAsAsset(MenuCommand menuCommand)
		{
			MeshFilter mf = menuCommand.context as MeshFilter;
			Mesh meshToSave = UnityEngine.Object.Instantiate(mf.sharedMesh);
			MeshUtility.Optimize(meshToSave);

			string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", meshToSave.name, "asset");
			if(!string.IsNullOrEmpty(path))
			{
				path = FileUtil.GetProjectRelativePath(path);
				AssetDatabase.CreateAsset(meshToSave, path);
				AssetDatabase.SaveAssets();
			}
		}

		[MenuItem("CONTEXT/MeshFilter/Export Mesh As Asset...", validate = true)]
		public static bool ValidateSaveMeshAsAsset(MenuCommand cmd)
		{
			var filter = (MeshFilter)cmd.context;
			return filter.sharedMesh != null;
		}

		[MenuItem("CONTEXT/MeshRenderer/Instantiate Materials")]
		public static void InstantiateMaterials(MenuCommand cmd)
		{
			Undo.RecordObject(cmd.context, "Instantiate Materials");
			var renderer = (MeshRenderer)cmd.context;
			var materials = renderer.sharedMaterials;
			for(int i = 0; i < materials.Length; i++)
			{
				var name = materials[i].name;
				materials[i] = UnityEngine.Object.Instantiate(materials[i]);
				if(!materials[i].name.EndsWith("(Instance)")) materials[i].name = name + " (Instance)";
			}
			renderer.sharedMaterials = materials;
		}

		[MenuItem("CONTEXT/Component/Search Similar")]
		public static void SearchSimilarComponents(MenuCommand cmd)
		{
			var comp = (Component)cmd.context;
			SceneModeUtility.SearchForType(comp.GetType());
		}

		[MenuItem("CONTEXT/Component/Search Similar (Base Type)")]
		public static void SearchSimilarComponentsBaseType(MenuCommand cmd)
		{
			var comp = (Component)cmd.context;
			var baseType = comp?.GetType().BaseType;
			if(baseType != null) SceneModeUtility.SearchForType(baseType);
		}

		[MenuItem("CONTEXT/Component/Search Similar (Base Type)", validate = true)]
		public static bool ValidateSearchSimilarComponentsBaseType(MenuCommand cmd)
		{
			var comp = (Component)cmd.context;
			var baseType = comp?.GetType().BaseType;
			return baseType != null && baseType != typeof(Component) && baseType != typeof(Behaviour);
		}

		[MenuItem("CONTEXT/MonoBehaviour/Select Script", priority = 110)]
		public static void SelectScript(MenuCommand cmd)
		{
			var script = (MonoBehaviour)cmd.context;
			Selection.activeObject = MonoScript.FromMonoBehaviour(script);
			EditorGUIUtility.PingObject(Selection.activeObject);
		}

		[MenuItem("CONTEXT/MonoBehaviour/Replace Script", priority = 111)]
		public static void ReplaceScript(MenuCommand cmd)
		{
			replaceScriptWith = MonoScript.FromMonoBehaviour((MonoBehaviour)cmd.context);
			EditorGUIUtility.ShowObjectPicker<MonoScript>(replaceScriptWith, false, "", -100);
			replaceScriptTarget = new SerializedObject(cmd.context);
			EditorApplication.update += UpdateObjectPicker;
		}

		[MenuItem("CONTEXT/MonoBehaviour/Select Script", validate = true)]
		[MenuItem("CONTEXT/MonoBehaviour/Replace Script", validate = true)]
		public static bool ValidateScriptSelection(MenuCommand cmd)
		{
			var script = cmd.context as MonoBehaviour;
			return script != null;
		}

		private static void UpdateObjectPicker()
		{
			var gameObject = ((Component)replaceScriptTarget?.targetObject).gameObject;
			if(replaceScriptTarget == null || !Selection.Contains(gameObject))
			{
				// Close the picker if the target is lost
				replaceScriptTarget = null;
				replaceScriptWith = null;
				EditorApplication.update -= UpdateObjectPicker;
				return;
			}
			var pickerControlID = EditorGUIUtility.GetObjectPickerControlID();
			if(pickerControlID == -100)
			{
				// Update the selected object
				var newScript = EditorGUIUtility.GetObjectPickerObject() as MonoScript;
				if(newScript != null && newScript != replaceScriptWith)
				{
					ApplyScript();
				}
				replaceScriptWith = newScript;
			}
			else if(pickerControlID == 0 && Selection.Contains(gameObject))
			{
				// Picker was closed
				if(replaceScriptWith == null)
				{
					replaceScriptTarget = null;
					replaceScriptWith = null;
					EditorApplication.update -= UpdateObjectPicker;
					return;
				}

				//Apply the new script
				ApplyScript();

				//Cleanup
				replaceScriptTarget = null;
				replaceScriptWith = null;
				EditorApplication.update -= UpdateObjectPicker;
			}
		}

		private static void ApplyScript()
		{
			Undo.RegisterCompleteObjectUndo(replaceScriptTarget.targetObject, "Replace Script");
			SerializedProperty scriptProperty = replaceScriptTarget.FindProperty("m_Script");
			replaceScriptTarget.Update();
			scriptProperty.objectReferenceValue = replaceScriptWith;
			replaceScriptTarget.ApplyModifiedProperties();
		}
	}
}
