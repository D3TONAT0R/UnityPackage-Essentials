using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEssentials.PlayerLoop;

namespace UnityEssentials
{
	/// <summary>
	/// Utility functions for debugging.
	/// </summary>
	public class DebugUtility
	{
		private abstract class GizmoInstance
		{
			public readonly float duration;
			public readonly Color color;
			private readonly float spawnTime;
			private float lastDrawTime;

			public float Life => SingleFrame ? 1f : 1f - (Time.unscaledTime - spawnTime) / duration;
			private bool SingleFrame => duration <= 0;

			public bool Expired
			{
				get
				{
					if(SingleFrame) return Time.unscaledTime > spawnTime && (lastDrawTime != 0 && lastDrawTime < Time.unscaledTime);
					else return Time.unscaledTime - spawnTime > duration;
				}
			}

			protected GizmoInstance(Color color, float duration)
			{
				spawnTime = Time.unscaledTime;
				this.duration = duration;
				this.color = color;
			}

			public void Draw()
			{
				lastDrawTime = Time.unscaledTime;
				DrawGizmos();
			}

			protected abstract void DrawGizmos();
		}

		private class RaycastGizmoInstance : GizmoInstance
		{
			protected readonly Ray ray;
			protected readonly float hitSize;
			protected readonly float distance;
			protected IEnumerable<RaycastHit> hits;

			public RaycastGizmoInstance(Ray ray, float hitSize, IEnumerable<RaycastHit> hits, float distance, Color color, float duration) : base(color, duration)
			{
				this.ray = ray;
				this.hitSize = Mathf.Max(0, hitSize);
				this.hits = hits;
				this.distance = distance;
			}

			protected override void DrawGizmos()
			{
				Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * distance);
				if(hits != null)
				{
					foreach(var hit in hits)
					{
						if(!hit.collider) continue;
						Gizmos.matrix = Matrix4x4.TRS(hit.point, Quaternion.LookRotation(hit.normal), Vector3.one);
						Gizmos.DrawWireCube(Vector3.zero, Vector3.one * hitSize);
						Gizmos.DrawLine(Vector3.zero, Vector3.forward * hitSize * 2f);
					}
				}
			}
		}

		private class SphereCastGizmoInstance : RaycastGizmoInstance
		{
			public SphereCastGizmoInstance(Ray ray, float hitSize, IEnumerable<RaycastHit> hits, float distance, Color color, float duration)
				: base(ray, hitSize, hits, distance, color, duration)
			{

			}

			protected override void DrawGizmos()
			{
				var matrix = Matrix4x4.TRS(ray.origin, Quaternion.LookRotation(ray.direction), Vector3.one);
				var inverseMatrix = matrix.inverse;
				Gizmos.matrix = matrix;
				ExtraGizmos.DrawWireCircle(Vector3.zero, Vector3.forward, hitSize, 16);
				ExtraGizmos.DrawWireCircle(Vector3.forward * distance, Vector3.forward, hitSize, 16);
				ExtraGizmos.DrawLineFrom(Vector3.up * hitSize, Vector3.forward, distance);
				ExtraGizmos.DrawLineFrom(Vector3.right * hitSize, Vector3.forward, distance);
				ExtraGizmos.DrawLineFrom(Vector3.down * hitSize, Vector3.forward, distance);
				ExtraGizmos.DrawLineFrom(Vector3.left * hitSize, Vector3.forward, distance);
				foreach(var hit in hits)
				{
					if(!hit.collider) continue;
					var localPos = inverseMatrix.MultiplyPoint(hit.point);
					var localNormal = inverseMatrix.MultiplyVector(hit.normal);
					Gizmos.DrawWireSphere(localPos + localNormal * hitSize, hitSize);
					ExtraGizmos.DrawPoint(localPos, hitSize / 2, false);
				}
			}
		}

		private class PointGizmoInstance : GizmoInstance
		{
			private readonly Vector3 position;
			private readonly float size;
			private readonly bool constantSize;
			private readonly bool centerSphere;

			public PointGizmoInstance(Vector3 position, float size, bool centerSphere, bool constantSize, Color color, float duration) : base(color, duration)
			{
				this.position = position;
				this.size = size;
				this.constantSize = constantSize;
				this.centerSphere = centerSphere;
			}

			protected override void DrawGizmos()
			{
				ExtraGizmos.DrawPoint(position, size, centerSphere, constantSize);
			}
		}

		private class LineGizmoInstance : GizmoInstance
		{
			private readonly Vector3 start;
			private readonly Vector3 end;

			public LineGizmoInstance(Vector3 start, Vector3 end, Color color, float duration) : base(color, duration)
			{
				this.start = start;
				this.end = end;
			}

			protected override void DrawGizmos()
			{
				Gizmos.DrawLine(start, end);
			}
		}

