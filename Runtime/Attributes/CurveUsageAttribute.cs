using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Color for an AnimationCurve.
	/// </summary>
	public enum CurveColor : uint
	{
		Red,
		Green,
		Blue,
		Yellow,
		Magenta,
		Cyan,
		White,
		Gray
	}

	/// <summary>
	/// Allows for setting minimum and maximum ranges and optional color for an AnimationCurve.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class CurveUsageAttribute : PropertyAttribute
	{
		public readonly Color color = Color.clear;
		public readonly Rect ranges = Rect.zero;

		public CurveUsageAttribute(float xMin, float yMin, float xMax, float yMax)
		{
			ranges = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
			color = Color.green;
		}

		public CurveUsageAttribute(float xMin, float yMin, float xMax, float yMax, CurveColor color) : this(xMin, yMin, xMax, yMax)
		{
			this.color = GetColor(color);
		}

		public CurveUsageAttribute(float xMin, float yMin, float xMax, float yMax, uint colorRGB) : this(xMin, yMin, xMax, yMax)
		{
			color = GetColor(colorRGB);
		}

		public CurveUsageAttribute(CurveColor color)
		{
			this.color = GetColor(color);
		}

		public CurveUsageAttribute(uint colorRGB)
		{
			color = GetColor(colorRGB);
		}

		private Color GetColor(CurveColor color)
		{
			Color col;
			switch(color)
			{
				case CurveColor.Red: col = Color.red; break;
				case CurveColor.Green: col = Color.green; break;
				case CurveColor.Blue: col = Color.blue; break;
				case CurveColor.Yellow: col = Color.yellow; break;
				case CurveColor.Magenta: col = Color.magenta; break;
				case CurveColor.Cyan: col = Color.cyan; break;
				case CurveColor.White: col = Color.white; break;
				case CurveColor.Gray: col = Color.gray; break;
				default: col = Color.white; break;
			}
			return Color.Lerp(Color.white, col, 0.95f);
		}

		private Color GetColor(uint rgb)
		{
			return new Color32(
				(byte)(rgb >> 16 & 0xFF),
				(byte)(rgb >> 8 & 0xFF),
				(byte)(rgb & 0xFF),
				255);
		}
	}
}
