using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEssentialsEditor
{
	internal static class DuplicateCustom
	{

		[MenuItem("Edit/Duplicate Custom %#d", priority = 120)]
		public static void DuplicateGameObjectCommand()
		{
			if (Selection.activeGameObject is GameObject)
			{
				var selection = new List<Object>();
				var objectsToDuplicate = new List<GameObject>(Selection.gameObjects);
				foreach (var go in objectsToDuplicate)
				{
					//Duplicate object
					GameObject duplicate = Duplicate(go);
					//Move directly underneath original
					duplicate.transform.SetSiblingIndex(
					go.transform.GetSiblingIndex() + 1);
					//Rename and increment
					duplicate.name = IncrementName(go, go.name);
					//Select new object
					selection.Add(duplicate);
					//Register Undo
					if (Selection.gameObjects.Length == 1)
					{
						Undo.RegisterCreatedObjectUndo(duplicate, "Duplicate GameObject");
					}
					else
					{
						Undo.RegisterCreatedObjectUndo(duplicate, "Duplicate " + Selection.gameObjects.Length + " GameObjects");
					}
				}
				Selection.objects = selection.ToArray();
			}
			else
			{
				//Don't break default behaviour elsewhere
				EditorApplication.ExecuteMenuItem("Edit/Duplicate");
			}
		}

		private static GameObject Duplicate(GameObject target)
		{
			Selection.activeGameObject = target;
			EditorApplication.ExecuteMenuItem("Edit/Duplicate");
			//Unsupported.DuplicateGameObjectsUsingPasteboard();
			return Selection.activeGameObject;

			/*GameObject go = PrefabUtility.GetCorrespondingObjectFromSource(target);

			if (go)
			{
				go = PrefabUtility.InstantiatePrefab(go, target.transform.parent) as GameObject;
			}
			else
			{
				go = Object.Instantiate(target, target.transform.parent);
				go.name = target.name;
			}
			return go;*/
		}

		private static string IncrementName(GameObject go, string input)
		{

			string pars = null;
			if (input.EndsWith(")"))
			{
				pars = "()";
			}
			else if (input.EndsWith("]"))
			{
				pars = "[]";
			}
			else if (input.EndsWith(">"))
			{
				pars = "<>";
			}

			if (pars != null)
			{
				input = input.TrimEnd(pars[1]);
			}

			var numStr = new System.Text.StringBuilder("");
			bool hasNumber = false;
			while (char.IsDigit(input[input.Length - 1]) && numStr.Length < 5)
			{
				hasNumber = true;
				numStr.Insert(0, input[input.Length - 1]);
				input = input.Substring(0, input.Length - 1);
			}
			if (!hasNumber)
			{
				numStr.Append("1");
				input += " ";
			}

			int num = int.Parse(numStr.ToString());
			num++;

			string newname = input + num.ToString().PadLeft(numStr.Length, '0') + (pars != null ? pars[1].ToString() : "");

			if (ChildExists(go.scene, go.transform.parent, newname))
			{
				return IncrementName(go, newname);
			}

			return newname;
		}

		private static bool ChildExists(Scene scene, Transform parent, string name)
		{
			List<Transform> children = new List<Transform>();
			if (parent)
			{
				for (int i = 0; i < parent.childCount; i++)
				{
					children.Add(parent.GetChild(i));
				}
			}
			else
			{
				foreach (var c in scene.GetRootGameObjects())
				{
					children.Add(c.transform);
				}
			}
			foreach (var t in children)
			{
				if (t.name == name) return true;
			}
			return false;
		}
	}

}