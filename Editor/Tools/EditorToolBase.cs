#if UNITY_2020_1_OR_NEWER
using ToolManager = UnityEditor.EditorTools.ToolManager;
#else
using ToolManager = UnityEditor.EditorTools.EditorTools;
#endif
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEditor;

namespace UnityEssentialsEditor.Tools
{
	public abstract class EditorToolBase : EditorTool
	{
		internal const string ESSENTIALS_PACKAGE_ICONS_PATH = "Packages/com.github.d3tonat0r.essentials/Editor/EditorAssets/Icons/";

		public static EditorToolBase Active { get; protected set; }

		public abstract bool ShowToolWindow { get; }
		public virtual string ToolWindowTitle => ObjectNames.NicifyVariableName(GetType().Name);
		public virtual int ToolWindowWidth => 250;
		public virtual bool EscapeKeyDeactivatesTool => true;

		private Rect lastWindowRect;

		protected bool AllowClicks { get; private set; }

		public override GUIContent toolbarIcon
		{
			get
			{
				if(_toolbarIcon == null)
				{
					var path = ToolbarIconPath;
					if(!string.IsNullOrEmpty(path))
					{
						var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
						if(icon == null) icon = Texture2D.whiteTexture;
						_toolbarIcon = new GUIContent(icon);
					}
					else
					{
						_toolbarIcon = new GUIContent("?");
					}
				}
				return _toolbarIcon;
			}
		}

		private GUIContent _toolbarIcon;

		public virtual string ToolbarIconPath => null;

		protected void OnEnable()
		{
#if !UNITY_2020_2_OR_NEWER
			ToolManager.activeToolChanged += () =>
			{
				if(ToolManager.IsActiveTool(this))
				{
					Active = this;
					OnBecameActive();
				}
			};
			ToolManager.activeToolChanging += () =>
			{
				if(ToolManager.IsActiveTool(this))
				{
					OnWillDeactivate();
					Active = null;
				}
			};
			if(ToolManager.IsActiveTool(this))
			{
				OnBecameActive();
			}
#endif
		}

#if UNITY_2020_2_OR_NEWER
		public override void OnActivated()
		{
			Active = this;
			OnBecameActive();
		}

		public override void OnWillBeDeactivated()
		{
			OnWillDeactivate();
			Active = null;
		}
#endif

		protected virtual void OnBecameActive()
		{

		}

		protected virtual void OnWillDeactivate()
		{

		}

		public sealed override void OnToolGUI(EditorWindow window)
		{
			DrawSceneGUI(window);
			Handles.color = Color.white;
			Handles.matrix = Matrix4x4.identity;
			//If on 2021.2 or newer, the window must be implemented via overlays
#if !UNITY_2021_2_OR_NEWER
			DrawWindowIfEnabled();
#endif
			GUI.enabled = true;
			AfterSceneGUI(window, AllowClicks);
			if(EscapeKeyDeactivatesTool && KeyUp(KeyCode.Escape))
			{
				Event.current.Use();
				Deactivate();
			}
		}

		public void Deactivate()
		{
			ToolManager.RestorePreviousTool();
		}

#if !UNITY_2021_2_OR_NEWER
		private void DrawWindowIfEnabled()
		{
			if(ShowToolWindow)
			{
				EditorGUIUtility.wideMode = ToolWindowWidth >= 250;
				Handles.BeginGUI();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical(ToolWindowTitle, GUI.skin.window, GUILayout.Width(ToolWindowWidth), GUILayout.MaxWidth(ToolWindowWidth));
				OnWindowGUI();
				GUILayout.EndVertical();
				lastWindowRect = GUILayoutUtility.GetLastRect();
				GUI.Button(lastWindowRect, GUIContent.none, GUIStyle.none);
				GUILayout.Space(25);
				GUILayout.EndVertical();
				GUILayout.Space(5);
				GUILayout.EndHorizontal();
				Handles.EndGUI();
			}
			else
			{
				lastWindowRect = Rect.zero;
			}
		}
#endif

		private void DrawSceneGUI(EditorWindow window)
		{
			if(Event.current.type == EventType.Layout)
			{
				//Disable handle interaction when the mouse is inside the window
				AllowClicks = !lastWindowRect.Contains(Event.current.mousePosition);
			}
			GUI.enabled = AllowClicks;
			OnSceneGUI(window, AllowClicks);
			GUI.enabled = true;
		}

		public abstract void OnWindowGUI();
		protected abstract void OnSceneGUI(EditorWindow window, bool enableInteraction);
		protected virtual void AfterSceneGUI(EditorWindow window, bool enableInteraction) { }

		protected bool ShiftDown => Event.current.shift;
		protected bool CtrlDown
		{
			get
			{
				if(Application.platform != RuntimePlatform.OSXEditor) return Event.current.control;
				else return Event.current.command;
			}
		}
		protected bool AltDown => Event.current.alt;

		protected bool KeyDown(KeyCode k)
		{
			return Event.current.type == EventType.KeyDown && Event.current.keyCode == k;
		}

		protected bool KeyUp(KeyCode k)
		{
			return Event.current.type == EventType.KeyUp && Event.current.keyCode == k;
		}

		protected bool AnyKeyDown(params KeyCode[] keys)
		{
			return CheckKeys(EventType.KeyDown, keys);
		}

		protected bool AnyKeyUp(params KeyCode[] keys)
		{
			return CheckKeys(EventType.KeyUp, keys);
		}

		private bool CheckKeys(EventType t, KeyCode[] keys)
		{
			if(keys.Length == 0)
			{
				return Event.current.type == t;
			}
			else
			{
				if(Event.current.type == t)
				{
					foreach(var k in keys)
					{
						if(Event.current.keyCode == k) return true;
					}
				}
				return false;
			}
		}

		protected bool MouseDown(int button)
		{
			return AllowClicks && Event.current.type == EventType.MouseDown && Event.current.button == button;
		}

		protected bool MouseUp(int button)
		{
			return AllowClicks && Event.current.type == EventType.MouseUp && Event.current.button == button;
		}

		protected void UseEvent()
		{
			Event.current.Use();
		}
	}
}
