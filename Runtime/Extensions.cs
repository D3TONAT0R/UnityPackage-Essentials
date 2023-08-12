using System.Text;
using UnityEngine;

namespace D3T
{
	public static class Extensions
	{

		#region Transforms

		public static void LookAtVertical(this Transform t, Transform target, bool invert = false)
		{
			LookAtVertical(t, target.position, invert);
		}

		public static void LookAtVertical(this Transform t, Vector3 target, bool invert = false)
		{
			Vector3 direction = Vector3.Normalize(target - t.position);
			FaceVertical(t, direction, invert);
		}

		public static void FaceVertical(this Transform t, Vector3 direction, bool invert = false)
		{
			Vector3 euler = Quaternion.LookRotation(direction).eulerAngles;
			euler.x = 0;
			euler.z = 0;
			if (invert) euler.y += 180f;
			t.eulerAngles = euler;
		}

		/// <summary>
		/// Returns the transform's hierarchy path string in the format "Root/Parent 1/Parent 2/Transform" up to the scene root or the given parent transform.
		/// </summary>
		public static string GetHierarchyPathString(this Transform transform, Transform parent = null)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(transform.name);
			var t = transform;
			while (t.parent != null && t.parent != parent)
			{
				t = t.parent;
				sb.Insert(0, t.name + "/");
			}
			return sb.ToString();
		}

		#endregion
		#region Numerics

		public static bool IsBetween(this float f, float min, float max) => f >= min && f <= max;

		public static bool IsBetweenExcluding(this float f, float min, float max) => f > min && f < max;

		public static bool IsBetweenIncluding(this int i, int min, int max) => i >= min && i <= max;

		public static bool IsBetweenExcluding(this int i, int min, int max) => i > min && i < max;

		public static float Round(this float f, int decNumbers)
		{
			int exp = (int)Mathf.Pow(10, decNumbers);
			return decNumbers <= 0 ? Mathf.RoundToInt(f) : Mathf.Round(f * exp) / exp;
		}

		public static float RoundTo(this float f, float rounding) => Mathf.Round(f * rounding) / rounding;

		public static float Abs(this float f) => Mathf.Abs(f);

		public static int Abs(this int i) => Mathf.Abs(i);

