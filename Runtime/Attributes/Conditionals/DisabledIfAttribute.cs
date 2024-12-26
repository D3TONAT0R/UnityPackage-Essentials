using System;

namespace UnityEssentials
{
	/// <summary>
	/// Makes an inspector field non-editable if a given field or property matches a specific value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class DisabledIfAttribute : EnabledIfAttribute
	{
		public DisabledIfAttribute(string memberName, params object[] matches) : base(memberName, matches)
		{

		}

		public override bool IsEnabled(object obj) => !base.IsEnabled(obj);
	}
}
