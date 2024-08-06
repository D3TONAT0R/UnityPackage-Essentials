using System.Collections.Generic;
using UnityEngine;

namespace D3T.Meshes
{
	public abstract class MeshBuilderBase
	{
		public class MatrixScope : System.IDisposable
		{
			private MeshBuilderBase builder;

			public MatrixScope(MeshBuilderBase mb)
			{
				builder = mb;
				mb.PushMatrix();
			}

			public void Dispose() => builder.PopMatrix();
		}

		public List<Vector3> verts = new List<Vector3>();
		public List<Color32> vertexColors = new List<Color32>();

		public Color32? currentVertexColor = null;

		protected List<Matrix4x4> matrixStack = new List<Matrix4x4>();
		protected Matrix4x4 currentMatrix = Matrix4x4.identity;

		protected static List<Vector3> tempVertexCache = new List<Vector3>();

		/// <summary>
		/// Builds the current mesh data into the given mesh object.
		/// </summary>
		public abstract void BuildMesh(Mesh mesh);

		/// <summary>
		/// Creates a new mesh with the current mesh data.
		/// </summary>
		public virtual Mesh CreateMesh(string name = null)
		{
			var mesh = new Mesh();
			if(name != null) mesh.name = name;
			BuildMesh(mesh);
			return mesh;
		}

		#region Matrix related methods

		public void PushMatrix()
		{
			matrixStack.Add(currentMatrix);
		}

		public void PopMatrix()
		{
			if(matrixStack.Count > 0)
			{
				matrixStack.RemoveAt(matrixStack.Count - 1);
				currentMatrix = matrixStack.Count > 0 ? matrixStack[matrixStack.Count - 1] : Matrix4x4.identity;
			}
		}

		public MatrixScope PushMatrixScope()
		{
			return new MatrixScope(this);
		}

		public void SetMatrix(Matrix4x4 matrix)
		{
			currentMatrix = matrix;
		}

		public void ApplyMatrix(Matrix4x4 matrix)
		{
			currentMatrix *= matrix;
		}

		public void ResetMatrix(bool resetStack = true)
		{
			currentMatrix = Matrix4x4.identity;
			if(resetStack) matrixStack.Clear();
		}

		public virtual void TransformPoint(ref Vector3 point)
		{
			point = currentMatrix.MultiplyPoint(point);
		}

		public virtual void TransformVector(ref Vector3 vector)
		{
			vector = currentMatrix.MultiplyVector(vector);
		}

		public Vector3 TransformPoint(Vector3 point)
		{
			TransformPoint(ref point);
			return point;
		}

		public Vector3 TransformVector(Vector3 vector)
		{
			TransformVector(ref vector);
			return vector;
		}

		#endregion

		/// <summary>
		/// Clears all current mesh data.
		/// </summary>
		public abstract void Clear();

		/// <summary>
		/// Adds a vertex to the mesh.
		/// </summary>
		public virtual void AddVertex(Vector3 pos)
		{
			verts.Add(pos);
			if(currentVertexColor.HasValue)
			{
				while(vertexColors.Count < verts.Count)
				{
					vertexColors.Add(currentVertexColor.Value);
				}
			}
		}

		/// <summary>
		/// Adds a vertex to the mesh, transformed by the current transformation matrix.
		/// </summary>
		/// <param name="pos"></param>
		public virtual void AddTransformedVertex(Vector3 pos)
		{
			AddVertex(TransformPoint(pos));
		}

		/// <summary>
		/// Calculates points for a circle with the given point count and radius.
		/// </summary>
		public static void GetCirclePoints(List<Vector3> results, int pointCount, float radius = 1f)
		{
			results.Clear();
			for(int i = 0; i < pointCount; i++)
			{
				float angleRad = i / (float)pointCount * Mathf.PI * 2f;
				results.Add(new Vector2(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius));
			}
		}

		public static Quaternion GetAxisRotation(Axis a)
		{
			switch(a)
			{
				case Axis.X: return Quaternion.Euler(-90, -90, 0);
				case Axis.Y: return Quaternion.identity;
				case Axis.Z: return Quaternion.Euler(-90, 0, 180);
				default: throw new System.InvalidOperationException();
			}
		}

		public static Quaternion GetAxisRotation(AxisDirection a)
		{
			switch(a)
			{
				case AxisDirection.XNeg: return Quaternion.Euler(-90, 90, 0);
				case AxisDirection.XPos: return Quaternion.Euler(-90, -90, 0);
				case AxisDirection.YNeg: return Quaternion.Euler(180, 0, 0);
				case AxisDirection.YPos: return Quaternion.identity;
				case AxisDirection.ZNeg: return Quaternion.Euler(-90, 0, 0);
				case AxisDirection.ZPos: return Quaternion.Euler(-90, 0, 180);
				default: throw new System.InvalidOperationException();
			}
		}
	}
}
