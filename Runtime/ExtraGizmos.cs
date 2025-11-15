using System.Collections.Generic;
using UnityEngine;
using UnityEssentials.Meshes;

namespace UnityEssentials
{
	public struct GizmoColorScope : System.IDisposable
	{
		private Color color;

		public GizmoColorScope(Color c)
		{
			color = Gizmos.color;
			Gizmos.color = c;
		}

		public void Dispose()
		{
			Gizmos.color = color;
		}
	}

	/// <summary>
	/// Provides additional gizmo drawing methods on top of Unity's own Gizmos.
	/// </summary>
	public class ExtraGizmos
	{
		internal static Mesh sphereMesh;
		internal static Mesh planeMesh;
		internal static Mesh discMesh;
		internal static Mesh cylinderMesh;
		internal static Mesh capsuleCenterMesh;
		internal static Mesh capsuleCapMesh;
		internal static Mesh coneMesh;

		internal static GUIStyle labelStyle;
		internal static GUIStyle boxStyle;

		private static List<Vector3> circlePointCache = new List<Vector3>();

		static ExtraGizmos()
		{
			var builder = new MeshBuilder();

			builder.AddSphere(Vector3.zero, 1, 16, 16);
			sphereMesh = builder.CreateMesh();

			builder.Clear();
			builder.AddQuad(new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0, -0.5f), new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f), Vector3.up);
			planeMesh = builder.CreateMesh();

			builder.Clear();
			builder.AddCircle(Vector3.zero, Vector3.up, 1f, 32);
			discMesh = builder.CreateMesh();

			builder.Clear();
			builder.AddCylinder(Vector3.zero, 1, 1, 16, true);
			cylinderMesh = builder.CreateMesh();

			builder.Clear();
			builder.AddCylinder(Vector3.zero, 1, 1, 16, false);
			capsuleCenterMesh = builder.CreateMesh();
			builder.Clear();
			builder.AddHemisphere(Vector3.zero, 1, 1, 16, 8);
			capsuleCapMesh = builder.CreateMesh();

