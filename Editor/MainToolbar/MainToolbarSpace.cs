#if UNITY_6000_3_OR_NEWER
using UnityEditor.Toolbars;

namespace UnityEssentialsEditor
{
	public class MainToolbarSpace
	{
		[MainToolbarElement("Layout/Space 1", defaultDockPosition = MainToolbarDockPosition.Middle)]
		public static MainToolbarElement CreateSpace1() => Create(false);
		[MainToolbarElement("Layout/Space 2", defaultDockPosition = MainToolbarDockPosition.Middle)]
		public static MainToolbarElement CreateSpace2() => Create(false);
		[MainToolbarElement("Layout/Space 3", defaultDockPosition = MainToolbarDockPosition.Middle)]
		public static MainToolbarElement CreateSpace3() => Create(false);
		[MainToolbarElement("Layout/Space 4", defaultDockPosition = MainToolbarDockPosition.Middle)]
		public static MainToolbarElement CreateSpace4() => Create(false);
		
		[MainToolbarElement("Layout/Space (Wide) 1", defaultDockPosition = MainToolbarDockPosition.Middle)]
		public static MainToolbarElement CreateWideSpace1() => Create(true);
		[MainToolbarElement("Layout/Space (Wide) 2", defaultDockPosition = MainToolbarDockPosition.Middle)]
		public static MainToolbarElement CreateWideSpace2() => Create(true);
		[MainToolbarElement("Layout/Space (Wide) 3", defaultDockPosition = MainToolbarDockPosition.Middle)]
		public static MainToolbarElement CreateWideSpace3() => Create(true);
		[MainToolbarElement("Layout/Space (Wide) 4", defaultDockPosition = MainToolbarDockPosition.Middle)]
		public static MainToolbarElement CreateWideSpace4() => Create(true);
		
		private static MainToolbarElement Create(bool wide)
		{
			string text = wide ? "<color=#00000000>______</color>" : "";
			return new MainToolbarLabel(new MainToolbarContent(text));
		}
	}
}
#endif