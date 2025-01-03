﻿using UnityEssentials.Meshes;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Provides additional gizmo drawing methods on top of Unity's own Gizmos.
	/// </summary>
	public class ExtraGizmos
	{
		internal static Mesh discMesh;
		internal static Mesh cylinderMesh;
		internal static Mesh capsuleCenterMesh;
		internal static Mesh capsuleCapMesh;
		internal static Mesh coneMesh;

		internal static GUIStyle labelStyle;

		private static List<Vector3> circlePointCache = new List<Vector3>();

		static ExtraGizmos()
		{
			var builder = new MeshBuilder();
			builder.AddCircle(Vector3.zero, Vector3.up, 1f, 32);
			//builder.AddCircle(Matrix4x4.identity, 1f, 32);
			discMesh = builder.CreateMesh();

			builder.Clear();
			builder.AddCylinder(Vector3.zero, 1, 1, 16, true);
			cylinderMesh = builder.CreateMesh();

			builder.Clear();
			builder.AddCylinder(Vector3.zero, 1, 1, 16, false);
			capsuleCenterMesh = builder.CreateMesh();
			builder.Clear();
			builder.AddHemisphere(Vector3.zero, 1, 0.5f, 16, 16);
			capsuleCapMesh = builder.CreateMesh();

			builder.Clear();
			builder.AddCone(Vector3.zero, AxisDirection.YPos, 1f, 1f, 16, true);
			coneMesh = builder.CreateMesh();
		}

		/// <summary>
		/// Draws a wireframe circle gizmo.
		/// </summary>
		public static void DrawWireCircle(Vector3 center, Vector3 normal, float radius, int segments = 64)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.LookRotation(normal), Vector3.one * radius);
			MeshBuilderBase.GetCirclePoints(circlePointCache, segments, 1);
			for(int i = 0; i < segments - 1; i++)
			{
				Gizmos.DrawLine(circlePointCache[i], circlePointCache[i + 1]);
			}
			Gizmos.DrawLine(circlePointCache[segments - 1], circlePointCache[0]);
			Gizmos.matrix = lastMatrix;
		}

		/// <summary>
		/// Draws a solid circle gizmo.
		/// </summary>
		public static void DrawCircle(Vector3 center, Vector3 normal, float radius)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.LookRotation(normal) * Quaternion.Euler(90, 0, 0), Vector3.one * radius);
			Gizmos.DrawMesh(discMesh);
			Gizmos.matrix = lastMatrix;
		}

		/// <summary>
		/// Draws an arc between the given angles.
		/// </summary>
		public static void DrawArc(Vector3 center, Vector3 up, Vector3 forward, float radius, float fromDegrees, float toDegrees, bool edges = false, int segments = 32)
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
			if(edges)
			{
				Gizmos.DrawLine(Vector3.zero, points[0]);
				Gizmos.DrawLine(Vector3.zero, points[points.Length - 1]);
			}
			Gizmos.matrix = lMatrix;
		}

		/// <summary>
		/// Draws a wireframe cylinder gizmo.
		/// </summary>
		public static void DrawWireCylinder(Vector3 center, Quaternion rotation, float radius, float height)
		{
			var lastMatrix = Gizmos.matrix;
			rotation *= Quaternion.Euler(90, 0, 0);
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation, Vector3.one);
			float h2 = height * 0.5f;
			DrawWireCircle(Vector3.back * h2, Vector3.back, radius);
			DrawWireCircle(Vector3.forward * h2, Vector3.forward, radius);
			DrawLineFrom(new Vector3(-radius, 0, -h2), Vector3.forward, height);
			DrawLineFrom(new Vector3(radius, 0, -h2), Vector3.forward, height);
			DrawLineFrom(new Vector3(0, -radius, -h2), Vector3.forward, height);
			DrawLineFrom(new Vector3(0, radius, -h2), Vector3.forward, height);
			Gizmos.matrix = lastMatrix;
		}

		/// <summary>
		/// Draws a wireframe cylinder gizmo.
		/// </summary>
		public static void DrawWireCylinder(Vector3 center, Axis axis, float radius, float height)
		{
			var rotation = GetAxisRotation(axis);
			DrawWireCylinder(center, rotation, radius, height);
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
		/// Draws a wireframe capsule gizmo.
		/// </summary>
		public static void DrawWireCapsule(Vector3 center, Quaternion rotation, float radius, float height)
		{
			height = Mathf.Max(height - radius * 2, 0);
			var lastMatrix = Gizmos.matrix;
			rotation *= Quaternion.Euler(90, 0, 0);
			Gizmos.matrix *= Matrix4x4.TRS(center, rotation, Vector3.one);
			float h2 = height * 0.5f;
			DrawWireCircle(Vector3.back * h2, Vector3.back, radius);
			DrawArc(Vector3.back * h2, Vector3.up, Vector3.back, radius, -90, 90);
			DrawArc(Vector3.back * h2, Vector3.right, Vector3.back, radius, -90, 90);
			DrawWireCircle(Vector3.forward * h2, Vector3.forward, radius);
			DrawArc(Vector3.forward * h2, Vector3.up, Vector3.forward, radius, -90, 90);
			DrawArc(Vector3.forward * h2, Vector3.right, Vector3.forward, radius, -90, 90);
			if(height > 0)
			{
				DrawLineFrom(new Vector3(-radius, 0, -h2), Vector3.forward, height);
				DrawLineFrom(new Vector3(radius, 0, -h2), Vector3.forward, height);
				DrawLineFrom(new Vector3(0, -radius, -h2), Vector3.forward, height);
				DrawLineFrom(new Vector3(0, radius, -h2), Vector3.forward, height);
			}
			Gizmos.matrix = lastMatrix;
		}

		/// <summary>
		/// Draws a wireframe capsule gizmo.
		/// </summary>
		public static void DrawWireCapsule(Vector3 center, Axis axis, float radius, float height)
		{
			var rotation = GetAxisRotation(axis);
			DrawWireCapsule(center, rotation, radius, height);
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
		/// Draws a wireframe cone gizmo.
		/// </summary>
		public static void DrawWireCone(Vector3 center, Quaternion rotation, float radius, float height, int circleSegments = 64)
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
		}

		/// <summary>
		/// Draws a wireframe cone gizmo.
		/// </summary>
		public static void DrawWireCone(Vector3 center, AxisDirection direction, float radius, float height, int circleSegments = 64)
		{
			var rotation = GetAxisRotation(direction);
			DrawWireCone(center, rotation, radius, height, circleSegments);
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
		public static void DrawAxes(Vector3 position, Quaternion rotation, float size)
		{
			var lColor = Gizmos.color;
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(position, rotation, Vector3.one);
			Gizmos.color = Color.red;
			DrawArrow(Vector3.zero, Vector3.right, size);
			Gizmos.color = Color.green;
			DrawArrow(Vector3.zero, Vector3.up, size);
			Gizmos.color = Color.blue;
			DrawArrow(Vector3.zero, Vector3.forward, size);
			Gizmos.color = lColor;
			Gizmos.matrix = lMatrix;
		}

		/// <summary>
		/// Draws a point gizmo.
		/// </summary>
		public static void DrawPoint(Vector3 point, float radius, bool centerSphere = true, bool constantSize = false)
		{
			if(constantSize) MakeConstantSize(point, ref radius);
			Gizmos.DrawLine(point + Vector3.left * radius, point + Vector3.right * radius);
			Gizmos.DrawLine(point + Vector3.down * radius, point + Vector3.up * radius);
			Gizmos.DrawLine(point + Vector3.back * radius, point + Vector3.forward * radius);
			if(centerSphere) Gizmos.DrawWireSphere(point, radius * 0.5f);
		}

		/// <summary>
		/// Draws an arrow gizmo.
		/// </summary>
		public static void DrawArrow(Vector3 origin, Quaternion rotation, float length, float? fixedHeadLength = null)
		{
			var lastMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(origin, rotation, Vector3.one);
			Gizmos.DrawLine(Vector3.zero, Vector3.forward * length);
			float headLength = fixedHeadLength ?? length * 0.25f;
			float headStart = length - headLength;
			DrawWireCone(Vector3.forward * headStart, AxisDirection.ZPos, headLength * 0.25f, headLength, 16);
			Gizmos.matrix = lastMatrix;
		}

		/// <summary>
		/// Draws an arrow gizmo.
		/// </summary>
		public static void DrawArrow(Vector3 origin, Vector3 direction, float length, float? fixedHeadLength = null)
		{
			DrawArrow(origin, Quaternion.LookRotation(direction), length, fixedHeadLength);
		}

		/// <summary>
		/// Draws an arrow gizmo.
		/// </summary>
		public static void DrawArrow(Vector3 origin, AxisDirection direction, float length, float? fixedHeadLength = null)
		{
			DrawArrow(origin, Quaternion.LookRotation(direction.GetDirectionVector()), length, fixedHeadLength);
		}

		/// <summary>
		/// Draws a rectangle gizmo.
		/// </summary>
		public static void DrawRectangle(Vector3 center, Vector3 up, Vector2 size)
		{
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.LookRotation(up), Vector3.one);
			size *= 0.5f;
			var p0 = new Vector3(-size.x, -size.y, 0);
			var p1 = new Vector3(size.x, -size.y, 0);
			var p2 = new Vector3(-size.x, size.y, 0);
			var p3 = new Vector3(size.x, size.y, 0);
			Gizmos.DrawLine(p0, p1);
			Gizmos.DrawLine(p1, p3);
			Gizmos.DrawLine(p0, p2);
			Gizmos.DrawLine(p2, p3);
			Gizmos.matrix = lMatrix;
		}

		/// <summary>
		/// Draws a rounded rectangle gizmo.
		/// </summary>
		public static void DrawRadiusRectangle(Vector3 center, Vector3 up, Vector2 size, float radius, bool grow = false)
		{
			if(radius <= 0)
			{
				DrawRectangle(center, up, size);
				return;
			}
			var lMatrix = Gizmos.matrix;
			Gizmos.matrix *= Matrix4x4.TRS(center, Quaternion.LookRotation(up), Vector3.one);
			if(!grow) size -= 2 * radius * Vector2.one;
			size *= 0.5f;
			var p0 = new Vector3(-size.x, -size.y, 0);
			var p1 = new Vector3(size.x, -size.y, 0);
			var p2 = new Vector3(-size.x, size.y, 0);
			var p3 = new Vector3(size.x, size.y, 0);
			Gizmos.DrawLine(p0 + Vector3.down * radius, p1 + Vector3.down * radius);
			Gizmos.DrawLine(p1 + Vector3.right * radius, p3 + Vector3.right * radius);
			Gizmos.DrawLine(p0 + Vector3.left * radius, p2 + Vector3.left * radius);
			Gizmos.DrawLine(p2 + Vector3.up * radius, p3 + Vector3.up * radius);
			DrawArc(p0, Vector3.up, Vector3.up, radius, 180, 270, false, 8);
			DrawArc(p1, Vector3.up, Vector3.up, radius, 90, 180, false, 8);
			DrawArc(p2, Vector3.up, Vector3.up, radius, -90, 0, false, 8);
			DrawArc(p3, Vector3.up, Vector3.up, radius, 0, 90, false, 8);
			Gizmos.matrix = lMatrix;
		}


		/// <summary>
		/// Draws a rouded cube gizmo.
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
			DrawRadiusRectangle(center + new Vector3(-size.x * 0.5f + inset, 0, 0), Vector3.right, size.ZY(), radius, grow);
			DrawRadiusRectangle(center + new Vector3(size.x * 0.5f - inset, 0, 0), Vector3.right, size.ZY(), radius, grow);
			DrawRadiusRectangle(center + new Vector3(0, -size.y * 0.5f + inset, 0), Vector3.up, size.XZ(), radius, grow);
			DrawRadiusRectangle(center + new Vector3(0, size.y * 0.5f - inset, 0), Vector3.up, size.XZ(), radius, grow);
			DrawRadiusRectangle(center + new Vector3(0, 0, -size.z * 0.5f + inset), Vector3.forward, size.XY(), radius, grow);
			DrawRadiusRectangle(center + new Vector3(0, 0, size.z * 0.5f - inset), Vector3.forward, size.XY(), radius, grow);
		}

		/// <summary>
		/// Draws a path between the given points.
		/// </summary>
		public static void DrawPath(Vector3[] points)
		{
			for(int i = 0; i < points.Length - 1; i++)
			{
				Gizmos.DrawLine(points[i], points[i + 1]);
			}
		}

		/// <summary>
		/// Draws a GUI text at the given point in the scene.
		/// </summary>
		public static void DrawText(Vector3 position, string text, Color? color = null, TextAnchor anchor = TextAnchor.UpperLeft, float offset = 0, Vector2? offsetUnits = null, bool shadow = false)
		{
#if UNITY_EDITOR
			if(labelStyle == null)
			{
				labelStyle = new GUIStyle(GUI.skin.label);
			}
			labelStyle.alignment = anchor;

			var lastColor = GUI.color;
			if(color.HasValue) GUI.color = color.Value;
			else GUI.color = Gizmos.color;
			Label(Gizmos.matrix.MultiplyPoint(position), new GUIContent(text), labelStyle, offset, offsetUnits ?? Vector2.zero, shadow);
			GUI.color = lastColor;
#endif
		}


		/// <summary>
		/// Draws a GUI text box at the given point in the scene.
		/// </summary>
		public static void DrawTextBox(Vector3 position, string text, Color? color = null, TextAnchor anchor = TextAnchor.UpperLeft, float offset = 0, Vector2? offsetUnits = null, GUIStyle style = null)
		{
#if UNITY_EDITOR
			var boxStyle = style ?? GUI.skin.box;
			var lastColor = GUI.color;
			if(color.HasValue) GUI.color = color.Value;
			else GUI.color = Gizmos.color;
			Box(Gizmos.matrix.MultiplyPoint(position), new GUIContent(text), boxStyle, anchor, offset, offsetUnits ?? Vector2.zero);
			GUI.color = lastColor;
#endif
		}

#if UNITY_EDITOR
		private static void Label(Vector3 position, GUIContent content, GUIStyle style, float offset, Vector2 offsetUnits, bool shadow)
		{
			Vector3 vector = UnityEditor.HandleUtility.WorldToGUIPointWithDepth(position);
			if(!(vector.z < 0f))
			{
				UnityEditor.Handles.BeginGUI();
				var rect = WorldPointToSizedRect(position, content, style, style.alignment, offset, offsetUnits).Round();
				if(shadow)
				{
					var position2 = rect;
					position2.x++;
					position2.y++;
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
