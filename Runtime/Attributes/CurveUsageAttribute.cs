using System;
using UnityEngine;

namespace D3T
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
		public readonly Color color;
		public readonly Rect ranges;

		public CurveUsageAttribute(float xMin, float yMin, float xMax, float yMax)
		{
			ranges = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
			color = Color.green;
		}

		public CurveUsageAttribute(float xMin, float yMin, float xMax, float yMax, CurveColor color)  : this(xMin, yMin, xMax, yMax)
		{
			this.color = GetColor(color);
		}

		private Color GetColor(CurveColor color)
		{
			Color col;
			switch(color)
			{
				case CurveColor.Red: col =  Color.red; break;
				case CurveColor.Green: col =  Color.green; break;
				case CurveColor.Blue: col =  Color.blue; break;
				case CurveColor.Yellow: col =  Color.yellow; break;
				case CurveColor.Magenta: col =  Color.magenta; break;
				case CurveColor.Cyan: col =  Color.cyan; break;
				case CurveColor.White: col =  Color.white; break;
				case CurveColor.Gray: col =  Color.gray; break;
				default: col =  Color.white; break;
			}
			return Color.Lerp(Color.white, col, 0.95f);
		}
	}
}
