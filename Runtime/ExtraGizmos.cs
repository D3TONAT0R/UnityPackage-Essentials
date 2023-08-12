﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace D3T
{
	public class ExtraGizmos
	{
		internal static Mesh disc;
		internal static Mesh cylinder;

		static ExtraGizmos()
		{
			var builder = new MeshBuilder();
			builder.AddDisc(Vector3.zero, Vector3.up, 1f, 32);
			//builder.AddDisc(Vector3.zero, Vector3.down, 1f, 32);
			disc = builder.CreateMesh();

			builder.Clear();
			builder.AddCylinder(Vector3.zero, Quaternion.identity, 1, 1, 16);
			cylinder = builder.CreateMesh();
		}

		public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius, int segments = 32)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.LookRotation(normal), Vector3.one * radius);
			var pts = MeshBuilder.GetCirclePoints(segments, 1);
			for(int i = 0; i < segments - 1; i++)
			{
				Gizmos.DrawLine(pts[i].XYV(0), pts[i + 1].XYV(0));
			}
			Gizmos.DrawLine(pts[31].XYV(0), pts[0].XYV(0));
			Gizmos.matrix = lastMatrix;
		}

		public static void DrawDisc(Vector3 center, Vector3 normal, float radius)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.LookRotation(normal), Vector3.one * radius);
			Gizmos.DrawMesh(disc);
			Gizmos.matrix = lastMatrix;
		}

		public static void DrawArc(Vector3 center, Vector3 up, Vector3 forward, float radius, float fromDegrees, float toDegrees, int segments = 16)
		{
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.LookRotation(forward, up), Vector3.one);
			Vector3[] points = new Vector3[segments + 1];
			for(int i = 0; i <= segments; i++)
			{
				float a = Mathf.Lerp(fromDegrees, toDegrees, i / (float)segments) * Mathf.Deg2Rad;
				points[i] = new Vector3(Mathf.Sin(a), 0, Mathf.Cos(a)) * radius;
			}
			DrawPath(points);
			Gizmos.matrix = lMatrix;
		}

		public static void DrawCylinder(Vector3 center, Quaternion rotation, float radius, float height)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation, new Vector3(radius, height, radius));
			Gizmos.DrawMesh(cylinder);
			Gizmos.matrix = lastMatrix;
		}

		public static void DrawWireCylinder(Vector3 center, Quaternion rotation, float radius, float height)
		{
			var lastMatrix = Gizmos.matrix;
			rotation *= Quaternion.Euler(90, 0, 0);
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation, Vector3.one);
			float h2 = height * 0.5f;
			DrawWireDisc(Vector3.back * h2, Vector3.back, radius);
			DrawWireDisc(Vector3.forward * h2, Vector3.forward, radius);
			DrawLineFrom(new Vector3(-radius, 0, -h2), Vector3.forward, height);
			DrawLineFrom(new Vector3(radius, 0, -h2), Vector3.forward, height);
			DrawLineFrom(new Vector3(0, -radius, -h2), Vector3.forward, height);
			DrawLineFrom(new Vector3(0, radius, -h2), Vector3.forward, height);
			Gizmos.matrix = lastMatrix;
		}

		public static void DrawLineFrom(Vector3 point, Vector3 direction, float length)
		{
			direction = direction.normalized;
			Gizmos.DrawLine(point, point + direction * length);
		}

		public static void DrawAxes(Vector3 position, Quaternion rotation, float size)
		{
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(position, rotation, Vector3.one);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(Vector3.zero, Vector3.right * size);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(Vector3.zero, Vector3.up * size);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(Vector3.zero, Vector3.forward * size);
			Gizmos.matrix = lMatrix;
		}

		public static void DrawCrosshairs(Vector3 point, float radius)
		{
			Gizmos.DrawLine(point + Vector3.left * radius, point + Vector3.right * radius);
			Gizmos.DrawLine(point + Vector3.down * radius, point + Vector3.up * radius);
			Gizmos.DrawLine(point + Vector3.back * radius, point + Vector3.forward * radius);
		}

		public static void DrawLocus(Vector3 point, float radius)
		{
			float r2 = radius * 2f;
			Gizmos.DrawLine(point + Vector3.left * r2, point + Vector3.right * r2);
			Gizmos.DrawLine(point + Vector3.down * r2, point + Vector3.up * r2);
			Gizmos.DrawLine(point + Vector3.back * r2, point + Vector3.forward * r2);
			Gizmos.DrawWireSphere(point, radius);
		}

		public static void DrawRadiusRectangle(Vector3 center, Vector3 up, Vector2 size, float radius)
		{
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.LookRotation(up), Vector3.one);
			var p0 = new Vector3(-size.x, -size.y, 0);
			var p1 = new Vector3(size.x, -size.y, 0);
			var p2 = new Vector3(-size.x, size.y, 0);
			var p3 = new Vector3(size.x, size.y, 0);
			Gizmos.DrawLine(p0 + Vector3.down * radius, p1 + Vector3.down * radius);
			Gizmos.DrawLine(p1 + Vector3.right * radius, p3 + Vector3.right * radius);
			Gizmos.DrawLine(p0 + Vector3.left * radius, p2 + Vector3.left * radius);
			Gizmos.DrawLine(p2 + Vector3.up * radius, p3 + Vector3.up * radius);
			DrawArc(p0, Vector3.up, Vector3.up, radius, 180, 270);
			DrawArc(p1, Vector3.up, Vector3.up, radius, 90, 180);
			DrawArc(p2, Vector3.up, Vector3.up, radius, -90, 0);
			DrawArc(p3, Vector3.up, Vector3.up, radius, 0, 90);
			Gizmos.matrix = lMatrix;
		}

		public static void DrawWireRadiusCube(Vector3 center, Vector3 size, float radius)
		{
			size = size.Abs();
			DrawRadiusRectangle(center + new Vector3(-size.x, 0, 0), Vector3.right, size.ZY(), radius);
			DrawRadiusRectangle(center + new Vector3(size.x, 0, 0), Vector3.right, size.ZY(), radius);
			DrawRadiusRectangle(center + new Vector3(0, -size.y, 0), Vector3.up, size.XZ(), radius);
			DrawRadiusRectangle(center + new Vector3(0, size.y, 0), Vector3.up, size.XZ(), radius);
			DrawRadiusRectangle(center + new Vector3(0, 0, -size.z), Vector3.forward, size.XY(), radius);
			DrawRadiusRectangle(center + new Vector3(0, 0, size.z), Vector3.forward, size.XY(), radius);
		}

		public static void DrawPath(Vector3[] points)
		{
			for(int i = 0; i < points.Length - 1; i++)
			{
				Gizmos.DrawLine(points[i], points[i + 1]);
			}
		}

		public static void DrawText(Vector3 position, string text, Color? color = null)
		{
#if UNITY_EDITOR
			var lastColor = GUI.color;
			if(color.HasValue) GUI.color = color.Value;
			UnityEditor.Handles.Label(position, text);
			GUI.color = lastColor;
#endif
		}

		#region Terrain Projected Shapes
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
				DrawWireDisc(center.WithY(yOffset), Vector3.up, radius, segments);
			}
		}

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
	}
}
