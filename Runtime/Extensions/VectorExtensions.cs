using UnityEngine;

namespace D3T
{
	public static class VectorExtensions
	{
		#region Vector conversions

		/// <summary>
		/// Converts this vector into a Quaternion.
		/// </summary>
		public static Quaternion ToQuaternion(this Vector4 v) => new Quaternion(v.x, v.y, v.z, v.w);

		/// <summary>
		/// Converts this Quaterion into a Vector4
		/// </summary>
		public static Vector4 ToVector4(this Quaternion q) => new Vector4(q.x, q.y, q.z, q.w);

		public static Vector3 ToVector3(this Vector2 v, float z) => new Vector3(v.x, v.y, z);

		public static Vector4 ToVector4(this Vector2 v, float z, float w) => new Vector4(v.x, v.y, z, w);

		public static Vector4 ToVector4(this Vector2 v, Vector2 zw) => new Vector4(v.x, v.y, zw.x, zw.y);

		public static Vector4 ToVector4(this Vector3 v, float w) => new Vector4(v.x, v.y, v.z, w);

		/// <summary>
		/// Returns a nicely formatted string for this vector, with a specific number of decimal places.
		/// </summary>
		public static string ToString(this Vector2 v, int decimals) => v.ToString("F" + decimals);

		/// <summary>
		/// Returns a nicely formatted string for this vector, with a specific number of decimal places.
		/// </summary>
		public static string ToString(this Vector3 v, int decimals) => v.ToString("F" + decimals);

		/// <summary>
		/// Returns a nicely formatted string for this vector, with a specific number of decimal places.
		/// </summary>
		public static string ToString(this Vector4 v, int decimals) => v.ToString("F" + decimals);

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

		#region Vector modifications

		/// <summary>
		/// Modifies the x value of this vector.
		/// </summary>
		public static Vector2 WithX(this Vector2 v, float x)
		{
			v.x = x;
			return v;
		}

		/// <summary>
		/// Modifies the y value of this vector.
		/// </summary>
		public static Vector2 WithY(this Vector2 v, float y)
		{
			v.y = y;
			return v;
		}

		/// <summary>
		/// Modifies the x value of this vector.
		/// </summary>
		public static Vector3 WithX(this Vector3 v, float x)
		{
			v.x = x;
			return v;
		}

		/// <summary>
		/// Modifies the y value of this vector.
		/// </summary>
		public static Vector3 WithY(this Vector3 v, float y)
		{
			v.y = y;
			return v;
		}

		/// <summary>
		/// Modifies the z value of this vector.
		/// </summary>
		public static Vector3 WithZ(this Vector3 v, float z)
		{
			v.z = z;
			return v;
		}

		/// <summary>
		/// Modifies the x value of this vector.
		/// </summary>
		public static Vector4 WithX(this Vector4 v, float x)
		{
			v.x = x;
			return v;
		}

		/// <summary>
		/// Modifies the y value of this vector.
		/// </summary>
		public static Vector4 WithY(this Vector4 v, float y)
		{
			v.y = y;
			return v;
		}

		/// <summary>
		/// Modifies the z value of this vector.
		/// </summary>
		public static Vector4 WithZ(this Vector4 v, float z)
		{
			v.z = z;
			return v;
		}

		/// <summary>
		/// Modifies the w value of this vector.
		/// </summary>
		public static Vector4 WithW(this Vector4 v, float w)
		{
			v.w = w;
			return v;
		}

		/// <summary>
		/// Returns the value representing the given axis.
		/// </summary>
		public static float GetAxis(this Vector2 v, Axis axis)
		{
			return v[(int)axis];
		}

		/// <summary>
		/// Returns the value representing the given axis.
		/// </summary>
		public static float GetAxis(this Vector3 v, Axis axis)
		{
			return v[(int)axis];
		}

		/// <summary>
		/// Sets the value representing the given axis.
		/// </summary>
		public static Vector2 SetAxis(this Vector2 v, Axis axis, float value)
		{
			v[(int)axis] = value;
			return v;
		}

		/// <summary>
		/// Sets the value representing the given axis.
		/// </summary>
		public static Vector3 SetAxis(this Vector3 v, Axis axis, float value)
		{
			v[(int)axis] = value;
			return v;
		}

