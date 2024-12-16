using UnityEssentials;
using UnityEssentials.Meshes;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor
{
	public class ExtraGameObjectMenuItems
	{
		[MenuItem("GameObject/Create Parent", false, 0)]
		public static void CreateParent(MenuCommand menuCommand)
		{
			var matrices = new Dictionary<Transform, Matrix4x4>();
			Bounds bounds = new Bounds();
			for(int i = 0; i < Selection.transforms.Length; i++)
			{
				var t = Selection.transforms[i];
				matrices.Add(t, t.GetTRSMatrix());
				if(i == 0) bounds = new Bounds(t.position, Vector3.zero);
				else bounds.Encapsulate(t.position);
			}
			var parent = new GameObject("New Parent").transform;
			Undo.RegisterCreatedObjectUndo(parent.gameObject, "Create Parent");
			parent.parent = Selection.activeTransform.parent;
			parent.transform.position = bounds.center;
			if(Selection.transforms.Length == 1)
			{
				parent.transform.rotation = Selection.activeTransform.rotation;
				parent.transform.localScale = Selection.activeTransform.localScale;
			}
			parent.SetSiblingIndex(Selection.activeTransform.GetSiblingIndex());
			foreach(var t in Selection.transforms)
			{
				Undo.SetTransformParent(t, parent, "Create Parent");
			}
			Selection.activeTransform = parent;

			SetExpanded(parent.gameObject, true);
		}

		[MenuItem("GameObject/Create Parent", true)]
		public static bool ValidateCreateParent(MenuCommand menuCommand)
		{
			return Selection.gameObjects.Length > 0;
		}


		[MenuItem("GameObject/3D Object/Convex Mesh", false, 6)]
		public static void CreateConvexMesh(MenuCommand menuCommand)
		{
			var root = CreateRootObject(menuCommand, "ConvexMesh");
			var builder = ObjectFactory.AddComponent<ConvexMeshBuilderComponent>(root);
			ObjectFactory.AddComponent<MeshFilter>(root);
			var mr = ObjectFactory.AddComponent<MeshRenderer>(root);
			mr.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
			var mc = ObjectFactory.AddComponent<MeshCollider>(root);
			mc.convex = true;
			builder.applyTo = ConvexMeshBuilderComponent.TargetComponents.Both;
			builder.Validate();
		}

		public static GameObject CreateRootObject(MenuCommand menuCommand, string name, float distanceFromSceneView = 10)
		{
			// Create a custom game object
			GameObject go = new GameObject(name);
			// Ensure it gets reparented if this was a context click (otherwise does nothing)
			GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
			if(SceneView.lastActiveSceneView != null && distanceFromSceneView > 0)
			{
				go.transform.position = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, distanceFromSceneView));
			}
			// Register the creation in the undo system
			Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
			Selection.activeObject = go;
			return go;
		}

		private static GameObject CreateChild(GameObject parent, string name, params Type[] components)
		{
			GameObject child = new GameObject(name, components);
			child.transform.parent = parent.transform;
			child.transform.localPosition = Vector3.zero;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
			return child;
		}

		private static void SetExpanded(GameObject obj, bool expand)
		{
			var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
			var window = EditorWindow.GetWindow(type);
			object sceneHierarchy = type.GetProperty("sceneHierarchy").GetValue(window);
			var methodInfo = sceneHierarchy.GetType().GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);

			methodInfo.Invoke(sceneHierarchy, new object[] { obj.GetInstanceID(), expand });
		}
	}
}
