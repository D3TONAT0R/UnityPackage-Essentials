using System;
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
		/// Pure black at 0% brightness.
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

		#region Darker Colors
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
		#endregion

		#region Lighter colors
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
		#endregion

		#region Color conversions
		/// <summary>
		/// Converts the given color to a 32-bit integer.
		/// </summary>
		public static int ToInt(Color32 color, bool includeAlpha = true)
		{
			if(includeAlpha)
			{
				return (color.r << 24) | (color.g << 16) | (color.b << 8) | color.a;
			}
			else
			{
				return (color.r << 16) | (color.g << 8) | color.b;
			}
		}

		/// <summary>
		/// Converts the given color to hex code.
		/// </summary>
		public static string ToHex(Color32 color, bool includeAlpha = true)
		{
			int i = ToInt(color, includeAlpha);
			return Convert.ToString(i, 16).ToUpper().PadLeft(includeAlpha ? 8 : 6, '0');
		}

		/// <summary>
		/// Creates a color from the given 32-bit integer.
		/// </summary>
		public static Color32 FromInt(int value, bool includeAlpha = true)
		{
			if(includeAlpha)
			{
				byte r = (byte)((value >> 24) & 0xFF);
				byte g = (byte)((value >> 16) & 0xFF);
				byte b = (byte)((value >> 8) & 0xFF);
				byte a = (byte)(value & 0xFF);
				return new Color32(r, g, b, a);
			}
			else
			{
				byte r = (byte)((value >> 16) & 0xFF);
				byte g = (byte)((value >> 8) & 0xFF);
				byte b = (byte)(value & 0xFF);
				return new Color32(r, g, b, 255);
			}
		}

		/// <summary>
		/// Creates a color from the given hex code.
		/// </summary>
		public static Color32 FromHex(string hex)
		{
			bool includeAlpha;
			if(hex.Length == 8) includeAlpha = true;
			else if(hex.Length == 6) includeAlpha = false;
			else throw new ArgumentException("Color hex code must be either 6 or 8 characters long.");
			int i = Convert.ToInt32(hex, 16);
			return FromInt(i, includeAlpha);
		}
		#endregion

		#region Color Temperature
		/// <summary>
		/// The minimum temperature covered by the correlated color temperature (CCT) conversion.
		/// </summary>
		public const int MIN_COLOR_TEMPERATURE = 1000;
		/// <summary>
		/// The maximum temperature covered by the correlated color temperature (CCT) conversion.
		/// </summary>
		public const int MAX_COLOR_TEMPERATURE = 15000;

		//Lookup table generated using https://gist.github.com/ibober/6b5a6e1dea888c01c0af175e71b15fa4
		private static readonly Color32[] temperatureLookupTable = new Color32[]
		{
			new Color32(255, 067, 000, 255), // 1000K
			new Color32(255, 077, 000, 255), // 1100K
			new Color32(255, 086, 000, 255), // 1200K
			new Color32(255, 094, 000, 255), // 1300K
			new Color32(255, 101, 000, 255), // 1400K
			new Color32(255, 108, 000, 255), // 1500K
			new Color32(255, 114, 000, 255), // 1600K
			new Color32(255, 120, 000, 255), // 1700K
			new Color32(255, 126, 000, 255), // 1800K
			new Color32(255, 131, 000, 255), // 1900K
			new Color32(255, 136, 013, 255), // 2000K
			new Color32(255, 141, 027, 255), // 2100K
			new Color32(255, 146, 039, 255), // 2200K
			new Color32(255, 150, 050, 255), // 2300K
			new Color32(255, 155, 060, 255), // 2400K
			new Color32(255, 159, 070, 255), // 2500K
			new Color32(255, 162, 079, 255), // 2600K
			new Color32(255, 166, 087, 255), // 2700K
			new Color32(255, 170, 095, 255), // 2800K
			new Color32(255, 173, 102, 255), // 2900K
			new Color32(255, 177, 109, 255), // 3000K
			new Color32(255, 180, 116, 255), // 3100K
			new Color32(255, 183, 123, 255), // 3200K
			new Color32(255, 186, 129, 255), // 3300K
			new Color32(255, 189, 135, 255), // 3400K
			new Color32(255, 192, 140, 255), // 3500K
			new Color32(255, 195, 146, 255), // 3600K
			new Color32(255, 198, 151, 255), // 3700K
			new Color32(255, 200, 156, 255), // 3800K
			new Color32(255, 203, 161, 255), // 3900K
			new Color32(255, 205, 166, 255), // 4000K
			new Color32(255, 208, 170, 255), // 4100K
			new Color32(255, 210, 175, 255), // 4200K
			new Color32(255, 213, 179, 255), // 4300K
			new Color32(255, 215, 183, 255), // 4400K
			new Color32(255, 217, 187, 255), // 4500K
			new Color32(255, 219, 191, 255), // 4600K
			new Color32(255, 221, 195, 255), // 4700K
			new Color32(255, 223, 198, 255), // 4800K
			new Color32(255, 226, 202, 255), // 4900K
			new Color32(255, 228, 205, 255), // 5000K
			new Color32(255, 229, 209, 255), // 5100K
			new Color32(255, 231, 212, 255), // 5200K
			new Color32(255, 233, 215, 255), // 5300K
			new Color32(255, 235, 219, 255), // 5400K
			new Color32(255, 237, 222, 255), // 5500K
			new Color32(255, 239, 225, 255), // 5600K
			new Color32(255, 241, 228, 255), // 5700K
			new Color32(255, 242, 231, 255), // 5800K
			new Color32(255, 244, 234, 255), // 5900K
			new Color32(255, 246, 236, 255), // 6000K
			new Color32(255, 247, 239, 255), // 6100K
			new Color32(255, 249, 242, 255), // 6200K
			new Color32(255, 251, 244, 255), // 6300K
			new Color32(255, 252, 247, 255), // 6400K
			new Color32(255, 254, 250, 255), // 6500K
			new Color32(255, 255, 255, 255), // 6600K
			new Color32(254, 248, 255, 255), // 6700K
			new Color32(249, 246, 255, 255), // 6800K
			new Color32(246, 244, 255, 255), // 6900K
			new Color32(242, 242, 255, 255), // 7000K
			new Color32(239, 240, 255, 255), // 7100K
			new Color32(236, 238, 255, 255), // 7200K
			new Color32(234, 237, 255, 255), // 7300K
			new Color32(231, 236, 255, 255), // 7400K
			new Color32(229, 234, 255, 255), // 7500K
			new Color32(227, 233, 255, 255), // 7600K
			new Color32(226, 232, 255, 255), // 7700K
			new Color32(224, 231, 255, 255), // 7800K
			new Color32(222, 230, 255, 255), // 7900K
			new Color32(221, 229, 255, 255), // 8000K
			new Color32(219, 228, 255, 255), // 8100K
			new Color32(218, 228, 255, 255), // 8200K
			new Color32(217, 227, 255, 255), // 8300K
			new Color32(215, 226, 255, 255), // 8400K
			new Color32(214, 225, 255, 255), // 8500K
			new Color32(213, 225, 255, 255), // 8600K
			new Color32(212, 224, 255, 255), // 8700K
			new Color32(211, 224, 255, 255), // 8800K
			new Color32(210, 223, 255, 255), // 8900K
			new Color32(209, 222, 255, 255), // 9000K
			new Color32(208, 222, 255, 255), // 9100K
			new Color32(207, 221, 255, 255), // 9200K
			new Color32(206, 221, 255, 255), // 9300K
			new Color32(206, 220, 255, 255), // 9400K
			new Color32(205, 220, 255, 255), // 9500K
			new Color32(204, 219, 255, 255), // 9600K
			new Color32(203, 219, 255, 255), // 9700K
			new Color32(203, 218, 255, 255), // 9800K
			new Color32(202, 218, 255, 255), // 9900K
			new Color32(201, 218, 255, 255), // 10000K
			new Color32(201, 217, 255, 255), // 10100K
			new Color32(200, 217, 255, 255), // 10200K
			new Color32(199, 216, 255, 255), // 10300K
			new Color32(199, 216, 255, 255), // 10400K
			new Color32(198, 216, 255, 255), // 10500K
			new Color32(197, 215, 255, 255), // 10600K
			new Color32(197, 215, 255, 255), // 10700K
			new Color32(196, 215, 255, 255), // 10800K
			new Color32(196, 214, 255, 255), // 10900K
			new Color32(195, 214, 255, 255), // 11000K
			new Color32(195, 214, 255, 255), // 11100K
			new Color32(194, 213, 255, 255), // 11200K
			new Color32(194, 213, 255, 255), // 11300K
			new Color32(193, 213, 255, 255), // 11400K
			new Color32(193, 212, 255, 255), // 11500K
			new Color32(192, 212, 255, 255), // 11600K
			new Color32(192, 212, 255, 255), // 11700K
			new Color32(191, 212, 255, 255), // 11800K
			new Color32(191, 211, 255, 255), // 11900K
			new Color32(191, 211, 255, 255), // 12000K
			new Color32(190, 211, 255, 255), // 12100K
			new Color32(190, 210, 255, 255), // 12200K
			new Color32(189, 210, 255, 255), // 12300K
			new Color32(189, 210, 255, 255), // 12400K
			new Color32(189, 210, 255, 255), // 12500K
			new Color32(188, 209, 255, 255), // 12600K
			new Color32(188, 209, 255, 255), // 12700K
			new Color32(187, 209, 255, 255), // 12800K
			new Color32(187, 209, 255, 255), // 12900K
			new Color32(187, 209, 255, 255), // 13000K
			new Color32(186, 208, 255, 255), // 13100K
			new Color32(186, 208, 255, 255), // 13200K
			new Color32(186, 208, 255, 255), // 13300K
			new Color32(185, 208, 255, 255), // 13400K
			new Color32(185, 207, 255, 255), // 13500K
			new Color32(185, 207, 255, 255), // 13600K
			new Color32(184, 207, 255, 255), // 13700K
			new Color32(184, 207, 255, 255), // 13800K
			new Color32(184, 207, 255, 255), // 13900K
			new Color32(183, 206, 255, 255), // 14000K
			new Color32(183, 206, 255, 255), // 14100K
			new Color32(183, 206, 255, 255), // 14200K
			new Color32(183, 206, 255, 255), // 14300K
			new Color32(182, 206, 255, 255), // 14400K
			new Color32(182, 206, 255, 255), // 14500K
			new Color32(182, 205, 255, 255), // 14600K
			new Color32(181, 205, 255, 255), // 14700K
			new Color32(181, 205, 255, 255), // 14800K
			new Color32(181, 205, 255, 255), // 14900K
			new Color32(181, 205, 255, 255)  // 15000K
		};

		/// <summary>
		/// Gets the correlated color temperature (CCT) for the given temperature.
		/// </summary>
		public static Color FromTemperature(float temperature)
		{
			//Clamp temperature between min and max
			temperature = Mathf.Clamp(temperature, MIN_COLOR_TEMPERATURE, MAX_COLOR_TEMPERATURE);
			float s = (temperature - MIN_COLOR_TEMPERATURE) / 100f;
			int i = Mathf.FloorToInt(s);
			if(Mathf.Abs(s - i) < 0.001f)
			{
				return temperatureLookupTable[i];
			}
			return Color.Lerp(temperatureLookupTable[i], temperatureLookupTable[i + 1], s - i);
		}
		#endregion
	}
}