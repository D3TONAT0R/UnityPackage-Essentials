using D3T.Utility;
using UnityEngine;

namespace D3T
{
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

		public virtual bool ShouldDraw(object obj) => CheckMemberCondition(obj, memberName, matches);
	}
}