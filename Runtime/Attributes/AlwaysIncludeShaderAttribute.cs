using System;

namespace D3T.Utility
{
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
