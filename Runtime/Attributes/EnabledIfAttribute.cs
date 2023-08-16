using UnityEngine;

namespace D3T
{
	public class EnabledIfAttribute : PropertyModifierAttribute
	{
		private readonly string memberName;
		private readonly object[] matches;

		public EnabledIfAttribute(string member, object matches)
		{
			memberName = member;
			this.matches = new object[] { matches };
		}

		public EnabledIfAttribute(string member, params object[] matches)
		{
			memberName = member;
			this.matches = matches;
		}

		public virtual bool IsEnabled(object obj) => CheckMemberCondition(obj, memberName, matches);
	} 
}