		/// <summary>
		/// Adds a given value to the given axis.
		/// </summary>
		public static Vector2 AddAxis(this Vector2 v, Axis axis, float value)
		{
			v[(int)axis] += value;
			return v;
		}

		/// <summary>
		/// Adds a given value to the given axis.
		/// </summary>
		public static Vector3 AddAxis(this Vector3 v, Axis axis, float value)
		{
			v[(int)axis] += value;
			return v;
		}

		#endregion

		#region Vector math

		/// <summary>
		/// Returns the sum of all individual vector components.
		/// </summary>
		public static float Sum(this Vector2 v)
		{
			return v.x + v.y;
		}

		/// <summary>
		/// Returns the sum of all individual vector components.
		/// </summary>
		public static float Sum(this Vector3 v)
		{
			return v.x + v.y + v.z;
		}

		/// <summary>
		/// Returns the sum of all individual vector components.
		/// </summary>
		public static float Sum(this Vector4 v)
		{
			return v.x + v.y + v.z + v.w;
		}

		/// <summary>
		/// Returns the multiplication of all individual vector components.
		/// </summary>
		public static float Product(this Vector2 v)
		{
			return v.x * v.y;
		}

		/// <summary>
		/// Returns the multiplication of all individual vector components.
		/// </summary>
		public static float Product(this Vector3 v)
		{
			return v.x * v.y * v.z;
		}

		/// <summary>
		/// Returns the multiplication of all individual vector components.
		/// </summary>
		public static float Product(this Vector4 v)
		{
			return v.x * v.y * v.z * v.w;
		}

		/// <summary>
		/// Returns the smallest component of this vector.
		/// </summary>
		public static float Min(this Vector2 v) => Mathf.Min(v.x, v.y);

		/// <summary>
		/// Returns the largest component of this vector.
		/// </summary>
		public static float Max(this Vector2 v) => Mathf.Max(v.x, v.y);

		/// <summary>
		/// Returns the smallest component of this vector.
		/// </summary>
		public static float Min(this Vector3 v) => Mathf.Min(v.x, Mathf.Min(v.y, v.z));

		/// <summary>
		/// Returns the largest component of this vector.
		/// </summary>
		public static float Max(this Vector3 v) => Mathf.Max(v.x, Mathf.Max(v.y, v.z));

		/// <summary>
		/// Returns the smallest component of this vector.
		/// </summary>
		public static float Min(this Vector4 v) => Mathf.Min(v.x, Mathf.Min(v.y, Mathf.Min(v.z, v.w)));

		/// <summary>
		/// Returns the largest component of this vector.
		/// </summary>
		public static float Max(this Vector4 v) => Mathf.Max(v.x, Mathf.Max(v.y, Mathf.Max(v.z, v.w)));

		/// <summary>
		/// Returns the average value of the individual components.
		/// </summary>
		public static float Average(this Vector2 v) => (v.x + v.y) / 2f;

		/// <summary>
		/// Returns the average value of the individual components.
		/// </summary>
		public static float Average(this Vector3 v) => (v.x + v.y + v.z) / 3f;

		/// <summary>
		/// Returns the average value of the individual components.
		/// </summary>
		public static float Average(this Vector4 v) => (v.x + v.y + v.z + v.w) / 4f;

