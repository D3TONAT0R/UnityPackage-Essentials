using System;

namespace UnityEssentials.Utility
{
	/// <summary>
	/// Ensures that a given shader is always included in builds.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
	public class AlwaysIncludeShaderAttribute : Attribute
	{
		public string shaderName;

		public AlwaysIncludeShaderAttribute(string shaderName)
		{
			this.shaderName = shaderName;
		}
	}
}