			builder.Clear();
			builder.AddCone(Vector3.zero, AxisDirection.YPos, 1f, 1f, 16, true);
			coneMesh = builder.CreateMesh();
		}

		#region Enhanced built-in shapes

		/// <summary>
		/// Draws a wireframe sphere gizmo with an optional boundary circle.
		/// </summary>
		public static void DrawWireSphere(Vector3 center, float radius, int segments = 32, bool boundary = true, bool drawPickShape = false)
		{
			DrawWireCircle(center, Vector3.up, radius, segments);
			DrawWireCircle(center, Vector3.right, radius, segments);
			DrawWireCircle(center, Vector3.forward, radius, segments);
			if(boundary)
			{
				if(Camera.current.orthographic)
				{
					Vector3 normal = -Gizmos.matrix.inverse.MultiplyVector(Camera.current.transform.forward);
					float sqrMagnitude = normal.sqrMagnitude;
					float num0 = radius * radius;
					DrawWireCircle(center - num0 * normal / sqrMagnitude, normal, radius, segments);
				}
				else
				{
					Vector3 normal = -Gizmos.matrix.inverse.MultiplyPoint3x4(Camera.current.transform.position - center);
					float sqrMagnitude = normal.sqrMagnitude;
					float num0 = radius * radius;
					float num1 = num0 * num0 / sqrMagnitude;
					float num2 = Mathf.Sqrt(num0 - num1);
					DrawWireCircle(center - num0 * normal / sqrMagnitude, normal, num2, segments);
				}
			}
			if(drawPickShape)
			{
				using(new GizmoColorScope(Color.clear))
				{
					DrawSphere(center, radius);
				}
			}
		}

		/// <summary>
		/// Draws a solid sphere gizmo.
		/// </summary>
		public static void DrawSphere(Vector3 center, float radius)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.identity, Vector3.one * radius);
			Gizmos.DrawMesh(sphereMesh);
			Gizmos.matrix = lastMatrix;
		}

		/// <summary>
		/// Draws a combined wireframe and solid sphere gizmo.
		/// </summary>
		public static void DrawCombinedSphere(Vector3 center, float radius, float fillAlpha, int segments = 32, bool boundary = true)
		{
			DrawWireSphere(center, radius, segments, boundary);
			using(new GizmoColorScope(Gizmos.color.MultiplyAlpha(fillAlpha)))
			{
				DrawSphere(center, radius);
			}
		}



		/// <summary>
		/// Draws a wireframe cube gizmo.
		/// </summary>
		public static void DrawWireCube(Vector3 center, Vector3 size, bool drawPickShape = false)
		{
			Gizmos.DrawWireCube(center, size);
			if(drawPickShape)
			{
				using(new GizmoColorScope(Color.clear))
				{
					Gizmos.DrawCube(center, size);
				}
			}
		}

		/// <summary>
		/// Draws a wireframe cube gizmo between the two given points.
		/// </summary>
		public static void DrawWireCubeBetween(Vector3 a, Vector3 b, bool drawPickShape = false)
		{
			var center = (a + b) * 0.5f;
			var size = (b - a).Abs();
			DrawWireCube(center, size, drawPickShape);
		}

		/// <summary>
		/// Draws a solid cube gizmo.
		/// </summary>
		public static void DrawCube(Vector3 center, Vector3 size)
		{
			Gizmos.DrawCube(center, size);
		}

		/// <summary>
		/// Draws a solid cube gizmo between the two given points.
		/// </summary>
		public static void DrawCubeBetween(Vector3 a, Vector3 b)
		{
			var center = (a + b) * 0.5f;
			var size = (b - a).Abs();
			DrawCube(center, size);
		}

		/// <summary>
		/// Draws a combined wireframe and solid cube gizmo.
		/// </summary>
		public static void DrawCombinedCube(Vector3 center, Vector3 size, float fillAlpha)
		{
			DrawWireCube(center, size);
			using(new GizmoColorScope(Gizmos.color.MultiplyAlpha(fillAlpha)))
			{
				Gizmos.DrawCube(center, size);
			}
		}

		/// <summary>
		/// Draws a combined wireframe and solid cube gizmo between the two given points.
		/// </summary>
		public static void DrawCombinedCubeBetween(Vector3 a, Vector3 b, float fillAlpha)
		{
			var center = (a + b) * 0.5f;
			var size = (b - a).Abs();
			DrawCombinedCube(center, size, fillAlpha);
		}

		#endregion

		#region New shapes

		/// <summary>
		/// Draws a wireframe circle gizmo.
		/// </summary>
		public static void DrawWireCircle(Vector3 center, Vector3 normal, float radius, int segments = 64, bool drawPickShape = false)
		{
			segments = Mathf.Clamp(segments, 3, 256);
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.LookRotation(normal), Vector3.one * radius);
			MeshBuilderBase.GetCirclePoints(circlePointCache, segments, 1);
			for(int i = 0; i < segments - 1; i++)
			{
				Gizmos.DrawLine(circlePointCache[i], circlePointCache[i + 1]);
			}
			Gizmos.DrawLine(circlePointCache[segments - 1], circlePointCache[0]);
			Gizmos.matrix = lastMatrix;
			if(drawPickShape)
			{
				using(new GizmoColorScope(Color.clear))
				{
					DrawCircle(center, normal, radius);
					DrawCircle(center, -normal, radius);
				}
			}
		}

		/// <summary>
		/// Draws a solid circle gizmo.
		/// </summary>
		public static void DrawCircle(Vector3 center, Vector3 normal, float radius, bool doubleSided = true)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.LookRotation(normal) * Quaternion.Euler(90, 0, 0), Vector3.one * radius);
			Gizmos.DrawMesh(discMesh);
			if(doubleSided)
			{
				Gizmos.matrix *= Matrix4x4.Scale(new Vector3(1, 1, -1));
				Gizmos.DrawMesh(discMesh);
			}
			Gizmos.matrix = lastMatrix;
		}

		/// <summary>
		/// Draws a combined wireframe and solid circle gizmo.
		/// </summary>
		public static void DrawCombinedCircle(Vector3 center, Vector3 normal, float radius, float fillAlpha, int segments = 64, bool doubleSided = true)
		{
			DrawWireCircle(center, normal, radius, segments);
			using(new GizmoColorScope(Gizmos.color.MultiplyAlpha(fillAlpha)))
			{
				DrawCircle(center, normal, radius, doubleSided);
			}
		}



		/// <summary>
		/// Draws an arc between the given angles.
		/// </summary>
		public static void DrawArc(Vector3 center, Vector3 up, Vector3 forward, float radius, float fromDegrees, float toDegrees, bool edges = false, int segments = 32)
		{
			segments = Mathf.Clamp(segments, 3, 256);
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.LookRotation(forward, up), Vector3.one);
			circlePointCache.Clear();
			for(int i = 0; i <= segments; i++)
			{
				float a = Mathf.Lerp(fromDegrees, toDegrees, i / (float)segments) * Mathf.Deg2Rad;
				circlePointCache.Add(new Vector3(Mathf.Sin(a), 0, Mathf.Cos(a)) * radius);
			}
			DrawPath(circlePointCache);
			if(edges)
			{
				Gizmos.DrawLine(Vector3.zero, circlePointCache[0]);
				Gizmos.DrawLine(Vector3.zero, circlePointCache[circlePointCache.Count - 1]);
			}
			Gizmos.matrix = lMatrix;
		}



		/// <summary>
		/// Draws a wireframe cylinder gizmo.
		/// </summary>
		public static void DrawWireCylinder(Vector3 center, Quaternion rotation, float radius, float height, int segments = 64, bool drawPickShape = false)
		{
			segments = Mathf.Clamp(segments, 3, 256);
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation * Quaternion.Euler(90, 0, 0), Vector3.one);
			float h2 = height * 0.5f;
			DrawWireCircle(Vector3.back * h2, Vector3.back, radius, segments);
			DrawWireCircle(Vector3.forward * h2, Vector3.forward, radius, segments);
			DrawLineFrom(new Vector3(-radius, 0, -h2), Vector3.forward, height);
			DrawLineFrom(new Vector3(radius, 0, -h2), Vector3.forward, height);
			DrawLineFrom(new Vector3(0, -radius, -h2), Vector3.forward, height);
			DrawLineFrom(new Vector3(0, radius, -h2), Vector3.forward, height);
			Gizmos.matrix = lastMatrix;
			if(drawPickShape)
			{
				using(new GizmoColorScope(Color.clear))
				{
					DrawCylinder(center, rotation, radius, height);
				}
			}
		}

		/// <summary>
		/// Draws a wireframe cylinder gizmo.
		/// </summary>
		public static void DrawWireCylinder(Vector3 center, Axis axis, float radius, float height, int segments = 64, bool drawPickShape = false)
		{
			var rotation = GetAxisRotation(axis);
			DrawWireCylinder(center, rotation, radius, height, segments, drawPickShape);
		}

		/// <summary>
		/// Draws a solid cylinder gizmo.
		/// </summary>
		public static void DrawCylinder(Vector3 center, Quaternion rotation, float radius, float height)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation, new Vector3(radius, height, radius));
			Gizmos.DrawMesh(cylinderMesh);
			Gizmos.matrix = lastMatrix;
		}

		/// <summary>
		/// Draws a solid cylinder gizmo.
		/// </summary>
		public static void DrawCylinder(Vector3 center, Axis axis, float radius, float height)
		{
			var rotation = GetAxisRotation(axis);
			DrawCylinder(center, rotation, radius, height);
		}

		/// <summary>
		/// Draws a combined wireframe and solid cylinder gizmo.
		/// </summary>
		public static void DrawCombinedCylinder(Vector3 center, Quaternion rotation, float radius, float height, float fillAlpha, int segments = 64)
		{
			DrawWireCylinder(center, rotation, radius, height, segments);
			using(new GizmoColorScope(Gizmos.color.MultiplyAlpha(fillAlpha)))
			{
				DrawCylinder(center, rotation, radius, height);
			}
		}

		/// <summary>
		/// Draws a combined wireframe and solid cylinder gizmo.
		/// </summary>
		public static void DrawCombinedCylinder(Vector3 center, Axis axis, float radius, float height, float fillAlpha, int segments = 64)
		{
			var rotation = GetAxisRotation(axis);
			DrawCombinedCylinder(center, rotation, radius, height, fillAlpha, segments);
		}



		/// <summary>
		/// Draws a wireframe capsule gizmo.
		/// </summary>
		public static void DrawWireCapsule(Vector3 center, Quaternion rotation, float radius, float height, int segments = 64, bool drawPickShape = false)
		{
			segments = Mathf.Clamp(segments, 3, 256);
			var h = Mathf.Max(height - radius * 2, 0);
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation * Quaternion.Euler(90, 0, 0), Vector3.one);
			float h2 = h * 0.5f;
			DrawWireCircle(Vector3.back * h2, Vector3.back, radius, segments);
			DrawArc(Vector3.back * h2, Vector3.up, Vector3.back, radius, -90, 90, false, segments / 2);
			DrawArc(Vector3.back * h2, Vector3.right, Vector3.back, radius, -90, 90, false, segments / 2);
			DrawWireCircle(Vector3.forward * h2, Vector3.forward, radius, segments);
			DrawArc(Vector3.forward * h2, Vector3.up, Vector3.forward, radius, -90, 90, false, segments / 2);
			DrawArc(Vector3.forward * h2, Vector3.right, Vector3.forward, radius, -90, 90, false, segments / 2);
			if(h > 0)
			{
				DrawLineFrom(new Vector3(-radius, 0, -h2), Vector3.forward, h);
				DrawLineFrom(new Vector3(radius, 0, -h2), Vector3.forward, h);
				DrawLineFrom(new Vector3(0, -radius, -h2), Vector3.forward, h);
				DrawLineFrom(new Vector3(0, radius, -h2), Vector3.forward, h);
			}
			Gizmos.matrix = lastMatrix;
			if(drawPickShape)
			{
				using(new GizmoColorScope(Color.clear))
				{
					DrawCapsule(center, rotation, radius, height);
				}
			}
		}

		/// <summary>
		/// Draws a wireframe capsule gizmo.
		/// </summary>
		public static void DrawWireCapsule(Vector3 center, Axis axis, float radius, float height, int segments = 64, bool drawPickShape = false)
		{
			var rotation = GetAxisRotation(axis);
			DrawWireCapsule(center, rotation, radius, height, segments, drawPickShape);
		}

		/// <summary>
		/// Draws a solid capsule gizmo.
		/// </summary>
		public static void DrawCapsule(Vector3 center, Quaternion rotation, float radius, float height)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation, Vector3.one);
			height = Mathf.Max(0, height - radius * 2);
			if(height > 0)
			{
				Gizmos.DrawMesh(capsuleCenterMesh, Vector3.zero, Quaternion.identity, new Vector3(radius, height, radius));
			}
			float h2 = height / 2f;
			Gizmos.DrawMesh(capsuleCapMesh, Vector3.up * h2, Quaternion.identity, Vector3.one * radius);
			Gizmos.DrawMesh(capsuleCapMesh, Vector3.down * h2, Quaternion.Euler(180, 0, 0), Vector3.one * radius);
			Gizmos.matrix = lastMatrix;
		}

		/// <summary>
		/// Draws a solid capsule gizmo.
		/// </summary>
		public static void DrawCapsule(Vector3 center, Axis axis, float radius, float height)
		{
			var rotation = GetAxisRotation(axis);
			DrawCapsule(center, rotation, radius, height);
		}

		/// <summary>
		/// Draws a combined wireframe and solid capsule gizmo.
		/// </summary>
		public static void DrawCombinedCapsule(Vector3 center, Quaternion rotation, float radius, float height, float fillAlpha, int segments = 64)
		{
			DrawWireCapsule(center, rotation, radius, height, segments);
			using(new GizmoColorScope(Gizmos.color.MultiplyAlpha(fillAlpha)))
			{
				DrawCapsule(center, rotation, radius, height);
			}
		}

		/// <summary>
		/// Draws a combined wireframe and solid capsule gizmo.
		/// </summary>
		public static void DrawCombinedCapsule(Vector3 center, Axis axis, float radius, float height, float fillAlpha, int segments = 64)
		{
			var rotation = GetAxisRotation(axis);
			DrawCombinedCapsule(center, rotation, radius, height, fillAlpha, segments);
		}



		/// <summary>
		/// Draws a wireframe cone gizmo.
		/// </summary>
		public static void DrawWireCone(Vector3 center, Quaternion rotation, float radius, float height, int circleSegments = 64, bool drawPickShape = false)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation, Vector3.one);
			DrawWireCircle(Vector3.zero, Vector3.up, radius, circleSegments);
			var top = Vector3.up * height;
			Gizmos.DrawLine(Vector3.left * radius, top);
			Gizmos.DrawLine(Vector3.right * radius, top);
			Gizmos.DrawLine(Vector3.forward * radius, top);
			Gizmos.DrawLine(Vector3.back * radius, top);
			Gizmos.matrix = lastMatrix;
			if(drawPickShape)
			{
				using(new GizmoColorScope(Color.clear))
				{
					DrawCone(center, rotation, radius, height);
				}
			}
		}

		/// <summary>
		/// Draws a wireframe cone gizmo.
		/// </summary>
		public static void DrawWireCone(Vector3 center, AxisDirection direction, float radius, float height, int circleSegments = 64, bool drawPickShape = false)
		{
			var rotation = GetAxisRotation(direction);
			DrawWireCone(center, rotation, radius, height, circleSegments, drawPickShape);
		}

		/// <summary>
		/// Draws a solid cone gizmo.
		/// </summary>
		public static void DrawCone(Vector3 center, Quaternion rotation, float radius, float height)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation, new Vector3(radius, height, radius));
			Gizmos.DrawMesh(coneMesh);
			Gizmos.matrix = lastMatrix;
		}

		/// <summary>
		/// Draws a solid cone gizmo.
		/// </summary>
		public static void DrawCone(Vector3 center, AxisDirection direction, float radius, float height)
		{
			var rotation = GetAxisRotation(direction);
			DrawCone(center, rotation, radius, height);
		}

		/// <summary>
		/// Draws a combined wireframe and solid cone gizmo.
		/// </summary>
		public static void DrawCombinedCone(Vector3 center, Quaternion rotation, float radius, float height, float fillAlpha, int circleSegments = 64)
		{
			DrawWireCone(center, rotation, radius, height, circleSegments);
			using(new GizmoColorScope(Gizmos.color.MultiplyAlpha(fillAlpha)))
			{
				DrawCone(center, rotation, radius, height);
			}
		}

		/// <summary>
		/// Draws a combined wireframe and solid cone gizmo.
		/// </summary>
		public static void DrawCombinedCone(Vector3 center, AxisDirection direction, float radius, float height, float fillAlpha, int circleSegments = 64)
		{
			var rotation = GetAxisRotation(direction);
			DrawCombinedCone(center, rotation, radius, height, fillAlpha, circleSegments);
		}


		/// <summary>
		/// Draws a line between the two given points.
		/// </summary>
		public static void DrawLine(Vector3 a, Vector3 b)
		{
			Gizmos.DrawLine(a, b);
		}

		/// <summary>
		/// Draws a line starting at the given point towards the given direction.
		/// </summary>
		public static void DrawLineFrom(Vector3 point, Vector3 direction, float length)
		{
			direction = direction.normalized;
			Gizmos.DrawLine(point, point + direction * length);
		}



		/// <summary>
		/// Draws a red, green and blue line representing three axes.
		/// </summary>
		public static void DrawAxes(Vector3 position, Quaternion rotation, float size, bool colors = true)
		{
			var lColor = Gizmos.color;
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(position, rotation, Vector3.one);
			if(colors) Gizmos.color = lColor * Color.red;
			DrawArrow(Vector3.zero, Vector3.right, size);
			if(colors) Gizmos.color = lColor * Color.green;
			DrawArrow(Vector3.zero, Vector3.up, size);
			if(colors) Gizmos.color = lColor * Color.blue;
			DrawArrow(Vector3.zero, Vector3.forward, size);
			Gizmos.color = lColor;
			Gizmos.matrix = lMatrix;
		}

		/// <summary>
		/// Draws the given transform's axes as gizmos.
		/// </summary>
		public static void DrawAxes(Transform transform, float size, bool colors = true)
		{
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.GetTRSMatrix(false);
			DrawAxes(Vector3.zero, Quaternion.identity, size, colors);
			Gizmos.matrix = lMatrix;
		}

		/// <summary>
		/// Draws a point gizmo.
		/// </summary>
		public static void DrawPoint(Vector3 point, float radius, bool centerSphere = true, bool constantSize = false, bool drawPickShape = false)
		{
			if(constantSize) MakeConstantSize(point, ref radius);
			Gizmos.DrawLine(point + Vector3.left * radius, point + Vector3.right * radius);
			Gizmos.DrawLine(point + Vector3.down * radius, point + Vector3.up * radius);
			Gizmos.DrawLine(point + Vector3.back * radius, point + Vector3.forward * radius);
			if(centerSphere)
			{
				DrawWireSphere(point, radius * 0.5f, 16, true, drawPickShape);
			}
		}



		/// <summary>
		/// Draws an arrow gizmo.
		/// </summary>
		public static void DrawArrow(Vector3 origin, Quaternion rotation, float length, float? fixedHeadLength = null, bool simple = false)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(origin, rotation, Vector3.one);
			Gizmos.DrawLine(Vector3.zero, Vector3.forward * length);
			float headLength = fixedHeadLength ?? length * 0.25f;
			float headStart = length - headLength;
			if(simple)
			{
				var head = Vector3.forward * length;
				headLength *= 0.7f;
				Gizmos.DrawLine(head, head + new Vector3(-headLength, 0, -headLength));
				Gizmos.DrawLine(head, head + new Vector3(headLength, 0, -headLength));
			}
			else
			{
				DrawWireCone(Vector3.forward * headStart, AxisDirection.ZPos, headLength * 0.25f, headLength, 4, true);
			}
			Gizmos.matrix = lastMatrix;
		}

		/// <summary>
		/// Draws an arrow gizmo.
		/// </summary>
		public static void DrawArrow(Vector3 origin, Vector3 direction, float length, float? fixedHeadLength = null, bool simple = false)
		{
			DrawArrow(origin, Quaternion.LookRotation(direction), length, fixedHeadLength, simple);
		}

		/// <summary>
		/// Draws an arrow gizmo.
		/// </summary>
		public static void DrawArrow(Vector3 origin, AxisDirection direction, float length, float? fixedHeadLength = null, bool simple = false)
		{
			DrawArrow(origin, Quaternion.LookRotation(direction.GetDirectionVector()), length, fixedHeadLength, simple);
		}

		/// <summary>
		/// Draws an arrow gizmo between the given points.
		/// </summary>
		public static void DrawArrowBetween(Vector3 a, Vector3 b, float? fixedHeadLength = null, bool simple = false)
		{
			if(a == b) return;
			b -= a;
			var dir = b.normalized;
			var length = b.magnitude;
			DrawArrow(a, dir, length, fixedHeadLength, simple);
		}

		/// <summary>
		/// Draws a rectangle gizmo.
		/// </summary>
		public static void DrawRectangle(Vector3 center, Quaternion rotation, Vector2 size, bool doubleSided = true)
		{
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation, size.XVY(1));
			Gizmos.DrawMesh(planeMesh);
			if(doubleSided)
			{
				Gizmos.matrix *= Matrix4x4.Scale(new Vector3(1, 1, -1));
				Gizmos.DrawMesh(planeMesh);
			}
			Gizmos.matrix = lMatrix;
		}

		/// <summary>
		/// Draws a rectangle gizmo.
		/// </summary>
		public static void DrawRectangle(Vector3 center, Vector3 up, Vector2 size)
		{
			DrawRectangle(center, Quaternion.LookRotation(up) * Quaternion.Euler(90, 0, 0), size);
		}

		/// <summary>
		/// Draws a wireframe rectangle gizmo.
		/// </summary>
		public static void DrawWireRectangle(Vector3 center, Vector3 up, Vector2 size, bool drawPickShape = false)
		{
			DrawWireRectangle(center, Quaternion.LookRotation(up) * Quaternion.Euler(90, 0, 0), size, drawPickShape);
		}

		/// <summary>
		/// Draws a wireframe rectangle gizmo.
		/// </summary>
		public static void DrawWireRectangle(Vector3 center, Quaternion rotation, Vector2 size, bool drawPickShape = false)
		{
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation, Vector3.one);
			var ext = size * 0.5f;
			var p0 = new Vector3(-ext.x, 0, -ext.y);
			var p1 = new Vector3(ext.x, 0, -ext.y);
			var p2 = new Vector3(-ext.x, 0, ext.y);
			var p3 = new Vector3(ext.x, 0, ext.y);
			Gizmos.DrawLine(p0, p1);
			Gizmos.DrawLine(p1, p3);
			Gizmos.DrawLine(p0, p2);
			Gizmos.DrawLine(p2, p3);
			Gizmos.matrix = lMatrix;
			if(drawPickShape)
			{
				using(new GizmoColorScope(Color.clear))
				{
					DrawRectangle(center, rotation, size);
				}
			}
		}

		/// <summary>
		/// Draws a combined wireframe and solid rectangle gizmo.
		/// </summary>
		public static void DrawCombinedRectangle(Vector3 center, Quaternion rotation, Vector2 size, float fillAlpha, bool doubleSided = true)
		{
			DrawWireRectangle(center, rotation, size);
			using(new GizmoColorScope(Gizmos.color.MultiplyAlpha(fillAlpha)))
			{
				DrawRectangle(center, rotation, size, doubleSided);
			}
		}

		/// <summary>
		/// Draws a combined wireframe and solid rectangle gizmo.
		/// </summary>
		public static void DrawCombinedRectangle(Vector3 center, Vector3 up, Vector2 size, float fillAlpha, bool doubleSided = true)
		{
			DrawCombinedRectangle(center, Quaternion.LookRotation(up) * Quaternion.Euler(90, 0, 0), size, fillAlpha, doubleSided);
		}

		/// <summary>
		/// Draws a rounded wire rectangle gizmo.
		/// </summary>
		public static void DrawWireRadiusRectangle(Vector3 center, Quaternion rotation, Vector2 size, float radius, bool grow = false)
		{
			if(radius <= 0)
			{
				DrawRectangle(center, rotation, size);
				return;
			}
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation, Vector3.one);
			if(!grow) size -= 2 * radius * Vector2.one;
			var ext = size * 0.5f;
			var p0 = new Vector3(-ext.x, 0, -ext.y);
			var p1 = new Vector3(ext.x, 0, -ext.y);
			var p2 = new Vector3(-ext.x, 0, ext.y);
			var p3 = new Vector3(ext.x, 0, ext.y);
			Gizmos.DrawLine(p0 + Vector3.back * radius, p1 + Vector3.back * radius);
			Gizmos.DrawLine(p1 + Vector3.right * radius, p3 + Vector3.right * radius);
			Gizmos.DrawLine(p0 + Vector3.left * radius, p2 + Vector3.left * radius);
			Gizmos.DrawLine(p2 + Vector3.forward * radius, p3 + Vector3.forward * radius);
			DrawArc(p0, Vector3.forward, Vector3.forward, radius, 180, 270, false, 8);
			DrawArc(p1, Vector3.forward, Vector3.forward, radius, 90, 180, false, 8);
			DrawArc(p2, Vector3.forward, Vector3.forward, radius, -90, 0, false, 8);
			DrawArc(p3, Vector3.forward, Vector3.forward, radius, 0, 90, false, 8);
			Gizmos.matrix = lMatrix;
		}

		/// <summary>
		/// Draws a rounded wire rectangle gizmo.
		/// </summary>
		public static void DrawWireRadiusRectangle(Vector3 center, Vector3 up, Vector2 size, float radius, bool grow = false)
		{
			DrawWireRadiusRectangle(center, Quaternion.LookRotation(up) * Quaternion.Euler(90, 0, 0), size, radius, grow);
		}

		/// <summary>
		/// Draws a rounded cube gizmo.
		/// </summary>
		public static void DrawWireRadiusCube(Vector3 center, Vector3 size, float radius, bool grow = false)
		{
			if(radius <= 0)
			{
				Gizmos.DrawWireCube(center, size);
				return;
			}
			size = size.Abs();
			float inset = grow ? 0 : radius;
			DrawWireRadiusRectangle(center + new Vector3(-size.x * 0.5f + inset, 0, 0), Vector3.right, size.ZY(), radius, grow);
			DrawWireRadiusRectangle(center + new Vector3(size.x * 0.5f - inset, 0, 0), Vector3.right, size.ZY(), radius, grow);
			DrawWireRadiusRectangle(center + new Vector3(0, -size.y * 0.5f + inset, 0), Vector3.up, size.XZ(), radius, grow);
			DrawWireRadiusRectangle(center + new Vector3(0, size.y * 0.5f - inset, 0), Vector3.up, size.XZ(), radius, grow);
			DrawWireRadiusRectangle(center + new Vector3(0, 0, -size.z * 0.5f + inset), Vector3.forward, size.XY(), radius, grow);
			DrawWireRadiusRectangle(center + new Vector3(0, 0, size.z * 0.5f - inset), Vector3.forward, size.XY(), radius, grow);
		}
		


		/// <summary>
		/// Draws a path between the given points.
		/// </summary>
		public static void DrawPath(params Vector3[] points)
		{
			for(int i = 0; i < points.Length - 1; i++)
			{
				Gizmos.DrawLine(points[i], points[i + 1]);
			}
		}

		/// <summary>
		/// Draws a path between the given points.
		/// </summary>
		public static void DrawPath(List<Vector3> points)
		{
			for(int i = 0; i < points.Count - 1; i++)
			{
				Gizmos.DrawLine(points[i], points[i + 1]);
			}
		}



		/// <summary>
		/// Draws a GUI text at the given point in the scene.
		/// </summary>
		public static void DrawText(Vector3 position, string text, Color? color = null, TextAnchor anchor = TextAnchor.UpperLeft, float fontSize = 0, float offset = 0, Vector2? offsetUnits = null, bool shadow = false, float maxDistance = 0)
		{
			if(!CheckDistance(position, maxDistance)) return;
#if UNITY_EDITOR
			if(labelStyle == null)
			{
				labelStyle = new GUIStyle(GUI.skin.label)
				{
					richText = true,
					normal = { textColor = Color.white },
					hover = { textColor = Color.white },
					active = { textColor = Color.white },
					fontSize = 12,
					wordWrap = false
				};
			}
			labelStyle.alignment = anchor;

			var lastColor = GUI.color;
			if(color.HasValue) GUI.color = color.Value;
			else GUI.color = Gizmos.color.WithAlpha(1);
			if(fontSize > 0) text = $"<size={fontSize}>{text}</size>";
			Label(Gizmos.matrix.MultiplyPoint(position), new GUIContent(text), labelStyle, offset, offsetUnits ?? Vector2.zero, shadow, fontSize);
			GUI.color = lastColor;
#endif
		}

		/// <summary>
		/// Draws a GUI text box at the given point in the scene.
		/// </summary>
		public static void DrawTextBox(Vector3 position, string text, Color? color = null, TextAnchor anchor = TextAnchor.UpperLeft, float fontSize = 0, float offset = 0, Vector2? offsetUnits = null, GUIStyle style = null, float maxDistance = 0)
		{
			if(!CheckDistance(position, maxDistance)) return;
#if UNITY_EDITOR
			if(boxStyle == null)
			{
				var backgroundTex = new Texture2D(1, 1);
				backgroundTex.SetPixel(0, 0, new Color(0.05f, 0.05f, 0.05f, 0.5f));
				backgroundTex.Apply();
				boxStyle = new GUIStyle(GUI.skin.box)
				{
					richText = true,
					normal = { textColor = Color.white, background = backgroundTex },
					hover = { textColor = Color.white },
					active = { textColor = Color.white },
					fontSize = 12,
					wordWrap = false
				};
			}

			var lastColor = GUI.color;
			if(color.HasValue) GUI.color = color.Value;
			else GUI.color = Gizmos.color.WithAlpha(1);
			if(fontSize > 0) text = $"<size={fontSize}>{text}</size>";
			Box(Gizmos.matrix.MultiplyPoint(position), new GUIContent(text), boxStyle, anchor, offset, offsetUnits ?? Vector2.zero);
			GUI.color = lastColor;
#endif
		}

