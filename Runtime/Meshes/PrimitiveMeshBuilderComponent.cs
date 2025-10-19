using UnityEngine;

namespace UnityEssentials.Meshes
{
	/// <summary>
	/// Component that generates a convex mesh.
	/// </summary>
	[DisallowMultipleComponent, AddComponentMenu("Mesh/Primitive Mesh Builder")]
	public class PrimitiveMeshBuilderComponent : MeshBuilderComponent
	{
		public enum PrimitiveType
		{
			Cube,
			Sphere,
			Cylinder,
			Capsule,
			Plane,
			Disc,
			Cone
		}

		public PrimitiveType primitiveType = PrimitiveType.Cube;

		[Min(0.001f), ShowIf(nameof(primitiveType), PrimitiveType.Cube)]
		public Vector3 cubeSize = Vector3.one;
		[Min(0.001f), ShowIf(nameof(primitiveType), PrimitiveType.Plane)]
		public Vector2 planeSize = Vector2.one;
		[Min(0.001f), ShowIf(nameof(primitiveType), PrimitiveType.Sphere, PrimitiveType.Cylinder, PrimitiveType.Capsule, PrimitiveType.Disc, PrimitiveType.Cone)]
		public float radius = 0.5f;
		[Min(0), ShowIf(nameof(primitiveType), PrimitiveType.Cylinder, PrimitiveType.Capsule, PrimitiveType.Cone)]
		public float height = 2.0f;

		[Min(1), ShowIf(nameof(primitiveType), PrimitiveType.Plane, PrimitiveType.Disc)]
		public Vector2Int subdivisions = new Vector2Int(10, 10);
		[Min(3), ShowIf(nameof(primitiveType), PrimitiveType.Sphere, PrimitiveType.Cylinder, PrimitiveType.Capsule, PrimitiveType.Disc, PrimitiveType.Cone)]
		public int segments = 32;

		protected override void Generate(ref Mesh mesh)
		{
			MeshBuilder mb = new MeshBuilder();
			switch(primitiveType)
			{
				case PrimitiveType.Cube:
					mb.AddCube(Vector3.zero, cubeSize);
					break;
				case PrimitiveType.Sphere:
					mb.AddSphere(Vector3.zero, radius, segments, segments);
					break;
				case PrimitiveType.Cylinder:
					mb.AddCylinder(Vector3.zero, radius, height, segments);
					break;
				case PrimitiveType.Capsule:
					mb.AddCapsule(Vector3.zero, radius, height, segments, segments);
					break;
				case PrimitiveType.Plane:
					var ll = new Vector3(-planeSize.x * 0.5f, 0, -planeSize.y * 0.5f);
					var lr = new Vector3(planeSize.x * 0.5f, 0, -planeSize.y * 0.5f);
					var ul = new Vector3(-planeSize.x * 0.5f, 0, planeSize.y * 0.5f);
					var ur = new Vector3(planeSize.x * 0.5f, 0, planeSize.y * 0.5f);
					mb.AddPlane(ll, lr, ul, ur, Vector3.up, subdivisions.x, subdivisions.y);
					break;
				case PrimitiveType.Disc:
					mb.AddCircle(Vector3.zero, Vector3.up, radius, segments, subdivisions.x);
					break;
				case PrimitiveType.Cone:
					mb.AddCone(Vector3.zero, AxisDirection.YPos, radius, height, segments);
					break;
			}
			if(mesh) mb.BuildMesh(mesh);
			else mesh = mb.CreateMesh("Primitive");
		}
	}
}
