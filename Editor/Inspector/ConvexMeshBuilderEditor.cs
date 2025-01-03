using UnityEssentials.Meshes;
using UnityEssentialsEditor.Tools;
using UnityEditor;
using UnityEngine;
#if UNITY_2020_1_OR_NEWER
using ToolManager = UnityEditor.EditorTools.ToolManager;
#else
using ToolManager = UnityEditor.EditorTools.EditorTools;
#endif

namespace UnityEssentialsEditor.Inspector
{
	[CustomEditor(typeof(ConvexMeshBuilderComponent))]
	internal class ConvexMeshBuilderEditor : Editor
	{
		private ConvexMeshBuilderComponent comp;

		private GUIContent buttonIcon;

		private void OnEnable()
		{
			comp = (ConvexMeshBuilderComponent)target;
			buttonIcon = EditorGUIUtility.IconContent("d_EditCollider");
		}

		public override void OnInspectorGUI()
		{
			GUILayout.BeginHorizontal();
			bool b = ToolManager.activeToolType == typeof(VertexEditorTool);
			GUILayout.Label("Edit Mesh", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
			bool b2 = GUILayout.Toggle(b, buttonIcon, GUI.skin.button, GUILayout.Width(32), GUILayout.Height(22));
			GUILayout.EndHorizontal();
			if(b != b2)
			{
				if(b2) ToolManager.SetActiveTool<VertexEditorTool>();
				else ToolManager.RestorePreviousTool();
			}
			GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

			base.OnInspectorGUI();
		}
	}
}
