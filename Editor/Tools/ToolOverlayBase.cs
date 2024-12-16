#if UNITY_2021_2_OR_NEWER
using UnityEditor.EditorTools;
using UnityEditor.Overlays;
using UnityEngine;

namespace UnityEssentialsEditor.Tools
{
	public abstract class ToolOverlayBase<T> : IMGUIOverlay, ITransientOverlay where T : EditorToolBase
	{
		public virtual bool visible => IsActiveToolValidForOverlay && EditorToolBase.Active != null && EditorToolBase.Active.ShowToolWindow;

		protected Vector2 scroll;

		protected bool IsActiveToolValidForOverlay => typeof(T).IsAssignableFrom(ToolManager.activeToolType);

		public override void OnGUI()
		{
			var tool = EditorToolBase.Active;
			if(tool == null || !IsActiveToolValidForOverlay) return;
			using(var scrollScope = new GUILayout.ScrollViewScope(scroll, GUILayout.Width(tool.ToolWindowWidth)))
			{
				scroll = scrollScope.scrollPosition;
				tool.OnWindowGUI();
			}
		}
	}
}
#endif