		/// <summary>
		/// Converts all components into their absolute values.
		/// </summary>
		public static Vector2 Abs(this Vector2 v) => new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));

		/// <summary>
		/// Converts all components into their absolute values.
		/// </summary>
		public static Vector3 Abs(this Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

		/// <summary>
		/// Converts all components into their absolute values.
		/// </summary>
		public static Vector4 Abs(this Vector4 v) => new Vector4(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z), Mathf.Abs(v.w));

		/// <summary>
		/// Clamps all component's angles between a -180 to 180 degrees range.
		/// </summary>
		public static Vector2 ClampAngle(this Vector2 v) => new Vector2(v.x.ClampAngle(), v.y.ClampAngle());

		/// <summary>
		/// Clamps all component's angles between a -180 to 180 degrees range.
		/// </summary>
		public static Vector3 ClampAngle(this Vector3 v) => new Vector3(v.x.ClampAngle(), v.y.ClampAngle(), v.z.ClampAngle());

		/// <summary>
		/// Clamps all components of this vector between the given minimum and maximum values.
		/// </summary>
		public static Vector2 Clamp(this Vector2 v, float min, float max)
		{
			v.x = Mathf.Clamp(v.x, min, max);
			v.y = Mathf.Clamp(v.y, min, max);
			return v;
		}

		/// <summary>
		/// Clamps all components of this vector between the given minimum and maximum values.
		/// </summary>
		public static Vector3 Clamp(this Vector3 v, float min, float max)
		{
			v.x = Mathf.Clamp(v.x, min, max);
			v.y = Mathf.Clamp(v.y, min, max);
			v.z = Mathf.Clamp(v.z, min, max);
			return v;
		}

		/// <summary>
		/// Clamps all components of this vector between the given minimum and maximum values.
		/// </summary>
		public static Vector4 Clamp(this Vector4 v, float min, float max)
		{
			v.x = Mathf.Clamp(v.x, min, max);
			v.y = Mathf.Clamp(v.y, min, max);
			v.z = Mathf.Clamp(v.z, min, max);
			v.w = Mathf.Clamp(v.w, min, max);
			return v;
		}

		/// <summary>
		/// Returns the angle of this vector in degrees (between 0 and 360, where 0 is up).
		/// </summary>
		public static float GetAngle(this Vector2 v)
		{
			v = v.normalized;
			float a = Mathf.Asin(v.x) * Mathf.Rad2Deg;
			if(v.y < 0) a = 180f - a;
			if(a < 0) a += 360f;
			return a;
		}

		/// <summary>
		/// Returns the slope angle of this normal vector (relative to the Y axis) in degrees.
		/// </summary>
		public static float GetSlopeAngle(this Vector3 v)
		{
			float h = Mathf.Sqrt(v.x * v.x + v.z * v.z);
			float y = v.y;
			return 90f - (Mathf.Atan(y / h) * Mathf.Rad2Deg);
		}

		#endregion

		#region Vector checks

		/// <summary>
		/// Returns true if this vector is between the given min and max bounds, excluding edge hits.
		/// </summary>
		public static bool IsBetweenExcluding(this Vector2 v, Vector2 min, Vector2 max)
		{
			return v.x.IsBetweenExcluding(min.x, max.x) 
				&& v.y.IsBetweenExcluding(min.y, max.y);
		}

		/// <summary>
		/// Returns true if this vector is between the given min and max bounds, excluding edge hits.
		/// </summary>
		public static bool IsBetweenExcluding(this Vector3 v, Vector3 min, Vector3 max)
		{
			return v.x.IsBetweenExcluding(min.x, max.x) 
				&& v.y.IsBetweenExcluding(min.y, max.y) 
				&& v.z.IsBetweenExcluding(min.z, max.z);
		}

		/// <summary>
		/// Returns true if this vector is between the given min and max bounds, excluding edge hits.
		/// </summary>
		public static bool IsBetweenExcluding(this Vector4 v, Vector4 min, Vector4 max)
		{
			return v.x.IsBetweenExcluding(min.x, max.x)
				&& v.y.IsBetweenExcluding(min.y, max.y) 
				&& v.z.IsBetweenExcluding(min.z, max.z) 
				&& v.w.IsBetweenExcluding(min.w, max.w);
		}

		/// <summary>
		/// Returns true if this vector is between the given min and max bounds.
		/// </summary>
		public static bool IsBetween(this Vector2 v, Vector2 min, Vector2 max)
		{
			return v.x.IsBetween(min.x, max.x)
				&& v.y.IsBetween(min.y, max.y);
		}

		/// <summary>
		/// Returns true if this vector is between the given min and max bounds.
		/// </summary>
		public static bool IsBetween(this Vector3 v, Vector3 min, Vector3 max)
		{
			return v.x.IsBetween(min.x, max.x) 
				&& v.y.IsBetween(min.y, max.y) 
				&& v.z.IsBetween(min.z, max.z);
		}

		/// <summary>
		/// Returns true if this vector is between the given min and max bounds.
		/// </summary>
		public static bool IsBetween(this Vector4 v, Vector4 min, Vector4 max)
		{
			return v.x.IsBetween(min.x, max.x)
				&& v.y.IsBetween(min.y, max.y)
				&& v.z.IsBetween(min.z, max.z) 
				&& v.w.IsBetween(min.w, max.w);
		}

		#endregion
	}
}
