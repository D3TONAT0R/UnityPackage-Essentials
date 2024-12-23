﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEssentialsEditor
{
	internal static class SelectInChildrenMenuItems
	{
		[InitializeOnLoadMethod]
		private static void Init()
		{
			if(!EditorApplication.isPlayingOrWillChangePlaymode) SceneHierarchyHooks.addItemsToGameObjectContextMenu += AddContextMenuItems;
		}

		private static void AddContextMenuItems(GenericMenu menu, GameObject gameObject)
		{
			int index = menu.GetIndexOf(L10n.Tr("Select Children"));
			if(index < 0)
			{
				Debug.LogError("Parent menu item not found.");
				index = 0;
			}
			index++;
			AddItem<Renderer>(menu, ref index);
			AddItem<Renderer>(menu, ref index, "Renderer (GI Static)", r => GameObjectUtility.GetStaticEditorFlags(r.gameObject).HasFlag(StaticEditorFlags.ContributeGI));
			AddItem<Collider>(menu, ref index);
			AddItem<Collider>(menu, ref index, "Trigger", c => c.isTrigger);
			AddItem<AudioSource>(menu, ref index);
			AddItem<Light>(menu, ref index);
			AddItem<ParticleSystem>(menu, ref index);
			AddItem<Camera>(menu, ref index);
			AddItem<MonoBehaviour>(menu, ref index);
		}

		private static void AddItem<T>(GenericMenu menu, ref int index, string name = null, Func<T, bool> predicate = null) where T : Component
		{
			if(name == null) name = ObjectNames.NicifyVariableName(typeof(T).Name);
			menu.InsertItem(index, "Select in Children/" + name, true, false, () => SelectInChildren<T>(Selection.transforms, predicate));
			index++;
		}

		private static void SelectInChildren<T>(Transform[] parents, Func<T, bool> predicate = null) where T : Component
		{
			List<Object> selection = new List<Object>();
			foreach(var parent in parents)
			{
				foreach(var c in parent.GetComponentsInChildren<T>(true))
				{
					if(predicate == null || predicate.Invoke(c))
					{
						if(!selection.Contains(c.gameObject)) selection.Add(c.gameObject);
					}
				}
			}
			Selection.objects = selection.ToArray();
			//Debug.Log($"Selected {selection.Count} GameObjects.");
		}
	}
}
