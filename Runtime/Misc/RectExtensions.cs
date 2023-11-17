using UnityEngine;

namespace UnityEssentials
{
	public static class RectExtensions
	{

		/// <summary>
		/// Scales this rect's position and size by the given value.
		/// </summary>
		public static Rect Scale(this Rect r, float value)
		{
			Rect r2 = new Rect(r);
			r2.position *= value;
			r2.size *= value;
			return r2;
		}

		/// <summary>
		/// Snaps this rect's corners to the nearest integer coordinates.
		/// </summary>
		public static Rect Snap(this Rect r)
		{
			Rect r2 = new Rect(r);
			r2.x = Mathf.Round(r2.x);
			r2.y = Mathf.Round(r2.y);
			r2.width = Mathf.Round(r2.width);
			r2.height = Mathf.Round(r2.height);
			return r2;
		}

		/// <summary>
		/// Splits this rect horizontally by a given width from the left.
		/// </summary>
		public static void SplitHorizontal(this Rect r, float leftRectWidth, out Rect left, out Rect right, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			left = new Rect(r)
			{
				width = leftRectWidth
			};
			right = new Rect(r)
			{
				xMin = r.xMin + leftRectWidth + margin
			};
		}

		/// <summary>
		/// Splits this rect horizontally by a given width from the right.
		/// </summary>
		public static void SplitHorizontalRight(this Rect r, float rightRectWidth, out Rect left, out Rect right, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			SplitHorizontal(r, r.width - rightRectWidth - margin, out left, out right, margin);
		}

		/// <summary>
		/// Splits this rect horizontally by a given ratio.
		/// </summary>
		public static void SplitHorizontalRelative(this Rect r, float leftRectRatio, out Rect left, out Rect right, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			float leftWidth = r.width * leftRectRatio;
			r.SplitHorizontal(leftWidth, out left, out right, margin);
		}

		/// <summary>
		/// Splits this rect vertically by a given height from the top.
		/// </summary>
		public static void SplitVertical(this Rect r, float topRectHeight, out Rect top, out Rect bottom, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			top = new Rect(r)
			{
				height = topRectHeight
			};
			bottom = new Rect(r)
			{
				yMin = r.yMin + topRectHeight + margin
			};
		}

		/// <summary>
		/// Splits this rect vertically by a given height from the bottom.
		/// </summary>
		public static void SplitVerticalBottom(this Rect r, float bottomRectHeight, out Rect top, out Rect bottom, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			SplitVertical(r, r.height - bottomRectHeight - margin, out top, out bottom, margin);
		}

		/// <summary>
		/// Splits this rect vertically by a given ratio.
		/// </summary>
		public static void SplitVerticalRelative(this Rect r, float topRectRatio, out Rect top, out Rect bottom, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			float topHeight = r.height * topRectRatio;
			r.SplitVertical(topHeight, out top, out bottom, margin);
		}

		/// <summary>
		/// Creates a new rect to the right of this rect.
		/// </summary>
		public static Rect AppendRight(this Rect r, float width, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			r.x += r.width + margin;
			r.width = width;
			return r;
		}

		/// <summary>
		/// Creates a new rect below this rect.
		/// </summary>
		public static Rect AppendDown(this Rect r, float height, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			r.y += r.height + margin;
			r.height = height;
			return r;
		}

		/// <summary>
		/// Creates an inset rect by the given inset values.
		/// </summary>
		public static Rect Inset(this Rect r, float left, float right, float top, float bottom)
		{
			return new Rect(r.x + left, r.y + top, r.width - left - right, r.height - top - bottom);
		}

		/// <summary>
		/// Creates an inset rect by the given inset value.
		/// </summary>
		public static Rect Inset(this Rect r, float inset)
		{
			return Inset(r, inset, inset, inset, inset);
		}

		/// <summary>
		/// Creates an outset rect by the given outset values.
		/// </summary>
		public static Rect Outset(this Rect r, float left, float right, float top, float bottom)
		{
			return Inset(r, -left, -right, -top, -bottom);
		}

		/// <summary>
		/// Creates an outset rect by the given outset value.
		/// </summary>
		public static Rect Outset(this Rect r, float outset)
		{
			return Inset(r, -outset);
		}

		/// <summary>
		/// Splits multiple rectangles from this rect, starting from the left.
		/// </summary>
		public static Rect[] SplitHorizontalMulti(this Rect r, int count, float width, out Rect leftover, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			if(count <= 0)
			{
				leftover = Rect.zero;
				return new Rect[] { r };
			}
			var rects = new Rect[count];
			for(int i = 0; i < count; i++)
			{
				float offset = i * (width + margin);
				rects[i] = new Rect(r.x + offset, r.y, width, r.height);
			}
			leftover = r;
			leftover.xMin += count * (width + margin) - margin;
			return rects;
		}

		/// <summary>
		/// Splits multiple rectangles from this rect, starting from the right.
		/// </summary>
		public static Rect[] SplitHorizontalMultiRight(this Rect r, int count, float width, out Rect leftover, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			if(count <= 0)
			{
				leftover = Rect.zero;
				return new Rect[] { r };
			}
			float w = count * (width + margin) - margin;
			leftover = r;
			leftover.width -= w;
			Rect r1 = r;
			r1.xMin = r1.xMax - w;
			return r1.SplitHorizontalMulti(count, width, out _, margin);
		}

		/// <summary>
		/// Splits multiple rectangles from this rect, starting from the top.
		/// </summary>
		public static Rect[] SplitVerticalMulti(this Rect r, int count, float height, out Rect leftover, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			if(count <= 0)
			{
				leftover = Rect.zero;
				return new Rect[] { r };
			}
			var rects = new Rect[count];
			for(int i = 0; i < count; i++)
			{
				float offset = i * (height + margin);
				rects[i] = new Rect(r.x, r.y + offset, r.width, height);
			}
			leftover = r;
			leftover.yMin += count * (height + margin) - margin;
			return rects;
		}

		/// <summary>
		/// Splits multiple rectangles from this rect, starting from the bottom.
		/// </summary>
		public static Rect[] SplitVerticalMultiBottom(this Rect r, int count, float height, out Rect leftover, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			if(count <= 0)
			{
				leftover = Rect.zero;
				return new Rect[] { r };
			}
			float h = count * (height + margin) - margin;
			leftover = r;
			leftover.height -= h;
			Rect r1 = r;
			r1.yMin = r1.yMax - h;
			return r1.SplitVerticalMulti(count, height, out _, margin);
		}

		/// <summary>
		/// Divides this rect horizontally into multiple equal rects.
		/// </summary>
		public static Rect[] DivideHorizontal(this Rect r, int count, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			float w = r.width / count - (margin * (count - 1) / count);
			return r.SplitHorizontalMulti(count, w, out _, margin);
		}

		/// <summary>
		/// Divides this rect vertically into multiple equal rects.
		/// </summary>
		public static Rect[] DivideVertical(this Rect r, int count, float margin = 0)
		{
			margin = Mathf.Max(margin, 0);
			float h = r.height / count - (margin * (count - 1) / count);
			return r.SplitVerticalMulti(count, h, out _, margin);
		}

		/// <summary>
		/// Limits this rect to the given bounds.
		/// </summary>
		public static void Limit(this ref Rect r, Rect bounds)
		{
			r.x = Mathf.Max(r.x, bounds.xMin);
			r.y = Mathf.Max(r.y, bounds.yMin);
			r.x = Mathf.Min(r.x + r.width, bounds.xMax) - r.width;
			r.y = Mathf.Min(r.y + r.height, bounds.yMax) - r.height;
		}

	}
}
