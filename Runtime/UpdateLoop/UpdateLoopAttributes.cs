using System;

namespace D3T
{
	[AttributeUsage(AttributeTargets.Method)]
	public class PreUpdateAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Method)]
	public class UpdateAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Method)]
	public class PreLateUpdateAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Method)]
	public class LateUpdateAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Method)]
	public class PostLateUpdateAttribute : Attribute { }



	[AttributeUsage(AttributeTargets.Method)]
	public class FixedUpdateAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Method)]
	public class PostFixedUpdateAttribute : Attribute { }



	[AttributeUsage(AttributeTargets.Method)]
	public class UpdateOnceAttribute : Attribute { }



	[AttributeUsage(AttributeTargets.Method)]
	public class OnGUIAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Method)]
	public class OnDrawGizmosRuntimeAttribute : Attribute { }
}