		public static Vector2 Abs(this Vector2 v) => new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));

		public static Vector3 Abs(this Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

		public static int Sign(this float f) => f > 0 ? 1 : f < 0 ? -1 : 0;
		public static int Sign(this int i) => i > 0 ? 1 : i < 0 ? -1 : 0;

		public static float Min(this Vector2 v) => Mathf.Min(v.x, v.y);

		public static float Max(this Vector2 v) => Mathf.Max(v.x, v.y);

		public static float Min(this Vector3 v) => Mathf.Min(v.x, v.y, v.z);

		public static float Max(this Vector3 v) => Mathf.Max(v.x, v.y, v.z);

		public static float Min(this Vector4 v) => Mathf.Min(v.x, v.y, v.z, v.w);

		public static float Max(this Vector4 v) => Mathf.Max(v.x, v.y, v.z, v.w);

		public static float ClampAngle(this float f)
		{
			while(f > 180) f -= 360;
			while(f < -180) f += 360;
			return f;
		}

		public static Vector2 ClampAngle(this Vector2 v) => new Vector2(ClampAngle(v.x), ClampAngle(v.y));

		public static Vector3 ClampAngle(this Vector3 v) => new Vector3(ClampAngle(v.x), ClampAngle(v.y), ClampAngle(v.z));

		public static Vector2 Clamp(this Vector2 v, float min, float max)
		{
			for (int i = 0; i < 2; i++)
			{
				v[i] = Mathf.Clamp(v[i], min, max);
			}
			return v;
		}

		public static Vector3 Clamp(this Vector3 v, float min, float max)
		{
			for (int i = 0; i < 3; i++)
			{
				v[i] = Mathf.Clamp(v[i], min, max);
			}
			return v;
		}

		#endregion
		#region Vector conversions

		public static Vector3 ToUVector2(this float f) => new Vector2(f, f);
		public static Vector3 ToUVector3(this float f) => new Vector3(f, f, f);
		public static Vector3 ToUVector4(this float f) => new Vector4(f, f, f, f);

		public static Quaternion ToQuaternion(this Vector4 v) => new Quaternion(v.x, v.y, v.z, v.w);
		public static Vector4 ToVector4(this Quaternion q) => new Vector4(q.x, q.y, q.z, q.w);

		#endregion
		#region Vector swizzling

		public static Vector2 Swizzle(this Vector3 v, int xIndex, int yIndex)
		{
			return new Vector2(v[xIndex], v[yIndex]);
		}
		public static Vector3 Swizzle(this Vector3 v, int xIndex, int yIndex, int zIndex)
		{
			return new Vector3(v[xIndex], v[yIndex], v[zIndex]);
		}

		public static Vector2 XX(this Vector3 v) => new Vector2(v.x, v.x);
		public static Vector2 XY(this Vector3 v) => new Vector2(v.x, v.y);
		public static Vector2 XZ(this Vector3 v) => new Vector2(v.x, v.z);
		public static Vector2 YX(this Vector3 v) => new Vector2(v.y, v.x);
		public static Vector2 YY(this Vector3 v) => new Vector2(v.y, v.y);
		public static Vector2 YZ(this Vector3 v) => new Vector2(v.y, v.z);
		public static Vector2 ZX(this Vector3 v) => new Vector2(v.z, v.x);
		public static Vector2 ZY(this Vector3 v) => new Vector2(v.z, v.y);
		public static Vector2 ZZ(this Vector3 v) => new Vector2(v.z, v.z);

		public static Vector3 XYV(this Vector2 v, float z) => new Vector3(v.x, v.y, z);
		public static Vector3 XVY(this Vector2 v, float y) => new Vector3(v.x, y, v.y);
		public static Vector3 VXY(this Vector2 v, float x) => new Vector3(x, v.x, v.y);

		public static Vector3 XXX(this Vector3 v) => new Vector3(v.x, v.x, v.x);
		public static Vector3 XXY(this Vector3 v) => new Vector3(v.x, v.x, v.y);
		public static Vector3 XXZ(this Vector3 v) => new Vector3(v.x, v.x, v.z);
		public static Vector3 XYX(this Vector3 v) => new Vector3(v.x, v.y, v.x);
		public static Vector3 XYY(this Vector3 v) => new Vector3(v.x, v.y, v.y);
		public static Vector3 XYZ(this Vector3 v) => v;
		public static Vector3 XZX(this Vector3 v) => new Vector3(v.x, v.z, v.x);
		public static Vector3 XZY(this Vector3 v) => new Vector3(v.x, v.z, v.y);
		public static Vector3 XZZ(this Vector3 v) => new Vector3(v.x, v.z, v.z);
		public static Vector3 YXX(this Vector3 v) => new Vector3(v.y, v.x, v.x);
		public static Vector3 YXY(this Vector3 v) => new Vector3(v.y, v.x, v.y);
		public static Vector3 YXZ(this Vector3 v) => new Vector3(v.y, v.x, v.z);
		public static Vector3 YYX(this Vector3 v) => new Vector3(v.y, v.y, v.x);
		public static Vector3 YYY(this Vector3 v) => new Vector3(v.y, v.y, v.y);
		public static Vector3 YYZ(this Vector3 v) => new Vector3(v.y, v.y, v.z);
		public static Vector3 YZX(this Vector3 v) => new Vector3(v.y, v.z, v.x);
		public static Vector3 YZY(this Vector3 v) => new Vector3(v.y, v.z, v.y);
		public static Vector3 YZZ(this Vector3 v) => new Vector3(v.y, v.z, v.z);
		public static Vector3 ZXX(this Vector3 v) => new Vector3(v.z, v.x, v.x);
		public static Vector3 ZXY(this Vector3 v) => new Vector3(v.z, v.x, v.y);
		public static Vector3 ZXZ(this Vector3 v) => new Vector3(v.z, v.x, v.z);
		public static Vector3 ZYX(this Vector3 v) => new Vector3(v.z, v.y, v.x);
		public static Vector3 ZYY(this Vector3 v) => new Vector3(v.z, v.y, v.y);
		public static Vector3 ZYZ(this Vector3 v) => new Vector3(v.z, v.y, v.z);
		public static Vector3 ZZX(this Vector3 v) => new Vector3(v.z, v.z, v.x);
		public static Vector3 ZZY(this Vector3 v) => new Vector3(v.z, v.z, v.y);
		public static Vector3 ZZZ(this Vector3 v) => new Vector3(v.z, v.z, v.z);

		#endregion

		public static float Sum(this Vector2 v)
		{
			return v.x + v.y;
		}

		public static float Sum(this Vector3 v)
		{
			return v.x + v.y + v.z;
		}

		public static float Sum(this Vector4 v)
		{
			return v.x + v.y + v.z + v.w;
		}

		public static float Product(this Vector2 v)
		{
			return v.x * v.y;
		}

		public static float Product(this Vector3 v)
		{
			return v.x * v.y * v.z;
		}

		public static float Product(this Vector4 v)
		{
			return v.x * v.y * v.z * v.w;
		}

		public static Vector2 WithX(this Vector2 v, float x)
		{
			v.x = x;
			return v;
		}

		public static Vector2 WithY(this Vector2 v, float y)
		{
			v.y = y;
			return v;
		}

		public static Vector3 WithX(this Vector3 v, float x)
		{
			v.x = x;
			return v;
		}

		public static Vector3 WithY(this Vector3 v, float y)
		{
			v.y = y;
			return v;
		}

		public static Vector3 WithZ(this Vector3 v, float z)
		{
			v.z = z;
			return v;
		}

		public static Vector4 WithX(this Vector4 v, float x)
		{
			v.x = x;
			return v;
		}

		public static Vector4 WithY(this Vector4 v, float y)
		{
			v.y = y;
			return v;
		}

		public static Vector4 WithZ(this Vector4 v, float z)
		{
			v.z = z;
			return v;
		}

		public static Vector4 WithW(this Vector4 v, float w)
		{
			v.w = w;
			return v;
		}

		public static bool IsBetweenExcluding(this Vector2 v, Vector2 min, Vector2 max)
		{
			return v.x.IsBetweenExcluding(min.x, max.x) && v.y.IsBetweenExcluding(min.y, max.y);
		}

		public static bool IsBetweenExcluding(this Vector3 v, Vector3 min, Vector3 max)
		{
			return v.x.IsBetweenExcluding(min.x, max.x) && v.y.IsBetweenExcluding(min.y, max.y) && v.z.IsBetweenExcluding(min.z, max.z);
		}

		public static bool IsBetweenExcluding(this Vector4 v, Vector4 min, Vector4 max)
		{
			return v.x.IsBetweenExcluding(min.x, max.x) && v.y.IsBetweenExcluding(min.y, max.y) && v.z.IsBetweenExcluding(min.z, max.z) && v.w.IsBetweenExcluding(min.w, max.w);
		}

		public static bool IsBetween(this Vector2 v, Vector2 min, Vector2 max)
		{
			return v.x.IsBetween(min.x, max.x) && v.y.IsBetween(min.y, max.y);
		}

		public static bool IsBetween(this Vector3 v, Vector3 min, Vector3 max)
		{
			return v.x.IsBetween(min.x, max.x) && v.y.IsBetween(min.y, max.y) && v.z.IsBetween(min.z, max.z);
		}

		public static bool IsBetween(this Vector4 v, Vector4 min, Vector4 max)
		{
			return v.x.IsBetween(min.x, max.x) && v.y.IsBetween(min.y, max.y) && v.z.IsBetween(min.z, max.z) && v.w.IsBetween(min.w, max.w);
		}

		public static float GetSlopeAngle(this Vector3 n)
		{
			float h = Mathf.Sqrt(n.x * n.x + n.z * n.z);
			float v = n.y;
			return 90f - (Mathf.Atan(v / h) * Mathf.Rad2Deg);
		}

		public static Vector2 ScaleAround(this Vector2 point, Vector2 scale, Vector2 pivot)
		{
			Vector2 v = point - pivot;
			scale.x -= 1;
			scale.y -= 1;
			Vector2 vS = new Vector2(v.x * scale.x, v.y * scale.y);
			return point + vS;
		}

		public static string ToString(this Vector2 v, int decimals) => v.ToString("F" + decimals);
		public static string ToString(this Vector3 v, int decimals) => v.ToString("F" + decimals);
		public static string ToString(this Vector4 v, int decimals) => v.ToString("F" + decimals);

		public static bool ContainsLayer(this LayerMask m, int layer)
		{
			return m == (m | (1 << layer));
		}

		public static Vector3 ExtractPosition(this Matrix4x4 matrix)
		{
			Vector3 position;
			position.x = matrix.m03;
			position.y = matrix.m13;
			position.z = matrix.m23;
			return position;
		}

		public static Quaternion ExtractRotation(this Matrix4x4 matrix)
		{
			return matrix.rotation;
		}

		public static Vector3 ExtractScale(this Matrix4x4 matrix)
		{
			Vector3 scale;
			scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
			scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
			scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
			return scale;
		}

		public static void ExtractTransform(this Matrix4x4 matrix, out Vector3 position, out Quaternion rotation, out Vector3 scale)
		{
			position = ExtractPosition(matrix);
			rotation = ExtractRotation(matrix);
			scale = ExtractScale(matrix);
		}

		public static Matrix4x4 GetTRSMatrix(this Transform t)
		{
			return Matrix4x4.TRS(t.position, t.rotation, t.localScale);
		}

		public static void ApplyTRSMatrix(this Transform t, Matrix4x4 trsMatrix)
		{
			t.position = trsMatrix.ExtractPosition();
			t.rotation = trsMatrix.ExtractRotation();
			t.localScale = trsMatrix.ExtractScale();
		}

		public static void ApplyTRSMatrixLocally(this Transform t, Matrix4x4 trsMatrix)
		{
			t.localPosition = trsMatrix.ExtractPosition();
			t.localRotation = trsMatrix.ExtractRotation();
			t.localScale = trsMatrix.ExtractScale();
		}

		public static Rect AddPosition(this Rect r, Vector2 v)
		{
			Rect r2 = new Rect(r);
			r2.position += v;
			return r2;
		}

		public static Rect Scale(this Rect r, float value)
		{
			Rect r2 = new Rect(r);
			r2.position *= value;
			r2.size *= value;
			return r2;
		}

		public static Rect Snap(this Rect r)
		{
			Rect r2 = new Rect(r);
			r2.x = Mathf.Round(r2.x);
			r2.y = Mathf.Round(r2.y);
			r2.width = Mathf.Round(r2.width);
			r2.height = Mathf.Round(r2.height);
			return r2;
		}

		public static void SplitHorizontal(this Rect r, float leftRectWidth, out Rect left, out Rect right, float margin = 0)
		{
			left = new Rect(r)
			{
				width = leftRectWidth - margin * 0.5f
			};
			right = new Rect(r)
			{
				xMin = r.xMin + leftRectWidth + margin * 0.5f
			};
		}

		public static void SplitHorizontalRight(this Rect r, float rightRectWidth, out Rect left, out Rect right, float margin = 0)
		{
			SplitHorizontal(r, r.width - rightRectWidth, out left, out right, margin);
		}

		public static void SplitHorizontalRelative(this Rect r, float leftRectRatio, out Rect left, out Rect right, float margin = 0)
		{
			float leftWidth = r.width * leftRectRatio;
			r.SplitHorizontal(leftWidth, out left, out right, margin);
		}

		public static void SplitVertical(this Rect r, float topRectHeight, out Rect top, out Rect bottom, float margin = 0)
		{
			top = new Rect(r)
			{
				height = topRectHeight - margin * 0.5f
			};
			bottom = new Rect(r)
			{
				yMin = r.yMin + topRectHeight + margin * 0.5f
			};
		}

		public static void SplitVerticalBottom(this Rect r, float bottomRectHeight, out Rect top, out Rect bottom, float margin = 0)
		{
			SplitVertical(r, r.height - bottomRectHeight, out top, out bottom, margin);
		}

		public static void SplitVerticalRelative(this Rect r, float topRectRatio, out Rect top, out Rect bottom, float margin = 0)
		{
			float topHeight = r.height * topRectRatio;
			r.SplitVertical(topHeight, out top, out bottom, margin);
		}

		public static Rect AppendRight(this Rect r, float width, float margin = 0)
		{
			r.x += r.width + margin;
			r.width = width;
			return r;
		}

		public static Rect AppendDown(this Rect r, float height, float margin = 0)
		{
			r.y += r.height + margin;
			r.height = height;
			return r;
		}

		public static Rect Inset(this Rect r, float left, float right, float top, float bottom)
		{
			return new Rect(r.x + left, r.y + top, r.width - left - right, r.height - top - bottom);
		}

		public static Rect Inset(this Rect r, float inset)
		{
			return Inset(r, inset, inset, inset, inset);
		}

		public static Rect Outset(this Rect r, float left, float right, float top, float bottom)
		{
			return Inset(r, -left, -right, -top, -bottom);
		}

		public static Rect Outset(this Rect r, float outset)
		{
			return Inset(r, -outset);
		}

		public static Rect[] SplitHorizontalMulti(this Rect r, int count, float rectWidths, out Rect leftover, float margin = 0)
		{
			var rects = new Rect[count];
			for (int i = 0; i < count; i++)
			{
				float offset = i * (rectWidths + margin);
				rects[i] = new Rect(r.x + offset, r.y, rectWidths, r.height);
			}
			leftover = r;
			leftover.xMin += count * (rectWidths + margin) - margin;
			return rects;
		}

		public static Rect[] SplitHorizontalMultiRight(this Rect r, int count, float rectWidths, out Rect leftover, float margin = 0)
		{
			float w = count * (rectWidths + margin) - margin;
			leftover = r;
			leftover.width -= w;
			Rect r1 = r;
			r1.xMin = r1.xMax - w;
			return r1.SplitHorizontalMulti(count, rectWidths, out _, margin);
		}

		public static Rect[] DivideHorizontal(this Rect r, int count, float margin = 0)
		{
			float w = r.width / count;
			return r.SplitHorizontalMulti(count, w, out _, margin);
		}

		public static void Limit(this ref Rect r, Rect limits)
		{
			r.x = Mathf.Max(r.x, limits.xMin);
			r.y = Mathf.Max(r.y, limits.yMin);
			r.x = Mathf.Min(r.x + r.width, limits.xMax) - r.width;
			r.y = Mathf.Min(r.y + r.height, limits.yMax) - r.height;
		}

		public static void DrawOnRepaint(this GUIStyle s, Rect r)
		{
			if (Event.current.type == EventType.Repaint) s.Draw(r, false, false, false, false);
		}

		public static void DrawOnRepaint(this GUIStyle s, Rect r, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
		{
			if (Event.current.type == EventType.Repaint) s.Draw(r, isHover, isActive, on, hasKeyboardFocus);
		}

		public static string ConvertToTimeString(this int t, bool hours)
		{
			t = Mathf.Abs(t);
			string sec = (t % 60).ToString();
			string min = (hours ? t / 60 % 60 : t / 60).ToString();
			string hrs = (t / 3600).ToString();
			if (sec.Length < 2) sec = "0" + sec;
			if (min.Length < 2) min = "0" + min;
			if (hrs.Length < 2) hrs = "0" + hrs;
			return hours ?
				hrs + ":" + min + ":" + sec
				: min + ":" + sec;
		}

		public static Color SetAlpha(this Color c, float a)
		{
			return new Color(c.r, c.g, c.b, a);
		}

		public static Color MultiplyAlpha(this Color c, float multiplier)
		{
			return new Color(c.r, c.g, c.b, c.a * multiplier);
		}

		public static Color ScaleSaturation(this Color c, float saturation)
		{
			return Color.LerpUnclamped(Grayscale(c), c, saturation);
		}

		public static Color Grayscale(this Color c)
		{
			float gray = c.grayscale;
			return new Color(gray, gray, gray, c.a);
		}

		public static void FillWithColor(this Texture2D tex, Color c)
		{
			Color32[] cols = new Color32[tex.width * tex.height];
			for (int i = 0; i < cols.Length; i++) cols[i] = c;
			tex.SetPixels32(cols);
			tex.Apply();
		}

		public static Texture2D GetReadableCopy(this Texture2D tex)
		{
			if (tex.isReadable)
			{
				Debug.LogWarning("Texture is already readable.");
				return tex;
			}
			Texture2D copy = new Texture2D(tex.width, tex.height, tex.format, tex.mipmapCount, false);
			copy.wrapMode = tex.wrapMode;
			copy.filterMode = tex.filterMode;
			UnityEngine.Graphics.CopyTexture(tex, copy);
			copy.Apply();
			return copy;
		}

		public static MessagedException AddMessage(this System.Exception e, string message)
		{
			return new MessagedException(message, e);
		}

		public static void LogException(this System.Exception e, string message = null, Object context = null)
		{
			Debug.LogException(e.AddMessage(message), context);
		}

		public static void InvokeValidation(this MonoBehaviour m, System.Action onValidateAction)
		{
			bool wasPlaying = Application.isPlaying;
#if UNITY_EDITOR
			UnityEditor.EditorApplication.delayCall += _OnValidate;

			void _OnValidate()
			{
				UnityEditor.EditorApplication.delayCall -= _OnValidate;
				if(Application.isPlaying == wasPlaying)
				{
					onValidateAction();
				}
			}
#endif
		}
	}
}