#if UNITY_EDITOR
		private static void Label(Vector3 position, GUIContent content, GUIStyle style, float offset, Vector2 offsetUnits, bool shadow, float fontSize)
		{
			Vector3 vector = UnityEditor.HandleUtility.WorldToGUIPointWithDepth(position);
			if(!(vector.z < 0f))
			{
				UnityEditor.Handles.BeginGUI();
				var rect = WorldPointToSizedRect(position, content, style, style.alignment, offset, offsetUnits).Round();
				if(shadow)
				{
					var position2 = rect;
					float shadowOffset = fontSize > 0 ? fontSize * 0.08f : 1f;
					position2.x += shadowOffset;
					position2.y += shadowOffset;
					var lastGUIColor = GUI.color;
					GUI.color = Color.black.WithAlpha(GUI.color.a * 0.8f);
					GUI.Label(position2, content, style);
					GUI.color = lastGUIColor;
				}
				GUI.Label(rect, content, style);
				UnityEditor.Handles.EndGUI();
			}
		}

		private static void Box(Vector3 position, GUIContent content, GUIStyle style, TextAnchor anchor, float offset, Vector2 offsetUnits)
		{
			Vector3 vector = UnityEditor.HandleUtility.WorldToGUIPointWithDepth(position);
			if(!(vector.z < 0f))
			{
				UnityEditor.Handles.BeginGUI();
				var rect = WorldPointToSizedRect(position, content, style, anchor, offset, offsetUnits);
				GUI.Label(rect, content, style);
				UnityEditor.Handles.EndGUI();
			}
		}

		private static Rect WorldPointToSizedRect(Vector3 position, GUIContent content, GUIStyle style, TextAnchor anchor, float offset, Vector2 offsetUnits)
		{
			Vector2 pos = UnityEditor.HandleUtility.WorldToGUIPoint(position);
			var size = UnityEditor.HandleUtility.GetHandleSize(position);
			pos += (1f / size) * offsetUnits * 80;
			Vector2 rectSize = style.CalcSize(content);
			Rect rect = new Rect(pos.x, pos.y, rectSize.x, rectSize.y);
			switch(anchor)
			{
				case TextAnchor.UpperCenter:
					rect.x -= rect.width * 0.5f;
					rect.y += offset;
					break;
				case TextAnchor.UpperRight:
					rect.x -= rect.width;
					rect.x -= offset;
					rect.y += offset;
					break;
				case TextAnchor.UpperLeft:
					rect.x += offset;
					rect.y += offset;
					break;
				case TextAnchor.MiddleLeft:
					rect.y -= rect.height * 0.5f;
					rect.x += offset;
					break;
				case TextAnchor.MiddleCenter:
					rect.x -= rect.width * 0.5f;
					rect.y -= rect.height * 0.5f;
					break;
				case TextAnchor.MiddleRight:
					rect.x -= rect.width;
					rect.y -= rect.height * 0.5f;
					rect.x -= offset;
					break;
				case TextAnchor.LowerLeft:
					rect.y -= rect.height;
					rect.x += offset;
					rect.y -= offset;
					break;
				case TextAnchor.LowerCenter:
					rect.x -= rect.width * 0.5f;
					rect.y -= rect.height;
					rect.y -= offset;
					break;
				case TextAnchor.LowerRight:
					rect.x -= rect.width;
					rect.y -= rect.height;
					rect.x -= offset;
					rect.y -= offset;
					break;
			}

			return style.padding.Add(rect);
		}
