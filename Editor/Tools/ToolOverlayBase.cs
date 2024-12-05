using UnityEditor.EditorTools;
using UnityEditor.Overlays;
using UnityEngine;

namespace D3TEditor.Tools
{
#if UNITY_2021_2_OR_NEWER
	public abstract class ToolOverlayBase<T> : IMGUIOverlay, ITransientOverlay where T : EditorToolBase
	{
		public virtual bool visible => ToolManager.activeToolType == typeof(T) && EditorToolBase.Active != null && EditorToolBase.Active.ShowToolWindow;

		protected Vector2 scroll;

		public override void OnGUI()
		{
			var tool = EditorToolBase.Active;
			if(tool == null || tool.GetType() != typeof(T)) return;
			using(var scrollScope = new GUILayout.ScrollViewScope(scroll, GUILayout.Width(tool.ToolWindowWidth)))
			{
				scroll = scrollScope.scrollPosition;
				tool.OnWindowGUI();
			}
		}
	}
#endif
}
