using UnityEngine;

namespace D3T
{
	public static class ColorExtensions
	{
		/// <summary>
		/// Returns this color with a different alpha value.
		/// </summary>
		public static Color WithAlpha(this Color c, float a)
		{
			return new Color(c.r, c.g, c.b, a);
		}

		/// <summary>
		/// Returns this color with a multiplied alpha value.
		/// </summary>
		public static Color MultiplyAlpha(this Color c, float multiplier)
		{
			return new Color(c.r, c.g, c.b, c.a * multiplier);
		}

		/// <summary>
		/// Returns this color with modified saturation levels.
		/// </summary>
		public static Color ScaleSaturation(this Color c, float saturation)
		{
			return Color.LerpUnclamped(Grayscale(c), c, saturation);
		}

		/// <summary>
		/// Returns a grayscaled version of this color.
		/// </summary>
		public static Color Grayscale(this Color c)
		{
			float gray = c.grayscale;
			return new Color(gray, gray, gray, c.a);
		}

		/// <summary>
		/// Returns the hue of this color.
		/// </summary>
		public static float GetHue(this Color c)
		{
			Color.RGBToHSV(c, out var h, out _, out _);
			return h;
		}

		/// <summary>
		/// Returns the saturation of this color.
		/// </summary>
		public static float GetSaturation(this Color c)
		{
			Color.RGBToHSV(c, out _, out var s, out _);
			return s;
		}

		/// <summary>
		/// Returns the value (brightness) of this color.
		/// </summary>
		public static float GetValue(this Color c)
		{
			Color.RGBToHSV(c, out _, out _, out var v);
			return v;
		}

		/// <summary>
		/// Returns this color with a different hue.
		/// </summary>
		public static Color WithHue(this Color c, float hue)
		{
			Color.RGBToHSV(c, out _, out var s, out var v);
			return Color.HSVToRGB(hue, s, v);
		}

		/// <summary>
		/// Returns this color with a different saturation.
		/// </summary>
		public static Color WithSaturation(this Color c, float saturation)
		{
			Color.RGBToHSV(c, out var h, out _, out var v);
			return Color.HSVToRGB(h, saturation, v);
		}

		/// <summary>
		/// Returns this color with a different value (brightness).
		/// </summary>
		public static Color WithValue(this Color c, float value)
		{
			Color.RGBToHSV(c, out var h, out var s, out _);
			return Color.HSVToRGB(h, s, value);
		}

		/// <summary>
		/// Returns this color with its hue shifted.
		/// </summary>
		public static Color WithHueShift(this Color c, float hueShift)
		{
			Color.RGBToHSV(c, out var h, out var s, out var v);
			h = (h + hueShift) % 1;
			return Color.HSVToRGB(h, s, v);
		}

		/// <summary>
		/// Returns this color with reduced precision.
		/// </summary>
		public static Color Reduce(this Color c, int steps, bool includeAlpha = true)
		{
			c.r = Mathf.Round(c.r * steps) / steps;
			c.g = Mathf.Round(c.g * steps) / steps;
			c.b = Mathf.Round(c.b * steps) / steps;
			if(includeAlpha) c.a = Mathf.Round(c.a * steps) / steps;
			return c;
		}
	}
}