#endif

		#endregion

		#region Terrain Projected Shapes

		/// <summary>
		/// Draws a circle that is projected onto the terrain's surface.
		/// </summary>
		public static void DrawTerrainProjectedCircle(Vector3 center, float radius, float yOffset = 0, int segments = 32, Terrain terrain = null)
		{
			if(!terrain) terrain = Terrain.activeTerrain;
			if(terrain)
			{
				for(int i = 0; i <= 64; i++)
				{
					float rad = (i / 64f) * Mathf.PI * 2f;
					var pos1 = center + new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * radius;
					pos1.y = terrain.GetAbsoluteHeightAt(pos1) + yOffset;
					rad = ((i + 1) / 64f) * Mathf.PI * 2f;
					var pos2 = center + new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * radius;
					pos2.y = terrain.GetAbsoluteHeightAt(pos2) + yOffset;
					Gizmos.DrawLine(pos1, pos2);
				}
			}
			else
			{
				DrawWireCircle(center.WithY(yOffset), Vector3.up, radius, segments);
			}
		}

		/// <summary>
		/// Draws a line that is projected onto the terrain's surface.
		/// </summary>
		public static void DrawTerrainProjectedLine(Vector3 from, Vector3 to, float yOffset = 0, int segments = 8, Terrain terrain = null)
		{
			if(!terrain) terrain = Terrain.activeTerrain;
			if(terrain)
			{
				Vector3 last = from;
				last.y = terrain.GetAbsoluteHeightAt(last) + yOffset;
				for(int i = 0; i < segments; i++)
				{
					Vector3 next = Vector3.Lerp(from, to, (i + 1f) / segments);
					next.y = terrain.GetAbsoluteHeightAt(next) + yOffset;
					Gizmos.DrawLine(last, next);
					last = next;
				}
			}
			else
			{
				Gizmos.DrawLine(from.WithY(yOffset), to.WithY(yOffset));
			}
		}

		/// <summary>
		/// Draws a rectangle that is projected onto the terrain's surface.
		/// </summary>
		public static void DrawTerrainProjectedRectangle(Vector3 from, Vector3 to, float yOffset = 0, int segments = 8, Terrain terrain = null)
		{
			var p00 = from;
			var p01 = new Vector3(from.x, 0, to.z);
			var p10 = new Vector3(to.x, 0, from.z);
			var p11 = to;
			if(!terrain) terrain = Terrain.activeTerrain;
			if(terrain)
			{
				DrawTerrainProjectedLine(p00, p10, yOffset, segments, terrain);
				DrawTerrainProjectedLine(p10, p11, yOffset, segments, terrain);
				DrawTerrainProjectedLine(p11, p01, yOffset, segments, terrain);
				DrawTerrainProjectedLine(p01, p00, yOffset, segments, terrain);
			}
			else
			{
				Gizmos.DrawLine(p00, p10);
				Gizmos.DrawLine(p10, p11);
				Gizmos.DrawLine(p11, p01);
				Gizmos.DrawLine(p01, p00);
			}
		}
		#endregion

		#region Colliders

		/// <summary>
		/// Draws a combined gizmo representing the given BoxCollider.
		/// </summary>
		/// <param name="boxCollider"></param>
		/// <param name="fillAlpha"></param>
		public static void DrawBoxCollider(BoxCollider boxCollider, float fillAlpha)
		{
			if(!boxCollider) return;
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(boxCollider.transform.TransformPoint(boxCollider.center), boxCollider.transform.rotation, boxCollider.transform.lossyScale);
			var size = boxCollider.size;
			DrawCombinedCube(Vector3.zero, size, fillAlpha);
			Gizmos.matrix = lMatrix;
		}

		/// <summary>
		/// Draws a combined gizmo representing the given SphereCollider.
		/// </summary>
		public static void DrawSphereCollider(SphereCollider sphereCollider, float fillAlpha)
		{
			if(!sphereCollider) return;
			var lMatrix = Gizmos.matrix;
			float maxScale = Mathf.Max(sphereCollider.transform.lossyScale.x, Mathf.Max(sphereCollider.transform.lossyScale.y, sphereCollider.transform.lossyScale.z));
			Gizmos.matrix = Matrix4x4.TRS(sphereCollider.transform.TransformPoint(sphereCollider.center), sphereCollider.transform.rotation, Vector3.one * maxScale);
			var radius = sphereCollider.radius;
			DrawCombinedSphere(Vector3.zero, radius, fillAlpha);
			Gizmos.matrix = lMatrix;
		}

		/// <summary>
		/// Draws a combined gizmo representing the given CapsuleCollider.
		/// </summary>
		public static void DrawCapsuleCollider(CapsuleCollider capsuleCollider, float fillAlpha)
		{
			if(!capsuleCollider) return;
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(capsuleCollider.bounds.center, capsuleCollider.transform.rotation, Vector3.one);
			var radius = capsuleCollider.radius;
			var height = capsuleCollider.height;
			Axis axis;
			switch(capsuleCollider.direction)
			{
				case 0:
					axis = Axis.X;
					height *= capsuleCollider.transform.lossyScale.x;
					radius *= Mathf.Max(capsuleCollider.transform.lossyScale.y, capsuleCollider.transform.lossyScale.z);
					break;
				case 1:
					axis = Axis.Y;
					height *= capsuleCollider.transform.lossyScale.y;
					radius *= Mathf.Max(capsuleCollider.transform.lossyScale.x, capsuleCollider.transform.lossyScale.z);
					break;
				case 2:
					axis = Axis.Z;
					height *= capsuleCollider.transform.lossyScale.z;
					radius *= Mathf.Max(capsuleCollider.transform.lossyScale.x, capsuleCollider.transform.lossyScale.y);
					break;
					default:
				axis = Axis.Y;
					break;
			}
			DrawCombinedCapsule(Vector3.zero, axis, radius, height, fillAlpha);
			Gizmos.matrix = lMatrix;
		}

		/// <summary>
		/// Draws a combined gizmo representing the given MeshCollider.
		/// </summary>
		public static void DrawMeshCollider(MeshCollider meshCollider, float fillAlpha)
		{
			if(meshCollider && meshCollider.sharedMesh)
			{
				var lMatrix = Gizmos.matrix;
				Gizmos.matrix = Matrix4x4.TRS(meshCollider.transform.position, meshCollider.transform.rotation, meshCollider.transform.lossyScale);
				Gizmos.DrawWireMesh(meshCollider.sharedMesh);
				using(new GizmoColorScope(Gizmos.color.MultiplyAlpha(fillAlpha)))
				{
					Gizmos.DrawMesh(meshCollider.sharedMesh);
				}
				Gizmos.matrix = lMatrix;
			}
		}

		#endregion

		/// <summary>
		/// Checks if the given position is within a certain distance from the view. Useful for hiding gizmos when too far away. If maxDistance is 0 or less, always returns true.
		/// </summary>
		public static bool CheckDistance(Vector3 pos, float maxDistance)
		{
			if(maxDistance <= 0) return true;
			pos = Gizmos.matrix.MultiplyPoint(pos);
			var cam = Camera.current.transform;
			var diff = cam.position - pos;
			return diff.sqrMagnitude <= maxDistance * maxDistance;
		}

		public static void MakeConstantSize(Vector3 pos, ref float size)
		{
#if UNITY_EDITOR
			var worldPos = Gizmos.matrix.MultiplyPoint(pos);
			size *= UnityEditor.HandleUtility.GetHandleSize(worldPos);
#endif
		}

		private static Quaternion GetAxisRotation(Axis a)
		{
			switch(a)
			{
				case Axis.X: return Quaternion.Euler(-90, -90, 0);
				case Axis.Y: return Quaternion.identity;
				case Axis.Z: return Quaternion.Euler(-90, 0, 180);
				default: throw new System.InvalidOperationException();
			}
		}

		private static Quaternion GetAxisRotation(AxisDirection a)
		{
			switch(a)
			{
				case AxisDirection.XPos: return Quaternion.Euler(-90, -90, 0);
				case AxisDirection.XNeg: return Quaternion.Euler(90, -90, 0);
				case AxisDirection.YPos: return Quaternion.identity;
				case AxisDirection.YNeg: return Quaternion.Euler(180, 0, 0);
				case AxisDirection.ZPos: return Quaternion.Euler(-90, 0, 180);
				case AxisDirection.ZNeg: return Quaternion.Euler(90, 0, 180);
				default: throw new System.InvalidOperationException();
			}
		}
	}
}
