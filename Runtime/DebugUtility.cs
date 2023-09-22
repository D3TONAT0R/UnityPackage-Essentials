﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace D3T
{
	/// <summary>
	/// Utility functions for debugging.
	/// </summary>
	public class DebugUtility
	{
		private abstract class GizmoInstance
		{
			public readonly float duration;
			public float timeLeft;
			public Color color;

			public GizmoInstance(Color color, float duration)
			{
				this.duration = duration;
				timeLeft = duration;
				this.color = color;
			}

			public abstract void Draw();
		}

		private class RaycastGizmoInstance : GizmoInstance
		{
			private Ray ray;
			private float radius;
			private IEnumerable<RaycastHit> hits;
			private float distance;

			public RaycastGizmoInstance(Ray ray, float radius, IEnumerable<RaycastHit> hits, float distance, Color color, float duration) : base(color, duration)
			{
				this.ray = ray;
				this.radius = Mathf.Max(0, radius);
				this.hits = hits;
				this.distance = distance;
			}

			public override void Draw()
			{
				if(radius > 0)
				{
					Gizmos.DrawWireSphere(ray.origin, radius);
					if(hits == null || hits.Count() == 0) Gizmos.DrawWireSphere(ray.origin + ray.direction * distance, radius);
				}
				Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * distance);
				if(hits != null)
				{
					foreach(var hit in hits)
					{
						if(hit.collider)
						{
							if(radius > 0)
							{
								Gizmos.DrawWireSphere(hit.point + hit.normal * radius, radius);
							}
							else
							{
								Gizmos.DrawWireCube(hit.point, Vector3.one * 0.05f);
							}
							Gizmos.color = Gizmos.color.MultiplyAlpha(0.5f);
							Gizmos.DrawLine(hit.point, hit.point + hit.normal * 0.25f);
						}
					}
				}
				else
				{
					if(radius > 0) Gizmos.DrawWireSphere(ray.origin, radius);
				}
			}
		}

		private class PointGizmoInstance : GizmoInstance
		{
			public Vector3 position;
			public float size;


			public PointGizmoInstance(Vector3 position, float size, Color color, float duration) : base(color, duration)
			{
				this.position = position;
				this.size = size;
			}

			public override void Draw()
			{
				ExtraGizmos.DrawCrosshairs(position, size);
			}
		}


		private class LineGizmoInstance : GizmoInstance
		{
			private Vector3 start;
			private Vector3 end;

			public LineGizmoInstance(Vector3 start, Vector3 end, Color color, float duration) : base(color, duration)
			{
				this.start = start;
				this.end = end;
			}

			public override void Draw()
			{
				Gizmos.DrawLine(start, end);
			}
		}

		private static List<GizmoInstance> instances = new List<GizmoInstance>();

		[OnDrawGizmosRuntime]
		private static void OnDrawGizmos()
		{
			foreach(var i in instances)
			{
				bool decrTime = true;
#if UNITY_EDITOR
				if(!UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPaused)
				{
					decrTime = false;
				}
#endif
				if(decrTime)
				{
					i.timeLeft -= Time.unscaledDeltaTime;
				}
				float alpha = i.duration > 0 ? i.timeLeft / i.duration : 1f;
				Gizmos.color = i.color.MultiplyAlpha(alpha);
				i.Draw();
			}
			instances.RemoveAll((i) => i.timeLeft <= 0);
		}

		private static void AddGizmo(GizmoInstance instance)
		{
#if UNITY_EDITOR
			instances.Add(instance);
#endif
		}

		public static void DebugRaycast(Ray ray, RaycastHit? hit = null, float maxDistance = 1000, Color? color = null, float duration = 1f)
		{
			var distance = GetMaxDistance(hit.HasValue ? new RaycastHit[] { hit.Value } : null, 1, maxDistance);
			AddGizmo(new RaycastGizmoInstance(ray, 0, hit.HasValue ? new RaycastHit[] { hit.Value } : null, distance, color ?? Color.white, duration));
		}

		public static void DebugRaycastAll(Ray ray, RaycastHit[] hits, int hitCount, float maxDistance = 1000, Color? color = null, float duration = 1f)
		{
			var distance = GetMaxDistance(hits, hitCount, maxDistance);
			AddGizmo(new RaycastGizmoInstance(ray, 0, hits, distance, color ?? Color.white, duration));
		}

		public static void DebugSphereCast(Ray ray, float radius, RaycastHit? hit = null, float maxDistance = 1000, Color? color = null, float duration = 1f)
		{
			var distance = GetMaxDistance(hit.HasValue ? new RaycastHit[] { hit.Value } : null, 1, maxDistance);
			AddGizmo(new RaycastGizmoInstance(ray, radius, hit.HasValue ? new RaycastHit[] { hit.Value } : null, distance, color ?? Color.white, duration));
		}

		public static void DebugSphereCastAll(Ray ray, float radius, RaycastHit[] hits, int hitCount, float maxDistance = 1000, Color? color = null, float duration = 1f)
		{
			var distance = GetMaxDistance(hits, hitCount, maxDistance);
			AddGizmo(new RaycastGizmoInstance(ray, radius, hits, distance, color ?? Color.white, duration));
		}

		public static void DebugPoint(Vector3 point, float size, Color? color = null, float duration = 1f)
		{
			AddGizmo(new PointGizmoInstance(point, size, color ?? Color.white, duration));
		}

		public static void DebugLine(Vector3 start, Vector3 end, Color? color = null, float duration = 1f)
		{
			AddGizmo(new LineGizmoInstance(start, end, color ?? Color.white, duration));
		}

		public static void DebugRay(Vector3 start, Vector3 direction, float length, Color? color = null, float duration = 1f)
		{
			AddGizmo(new LineGizmoInstance(start, start + direction.normalized * length, color ?? Color.white, duration));
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

		public static void LogArray<T>(string message, IEnumerable<T> array, Func<T, string> elementFunc = null)
		{
			if(elementFunc == null) elementFunc = (t) => t.ToString();
			var sb = new StringBuilder();
			if(!string.IsNullOrEmpty(message)) sb.Append(message + " ");
			sb.AppendLine($"{array.GetType().GetElementType()}[{array.Count()}]");
			int i = 0;
			foreach(var elem in array)
			{
				sb.AppendLine($"{i}: {(elem != null ? elementFunc(elem) : "(null)")}");
				i++;
			}
			Debug.Log(sb.ToString());
		}
	}
}