		private class CustomGizmoInstance : GizmoInstance
		{
			private readonly Action drawer;

			public CustomGizmoInstance(Action drawer, Color color, float duration) : base(color, duration)
			{
				this.drawer = drawer;
			}

			protected override void DrawGizmos()
			{
				drawer?.Invoke();
			}
		}

		private class ShapeGizmoInstance : GizmoInstance
		{
			public const byte TYPE_CUBE = 0;
			public const byte TYPE_SPHERE = 1;
			public const byte TYPE_CAPSULE = 2;

			private byte type;
			private Vector3 position;
			private Quaternion rotation;
			private Vector3 size;

			public ShapeGizmoInstance(byte type, Vector3 position, Quaternion rotation, Vector3 size, Color color, float duration) : base(color, duration)
			{
				this.type = type;
				this.position = position;
				this.rotation = rotation;
				this.size = size;
			}

			protected override void DrawGizmos()
			{
				Gizmos.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
				switch(type)
				{
					case TYPE_CUBE:
						ExtraGizmos.DrawWireCube(Vector3.zero, size);
						break;
					case TYPE_SPHERE:
						ExtraGizmos.DrawWireSphere(Vector3.zero, size.x, 16, false);
						break;
					case TYPE_CAPSULE:
						ExtraGizmos.DrawWireCapsule(Vector3.zero, Axis.Y, size.x, size.y, 16);
						break;
				}
			}
		}

		private static List<GizmoInstance> instances = new List<GizmoInstance>();

		private static StringBuilder stringBuilder = new StringBuilder();

		private static bool gizmosSubscribed = false;

