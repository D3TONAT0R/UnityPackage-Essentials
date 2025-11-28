using System;

namespace UnityEssentials.Collections
{
	/// <summary>
	/// Add this attribute to a <see cref="StackComponent"/> class to set the group this class should be placed in within context menus.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class AddElementMenuAttribute : Attribute
	{
		public readonly string group;
		public readonly string customName;

		public AddElementMenuAttribute(string group, string customName = null)
		{
			this.group = group;
			this.customName = customName;
		}
	}
}