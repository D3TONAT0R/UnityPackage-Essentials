using UnityEditor;
using UnityEngine;

namespace D3TEditor {
	public class MassReplacePrefabs : ScriptableWizard {

		public GameObject replace;
		public bool keepName;

		[MenuItem("GameObject/Mass Replace Objects ...")]
		static void CreateWizard() {
			ScriptableWizard.DisplayWizard<MassReplacePrefabs>("Mass Replace", "Apply", "Cancel");
			//If you don't want to use the secondary button simply leave it out:
			//ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
		}

		void OnWizardCreate() {
			foreach(GameObject g in Selection.gameObjects) {
				if(g) {
					Vector3 pos = g.transform.position;
					Quaternion rot = g.transform.rotation;
					Vector3 scale = g.transform.localScale;
					Transform parent = g.transform.parent;
					string n = g.name;
					DestroyImmediate(g);
					GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(replace);
					inst.transform.position = pos;
					inst.transform.rotation = rot;
					inst.transform.parent = parent;
					inst.transform.localScale = scale;
					if(keepName) {
						inst.name = n;
					} else {
						inst.name = replace.name;
					}
				}
			}
		}

		void OnWizardUpdate() {
			helpString = "Pick a GameObject to replace the selected ones.\n Objects to replace: " + Selection.gameObjects.Length;
		}

		private void OnWizardOtherButton() {

		}
	} 
}
