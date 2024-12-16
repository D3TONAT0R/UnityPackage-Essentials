using System;

namespace UnityEssentials
{
	/// <summary>
	/// Add this attribute to a static method to have it called right before the Update period.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class PreUpdateAttribute : Attribute { }

	/// <summary>
	/// Add this attribute to a static method to have it called during the Update period.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class UpdateAttribute : Attribute { }

	/// <summary>
	/// Add this attribute to a static method to have it called right before the LateUpdate period.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class PreLateUpdateAttribute : Attribute { }

	/// <summary>
	/// Add this attribute to a static method to have it called during the LateUpdate period.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class LateUpdateAttribute : Attribute { }

	/// <summary>
	/// Add this attribute to a static method to have it called after the LateUpdate period.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class PostLateUpdateAttribute : Attribute { }


	/// <summary>
	/// Add this attribute to a static method to have it called during the FixedUpdate period.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class FixedUpdateAttribute : Attribute { }

	/// <summary>
	/// Add this attribute to a static method to have it called after the FixedUpdate period.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class LateFixedUpdateAttribute : Attribute { }

	/// <summary>
	/// Add this attribute to a static method to have it called after the FixedUpdate and LateFixedUpdate period.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class PostFixedUpdateAttribute : Attribute { }



	/// <summary>
	/// Add this attribute to a static method to have it called exactly once during the update period.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class UpdateOnceAttribute : Attribute { }

	/// <summary>
	/// Add this attribute to a static method to have it called exactly once during the fixed update period.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class FixedUpdateOnceAttribute : Attribute { }


	/// <summary>
	/// Add this attribute to a static method to have it called during the OnGUI period.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class OnGUIAttribute : Attribute { }

	/// <summary>
	/// Add this attribute to a static method to draw gizmos in the scene view (runtime only).
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class OnDrawGizmosRuntimeAttribute : Attribute { }
}
