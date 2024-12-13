using UnityEditor;
using UnityEngine;

namespace D3T
{
	public static class Colors
	{
		private const float H = 0.5f;
		private const float Q1 = 0.25f;
		private const float Q3 = 0.75f;

		public static readonly Color white = Color.white;
		public static readonly Color lightGray = new Color(Q3, Q3, Q3);
		public static readonly Color gray = new Color(H, H, H);
		public static readonly Color darkGray = new Color(Q1, Q1, Q1);

		public static readonly Color clearWhite = new Color(1, 1, 1, 0);
		public static readonly Color clearBlack = new Color(0, 0, 0, 0);
		public static readonly Color clearGray = new Color(H, H, H, 0);

		public static readonly Color red = new Color(1, 0, 0);
		public static readonly Color orange = new Color(1, H, 0);
		public static readonly Color yellow = new Color(1, 1, 0);
		public static readonly Color lime = new Color(H, 1, 0);
		public static readonly Color green = new Color(0, 1, 0);
		public static readonly Color aqua = new Color(0, 1, H);
		public static readonly Color cyan = new Color(0, 1, 1);
		//TODO: name
		public static readonly Color cyanBlue = new Color(0, H, 1);
		public static readonly Color blue = new Color(0, 0, 1);
		public static readonly Color purple = new Color(H, 0, 1);
		public static readonly Color magenta = new Color(1, 0, 1);
		//TODO: name
		public static readonly Color magenta2 = new Color(1, 0, H);
	}
}