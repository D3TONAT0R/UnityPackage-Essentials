using System;

namespace UnityEssentials
{
	/// <summary>
	/// Makes an inspector field visible only if a given field or property matches a specific value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ShowIfAttribute : PropertyModifierAttribute
	{
		private readonly string memberName;
		private readonly object[] matches;

		public ShowIfAttribute(string member, object matches)
		{
			memberName = member;
			this.matches = new object[] { matches };
		}

		public ShowIfAttribute(string member, params object[] matches)
		{
			memberName = member;
			this.matches = matches;
		}

		public virtual bool ShouldDraw(object parentObject) => CheckMemberCondition(parentObject, memberName, matches);
	}
}