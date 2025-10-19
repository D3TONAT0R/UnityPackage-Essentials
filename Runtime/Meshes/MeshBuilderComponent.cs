using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEssentials.Meshes
{
	/// <summary>
	/// Base class for mesh generating components.
	/// </summary>
	public abstract class MeshBuilderComponent : MonoBehaviour
	{
		public enum TargetComponents
		{
			None,
			MeshCollider,
			MeshFilter,
			Both
		}

		public TargetComponents applyTo = TargetComponents.Both;

		protected Mesh generatedMesh;

		public Mesh GeneratedMesh => generatedMesh;

		protected virtual void Awake()
		{
			Rebuild();
			AssignToComponents();
		}

		public void Rebuild()
		{
			Generate(ref generatedMesh);
		}

		protected abstract void Generate(ref Mesh mesh);

		public void AssignToComponents()
		{
			if((applyTo == TargetComponents.MeshCollider || applyTo == TargetComponents.Both) && TryGetComponent<MeshCollider>(out var meshCollider))
			{
				meshCollider.sharedMesh = GeneratedMesh;
			}
			if((applyTo == TargetComponents.MeshFilter || applyTo == TargetComponents.Both) && TryGetComponent<MeshFilter>(out var meshFilter))
			{
				meshFilter.sharedMesh = GeneratedMesh;
			}
		}

		public virtual void Validate()
		{
			if(this == null) return;
			Rebuild();
			AssignToComponents();
		}

		protected virtual void Reset()
		{
			Validate();
		}

		protected virtual void OnValidate()
		{
			this.EditorDelayCall(Validate);
		}

		protected virtual void OnDrawGizmosSelected()
		{
			if(GeneratedMesh)
			{
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.color = Colors.darkBlue.WithAlpha(0.3f);
				Gizmos.DrawWireMesh(GeneratedMesh);
			}
		}
	}
}
