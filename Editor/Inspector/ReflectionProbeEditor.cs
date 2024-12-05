using UnityEssentialsEditor.Tools;
using System;
using UnityEditor;
using UnityEngine;
#if UNITY_2020_1_OR_NEWER
using ToolManager = UnityEditor.EditorTools.ToolManager;
#else
using ToolManager = UnityEditor.EditorTools.EditorTools;
#endif

namespace UnityEssentialsEditor.Inspector
{
	[CustomEditor(typeof(ReflectionProbe))]
	[CanEditMultipleObjects]
	public class ReflectionProbeEditor : ExtendedEditor
	{
		protected override Type BaseType => Type.GetType("UnityEditor.ReflectionProbeEditor, UnityEditor", true);

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnBeforeDefaultInspectorGUI()
		{
			Rect r = new Rect(EditorGUIUtility.currentViewWidth / 2f + 40, 4, 30, 20);
			bool b = ToolManager.activeToolType == typeof(ReflectionProbeBoundsDefinitionTool);
			bool b2 = GUI.Toggle(r, b, BoundsDefinitionTool.Icon, GUI.skin.button);
			if(b2 != b)
			{
				if(b2) ToolManager.SetActiveTool<ReflectionProbeBoundsDefinitionTool>();
				else ToolManager.RestorePreviousTool();
			}
		}
	}
}