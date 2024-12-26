using System;

namespace UnityEssentials
{
	/// <summary>
	/// Makes an inspector field editable only if a given field or property matches a specific value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class EnabledIfAttribute : PropertyModifierAttribute
	{
		private readonly string memberName;
		private readonly object[] matches;

		public EnabledIfAttribute(string memberName, params object[] matches)
		{
			this.memberName = memberName;
			this.matches = matches;
		}

		public virtual bool IsEnabled(object obj) => CheckMemberCondition(obj, memberName, matches);
	}
}
