using UnityEngine;

namespace UnityEssentials
{
	public static class MatrixExtensions
	{
		/// <summary>
		/// Extracts translation information from this matrix.
		/// </summary>
		public static Vector3 ExtractPosition(this Matrix4x4 matrix)
		{
			Vector3 position;
			position.x = matrix.m03;
			position.y = matrix.m13;
			position.z = matrix.m23;
			return position;
		}

		/// <summary>
		/// Extracts rotation information from this matrix.
		/// </summary>
		public static Quaternion ExtractRotation(this Matrix4x4 matrix)
		{
			return matrix.rotation;
		}

		/// <summary>
		/// Extracts scale information from this matrix.
		/// </summary>
		public static Vector3 ExtractScale(this Matrix4x4 matrix)
		{
			Vector3 scale;
			scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
			scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
			scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
			return scale;
		}

		/// <summary>
		/// Extracts transformation information from this matrix.
		/// </summary>
		public static void ExtractTransform(this Matrix4x4 matrix, out Vector3 position, out Quaternion rotation, out Vector3 scale)
		{
			position = ExtractPosition(matrix);
			rotation = ExtractRotation(matrix);
			scale = ExtractScale(matrix);
		}

		/// <summary>
		/// Creates a TRS matrix from this transform.
		/// </summary>
		public static Matrix4x4 GetTRSMatrix(this Transform t, bool includeScale = true)
		{
			return Matrix4x4.TRS(t.position, t.rotation, includeScale ? t.localScale : Vector3.one);
		}

		/// <summary>
		/// Applies the given TRS matrix to this transform.
		/// </summary>
		public static void ApplyTRSMatrix(this Transform t, Matrix4x4 trsMatrix)
		{
			t.position = trsMatrix.ExtractPosition();
			t.rotation = trsMatrix.ExtractRotation();
			t.localScale = trsMatrix.ExtractScale();
		}

		/// <summary>
		/// Applies the given TRS matrix to this transform in local space.
		/// </summary>
		public static void ApplyTRSMatrixLocally(this Transform t, Matrix4x4 trsMatrix)
		{
			t.localPosition = trsMatrix.ExtractPosition();
			t.localRotation = trsMatrix.ExtractRotation();
			t.localScale = trsMatrix.ExtractScale();
		}
	}
}
