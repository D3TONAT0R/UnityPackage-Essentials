using System.Collections.Generic;
using UnityEngine;

namespace D3T
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

		public abstract void BuildMesh(Mesh mesh, bool recalculateTangents = true);

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

		public Vector3 TransformPoint(Vector3 point)
		{
			return currentMatrix.MultiplyPoint(point);
		}

		public Vector3 TransformVector(Vector3 vector)
		{
			return currentMatrix.MultiplyVector(vector);
		}

		public void TransformPoint(ref Vector3 point)
		{
			point = currentMatrix.MultiplyPoint(point);
		}

		public void TransformVector(ref Vector3 vector)
		{
			vector = currentMatrix.MultiplyVector(vector);
		}

		#endregion

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

		public virtual void AddTransformedVertex(Vector3 pos)
		{
			AddVertex(TransformPoint(pos));
		}
	}
}
