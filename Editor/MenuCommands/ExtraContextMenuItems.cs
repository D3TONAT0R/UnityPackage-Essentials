using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	internal static class ExtraContextMenuItems
	{
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
			Mesh meshToSave = Object.Instantiate(mf.sharedMesh);
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
	} 
}
