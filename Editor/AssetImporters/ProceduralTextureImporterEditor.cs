using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace UnityEssentialsEditor
{
	[CustomEditor(typeof(ProceduralTextureImporter<>), true)]
#if UNITY_2020_2_OR_NEWER
	public class ProceduralTextureImporterEditor : UnityEditor.AssetImporters.ScriptedImporterEditor
#else
	public class ProceduralTextureImporterEditor : UnityEditor.Experimental.AssetImporters.ScriptedImporterEditor
#endif
	{
		private ProceduralTexureFormat format;
		private SerializedObject formatSO;
		private GUIStyle boxStyle;
		private bool hasFormatChanges = false;

		public override void OnEnable()
		{
			base.OnEnable();
			format = ((IProceduralTextureGenerator)target).LoadFormat();
			formatSO = new SerializedObject(format);
		}

		public override void OnInspectorGUI()
		{
			if(boxStyle == null)
			{
				boxStyle = "FrameBox";
			}

			GUILayout.BeginVertical(boxStyle);
			GUILayout.Label("Texture Generation Settings", EditorStyles.boldLabel);
			EditorGUI.BeginChangeCheck();
			DrawFields(formatSO);
			if(EditorGUI.EndChangeCheck()) hasFormatChanges = true;
			GUILayout.EndVertical();

			DrawFields(serializedObject);

			GUILayout.Space(20);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			/*
			//TODO: Broken at the moment
			if(GUILayout.Button("Bake to Texture", GUILayout.Width(150)))
			{
				((IProceduralTextureGenerator)target).Bake();
			}
			*/
			GUILayout.EndHorizontal();

			ApplyRevertGUI();
		}

		public override bool HasModified()
		{
			return base.HasModified() || hasFormatChanges;
		}

		protected override void Apply()
		{
			if(hasFormatChanges)
			{
				var json = format.Write();
				System.IO.File.WriteAllText(((ScriptedImporter)target).assetPath, json);
				hasFormatChanges = false;
			}
			base.Apply();
		}

		public override void DiscardChanges()
		{
			format = ((IProceduralTextureGenerator)target).LoadFormat();
			formatSO = new SerializedObject(format);
			hasFormatChanges = false;
			base.DiscardChanges();
		}

		private void DrawFields(SerializedObject obj)
		{
			obj.Update();
			bool first = true;
			var property = obj.GetIterator();
			while(property.NextVisible(first))
			{
				first = false;
				if(property.name == "m_Script")
					continue;
				EditorGUILayout.PropertyField(property, true);
			}
			obj.ApplyModifiedProperties();
		}
	}
}
