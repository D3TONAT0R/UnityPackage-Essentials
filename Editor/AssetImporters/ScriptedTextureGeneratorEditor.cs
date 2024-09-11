using D3TEditor;
using UnityEditor;

using UnityEngine;

namespace D3T
{
	[CustomEditor(typeof(ScriptedTextureGenerator), true)]
	public class ScriptedTextureGeneratorEditor : UnityEditor.AssetImporters.ScriptedImporterEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			GUILayout.Space(20);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Bake to Texture", GUILayout.Width(150)))
			{
				((ScriptedTextureGenerator)target).Bake();
			}
			GUILayout.EndHorizontal();
		}
	}
}