		private static void AddGizmo(GizmoInstance instance)
		{
#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				Debug.LogWarning("Debug Gizmos are only supported during play mode.");
				return;
			}
			instances.Add(instance);
#endif
			UpdateLoopScriptInstance.InitIfRequired();
			if(!gizmosSubscribed)
			{
				UpdateLoop.OnDrawGizmosRuntime += OnDrawGizmos;
				gizmosSubscribed = true;
			}
		}

		/// <summary>
		/// Temporarily draws a Raycast trajectory and its hit point as gizmos.
		/// </summary>
		public static void DrawRaycast(Ray ray, RaycastHit? hit = null, float maxDistance = 1000, float hitSize = 0.05f, Color? color = null, float duration = 1f)
		{
			var distance = GetMaxDistance(hit.HasValue ? new RaycastHit[] { hit.Value } : null, 1, maxDistance);
			AddGizmo(new RaycastGizmoInstance(ray, hitSize, hit.HasValue ? new RaycastHit[] { hit.Value } : Array.Empty<RaycastHit>(), distance, color ?? Color.white, duration));
		}

		/// <summary>
		/// Temporarily draws a Raycast trajectory and all hit points as gizmos.
		/// </summary>
		public static void DrawRaycastAll(Ray ray, RaycastHit[] hits, int hitCount, float maxDistance = 1000, float hitSize = 0.05f, Color? color = null, float duration = 1f)
		{
			AddGizmo(new RaycastGizmoInstance(ray, hitSize, hits, maxDistance, color ?? Color.white, duration));
		}

		/// <summary>
		/// Temporarily draws a SphereCast trajectory and its hit point as gizmos.
		/// </summary>
		public static void DrawSphereCast(Ray ray, float radius, RaycastHit? hit = null, float maxDistance = 1000, Color? color = null, float duration = 1f)
		{
			var distance = GetMaxDistance(hit.HasValue ? new RaycastHit[] { hit.Value } : null, 1, maxDistance);
			AddGizmo(new SphereCastGizmoInstance(ray, radius, hit.HasValue ? new RaycastHit[] { hit.Value } : Array.Empty<RaycastHit>(), distance, color ?? Color.white, duration));
		}

		/// <summary>
		/// Temporarily draws a SphereCast trajectory and all its hit points as gizmos.
		/// </summary>
		public static void DrawSphereCastAll(Ray ray, float radius, RaycastHit[] hits, int hitCount, float maxDistance = 1000, Color? color = null, float duration = 1f)
		{
			AddGizmo(new SphereCastGizmoInstance(ray, radius, hits, maxDistance, color ?? Color.white, duration));
		}

		/// <summary>
		/// Temporarily draws a point gizmo at the given location.
		/// </summary>
		public static void DrawPoint(Vector3 point, float size, bool centerSphere = false, Color? color = null, bool constantSize = false, float duration = 1f)
		{
			AddGizmo(new PointGizmoInstance(point, size, centerSphere, constantSize, color ?? Color.white, duration));
		}

		/// <summary>
		/// Temporarily draws a line gizmo between the given points.
		/// </summary>
		public static void DrawLine(Vector3 start, Vector3 end, Color? color = null, float duration = 1f)
		{
			AddGizmo(new LineGizmoInstance(start, end, color ?? Color.white, duration));
		}

		/// <summary>
		/// Temporarily draws a line gizmo from the given start towards the given direction.
		/// </summary>
		public static void DrawRay(Vector3 start, Vector3 direction, float length, Color? color = null, float duration = 1f)
		{
			AddGizmo(new LineGizmoInstance(start, start + direction.normalized * length, color ?? Color.white, duration));
		}

		/// <summary>
		/// Temporarily draws a cube gizmo at the given position and rotation with the given size.
		/// </summary>
		public static void DrawCube(Vector3 position, Quaternion rotation, Vector3 size, Color? color = null, float duration = 1f)
		{
			AddGizmo(new ShapeGizmoInstance(ShapeGizmoInstance.TYPE_CUBE, position, rotation, size, color ?? Color.white, duration));
		}

		/// <summary>
		/// Temporarily draws a sphere gizmo at the given position and rotation with the given radius.
		/// </summary>
		public static void DrawSphere(Vector3 position, Quaternion rotation, float radius, Color? color = null, float duration = 1f)
		{
			AddGizmo(new ShapeGizmoInstance(ShapeGizmoInstance.TYPE_SPHERE, position, rotation, Vector3.one * radius, color ?? Color.white, duration));
		}

		/// <summary>
		/// Temporarily draws a capsule gizmo at the given position and rotation with the given radius and height.
		/// </summary>
		public static void DrawCapsule(Vector3 position, Quaternion rotation, float radius, float height, Color? color = null, float duration = 1f)
		{
			AddGizmo(new ShapeGizmoInstance(ShapeGizmoInstance.TYPE_CAPSULE, position, rotation, new Vector3(radius, height, 0), color ?? Color.white, duration));
		}

		/// <summary>
		/// Temporarily draws a custom gizmo with the given drawer action.
		/// </summary>
		public static void DrawCustomGizmo(Action drawer, Color? color = null, float duration = 1f)
		{
			AddGizmo(new CustomGizmoInstance(drawer, color ?? Color.white, duration));
		}

		/// <summary>
		/// Logs an array's content to the console.
		/// </summary>
		public static void LogArray<T>(string message, IEnumerable<T> array, Func<T, string> elementFunc = null)
		{
			if(elementFunc == null) elementFunc = (t) => t.ToString();
			stringBuilder.Clear();
			if(!string.IsNullOrEmpty(message)) stringBuilder.Append(message + " ");
			if(array != null)
			{
				stringBuilder.AppendLine($"{array.GetType().GetElementType()}[{array.Count()}]");
				int i = 0;
				foreach(var elem in array)
				{
					stringBuilder.AppendLine($"{i}: {(elem != null ? elementFunc(elem) : "(null)")}");
					i++;
				}
			}
			else
			{
				stringBuilder.AppendLine("(null)");
			}
			Debug.Log(stringBuilder.ToString());
		}

		/// <summary>
		/// Logs a transform's position, rotation and scale to the console
		/// </summary>
		public static void LogTransform(string message, Transform t, bool oneLine = false, bool position = true, bool rotation = true, bool scale = true)
		{
			stringBuilder.Clear();
			if(message == null && t) stringBuilder.Append(t.name);
			stringBuilder.AppendLine(message);
			if(t)
			{
				if(!oneLine)
				{
					if(position) stringBuilder.AppendLine("  Position: " + t.position);
					if(rotation) stringBuilder.AppendLine("  Rotation: " + t.eulerAngles);
					if(scale) stringBuilder.AppendLine("  Scale (Local): " + t.localScale);
				}
				else
				{
					if(position) stringBuilder.Append(" Pos: " + t.position);
					if(rotation) stringBuilder.Append(" Rot: " + t.eulerAngles);
					if(scale) stringBuilder.Append(" Scale (Local): " + t.localScale);
				}
			}
			else
			{
				stringBuilder.Append(" (null)");
			}
			Debug.Log(stringBuilder.ToString());
		}

		private static void OnDrawGizmos()
		{
			foreach(var i in instances)
			{
				if(i.Expired) continue;
				Gizmos.color = i.color.MultiplyAlpha(i.Life);
				i.Draw();
				Gizmos.color = Color.white;
				Gizmos.matrix = Matrix4x4.identity;
			}
			instances.RemoveAll((i) => i.Expired);
		}

		private static float GetMaxDistance(RaycastHit[] hits, int hitCount, float maxDistance)
		{
			if(hits != null)
			{
				float distance = 0;
				for(int i = 0; i < hitCount; i++)
				{
					if(hits[i].collider != null)
					{
						distance = Mathf.Max(distance, hits[i].distance);
					}
				}
				if(distance == 0) distance = maxDistance;
				return distance;
			}
			else
			{
				return maxDistance;
			}
		}
	}
}
