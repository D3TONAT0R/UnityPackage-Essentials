using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace D3TEditor
{
	public static class SelectInChildrenMenuItems
	{
		const string menu = "GameObject/Select In Children/";
		const int prio = -10;

		[MenuItem(menu + "Mesh Renderer", false, prio)]
		private static void SelectRenderers(MenuCommand cmd) => SelectInChildren<MeshRenderer>(cmd.context);

		[MenuItem(menu + "Mesh Renderer (Contribute GI Static)", false, prio)]
		private static void SelectRenderersStatic(MenuCommand cmd) => SelectInChildren<MeshRenderer>(cmd.context, (r) => GameObjectUtility.GetStaticEditorFlags(r.gameObject).HasFlag(StaticEditorFlags.ContributeGI));

		[MenuItem(menu + "Mesh Renderer", true)]
		[MenuItem(menu + "Mesh Renderer (Contribute GI Static)", true)]
		private static bool Validate()
		{
			return Selection.activeGameObject != null && Selection.gameObjects.Length == 1 && Selection.activeGameObject.scene != null;
		}

		static void SelectInChildren<T>(Object obj, Func<T, bool> predicate = null) where T : Component
		{
			var parentGO = obj as GameObject;
			var comps = parentGO.GetComponentsInChildren<T>(true);
			List<Object> sel = new List<Object>();
			foreach(var c in parentGO.GetComponentsInChildren<T>(true))
			{
				if(predicate == null || predicate.Invoke(c))
				{
					if(!sel.Contains(c.gameObject)) sel.Add(c.gameObject);
				}
			}
			Selection.objects = sel.ToArray();
			Debug.Log($"Selected {sel.Count} GameObjects.");
		}
	} 
}
