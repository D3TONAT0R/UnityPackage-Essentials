using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// A collection of predefined colors.
	/// </summary>
	public static class Colors
	{
		private const float H = 0.5f;
		private const float Q1 = 0.25f;
		private const float Q3 = 0.75f;

		/// <summary>
		/// Pure white at 100% brightness.
		/// </summary>
		public static readonly Color white = Color.white;
		/// <summary>
		/// Light gray at 75% brightness.
		/// </summary>
		public static readonly Color lightGray = new Color(Q3, Q3, Q3);
		/// <summary>
		/// Medium gray at 50% brightness.
		/// </summary>
		public static readonly Color gray = new Color(H, H, H);
		/// <summary>
		/// Dark gray at 25% brightness.
		/// </summary>
		public static readonly Color darkGray = new Color(Q1, Q1, Q1);
		/// <summary>
		/// Black at 0% brightness.
		/// </summary>
		public static readonly Color black = new Color(0, 0, 0);

		/// <summary>
		/// Fully transparent white.
		/// </summary>
		public static readonly Color clearWhite = new Color(1, 1, 1, 0);
		/// <summary>
		/// Fully transparent black.
		/// </summary>
		public static readonly Color clearBlack = new Color(0, 0, 0, 0);
		/// <summary>
		/// Fully transparent medium gray.
		/// </summary>
		public static readonly Color clearGray = new Color(H, H, H, 0);

		/// <summary>
		/// Pure red. Hue: 0/360°
		/// </summary>
		public static readonly Color red = new Color(1, 0, 0);
		/// <summary>
		/// 100% R, 50% G. Hue: 30°
		/// </summary>
		public static readonly Color orange = new Color(1, H, 0);
		/// <summary>
		/// 100% R, 100% G. Hue: 60°
		/// </summary>
		public static readonly Color yellow = new Color(1, 1, 0);
		/// <summary>
		/// 50% G, 100% R. Hue: 90°
		/// </summary>
		public static readonly Color lime = new Color(H, 1, 0);
		/// <summary>
		/// Pure green. Hue: 120°
		/// </summary>
		public static readonly Color green = new Color(0, 1, 0);
		/// <summary>
		/// 100% G, 50% B. Hue: 150°
		/// </summary>
		public static readonly Color emerald = new Color(0, 1, H);
		/// <summary>
		/// 100% G, 100% B. Hue: 180°
		/// </summary>
		public static readonly Color cyan = new Color(0, 1, 1);
		/// <summary>
		/// 50% G, 100% B. Hue: 210°
		/// </summary>
		public static readonly Color azure = new Color(0, H, 1);
		/// <summary>
		/// Pure blue. Hue: 240°
		/// </summary>
		public static readonly Color blue = new Color(0, 0, 1);
		/// <summary>
		/// 50% R, 100% B. Hue: 270°
		/// </summary>
		public static readonly Color purple = new Color(H, 0, 1);
		/// <summary>
		/// 100% R, 100% B. Hue: 300°
		/// </summary>
		public static readonly Color magenta = new Color(1, 0, 1);
		/// <summary>
		/// 100% R, 50% B. Hue: 330°
		/// </summary>
		public static readonly Color pink = new Color(1, 0, H);

		/// <summary>
		/// A 50% darker version of <see cref="red"/>.
		/// </summary>
		public static readonly Color darkRed = red.Darken(0.5f);
		/// <summary>
		/// A 50% darker version of <see cref="orange"/>.
		/// </summary>
		public static readonly Color darkOrange = orange.Darken(0.5f);
		/// <summary>
		/// A 50% darker version of <see cref="yellow"/>.
		/// </summary>
		public static readonly Color darkYellow = yellow.Darken(0.5f);
		/// <summary>
		/// A 50% darker version of <see cref="lime"/>.
		/// </summary>
		public static readonly Color darkLime = lime.Darken(0.5f);
		/// <summary>
		/// A 50% darker version of <see cref="green"/>.
		/// </summary>
		public static readonly Color darkGreen = green.Darken(0.5f);
		/// <summary>
		/// A 50% darker version of <see cref="emerald"/>.
		/// </summary>
		public static readonly Color darkEmerald = emerald.Darken(0.5f);
		/// <summary>
		/// A 50% darker version of <see cref="cyan"/>.
		/// </summary>
		public static readonly Color darkCyan = cyan.Darken(0.5f);
		/// <summary>
		/// A 50% darker version of <see cref="azure"/>.
		/// </summary>
		public static readonly Color darkAzure = azure.Darken(0.5f);
		/// <summary>
		/// A 50% darker version of <see cref="blue"/>.
		/// </summary>
		public static readonly Color darkBlue = blue.Darken(0.5f);
		/// <summary>
		/// A 50% darker version of <see cref="purple"/>.
		/// </summary>
		public static readonly Color darkPurple = purple.Darken(0.5f);
		/// <summary>
		/// A 50% darker version of <see cref="magenta"/>.
		/// </summary>
		public static readonly Color darkMagenta = magenta.Darken(0.5f);
		/// <summary>
		/// A 50% darker version of <see cref="pink"/>.
		/// </summary>
		public static readonly Color darkPink = pink.Darken(0.5f);


		/// <summary>
		/// A 50% lighter version of <see cref="red"/>.
		/// </summary>
		public static readonly Color lightRed = red.Lighten(0.5f);
		/// <summary>
		/// A 50% lighter version of <see cref="orange"/>.
		/// </summary>
		public static readonly Color lightOrange = orange.Lighten(0.5f);
		/// <summary>
		/// A 50% lighter version of <see cref="yellow"/>.
		/// </summary>
		public static readonly Color lightYellow = yellow.Lighten(0.5f);
		/// <summary>
		/// A 50% lighter version of <see cref="lime"/>.
		/// </summary>
		public static readonly Color lightLime = lime.Lighten(0.5f);
		/// <summary>
		/// A 50% lighter version of <see cref="green"/>.
		/// </summary>
		public static readonly Color lightGreen = green.Lighten(0.5f);
		/// <summary>
		/// A 50% lighter version of <see cref="emerald"/>.
		/// </summary>
		public static readonly Color lightEmerald = emerald.Lighten(0.5f);
		/// <summary>
		/// A 50% lighter version of <see cref="cyan"/>.
		/// </summary>
		public static readonly Color lightCyan = cyan.Lighten(0.5f);
		/// <summary>
		/// A 50% lighter version of <see cref="azure"/>.
		/// </summary>
		public static readonly Color lightAzure = azure.Lighten(0.5f);
		/// <summary>
		/// A 50% lighter version of <see cref="blue"/>.
		/// </summary>
		public static readonly Color lightBlue = blue.Lighten(0.5f);
		/// <summary>
		/// A 50% lighter version of <see cref="purple"/>.
		/// </summary>
		public static readonly Color lightPurple = purple.Lighten(0.5f);
		/// <summary>
		/// A 50% lighter version of <see cref="magenta"/>.
		/// </summary>
		public static readonly Color lightMagenta = magenta.Lighten(0.5f);
		/// <summary>
		/// A 50% lighter version of <see cref="pink"/>.
		/// </summary>
		public static readonly Color lightPink = pink.Lighten(0.5f);
	}
}