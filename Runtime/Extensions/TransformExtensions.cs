using System.Text;
using UnityEngine;

namespace D3T
{
	public static class TransformExtensions
	{
		private static StringBuilder stringBuilder = new StringBuilder();

		/// <summary>
		/// Makes this transform look at another transform while remaining vertical.
		/// </summary>
		public static void LookAtVertical(this Transform t, Transform target, bool invert = false)
		{
			LookAtVertical(t, target.position, invert);
		}

		/// <summary>
		/// Makes this transform look at a given position while remaining vertical.
		/// </summary>
		public static void LookAtVertical(this Transform t, Vector3 target, bool invert = false)
		{
			Vector3 direction = Vector3.Normalize(target - t.position);
			FaceVertical(t, direction, invert);
		}

		/// <summary>
		/// Makes this transform face a given direction while remaining vertical.
		/// </summary>
		public static void FaceVertical(this Transform t, Vector3 direction, bool invert = false)
		{
			Vector3 euler = Quaternion.LookRotation(direction).eulerAngles;
			euler.x = 0;
			euler.z = 0;
			if(invert) euler.y += 180f;
			t.eulerAngles = euler;
		}

		/// <summary>
		/// Returns the transform's hierarchy path string in the format "Root/Parent/Transform" up to the scene root or the given parent transform.
		/// </summary>
		public static string GetHierarchyPathString(this Transform transform, Transform parent = null)
		{
			stringBuilder.Clear();
			stringBuilder.Append(transform.name);
			var t = transform;
			while(t.parent != null && t.parent != parent)
			{
				t = t.parent;
				stringBuilder.Insert(0, t.name + "/");
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Returns the hierarchical depth of this transform.
		/// </summary>
		public static int GetHierarchyDepth(this Transform t)
		{
			int d = 0;
			while(t.parent != null)
			{
				d++;
				t = t.parent;
			}
			return d;
		}

		/// <summary>
		/// Returns the total number of children parented to this transform.
		/// </summary>
		public static int GetTotalChildCount(this Transform t)
		{
			int recursiveChildCount = t.childCount;
			for(int i = 0; i < t.childCount; i++)
			{
				recursiveChildCount += GetTotalChildCount(t.GetChild(i));
			}
			return recursiveChildCount;
		}
	}
}
