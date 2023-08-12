using D3T.Utility;
using UnityEngine;

namespace D3T
{
	public class ShowIfAttribute : PropertyAttribute
	{
		private readonly string memberName;
		private readonly object[] requiredValues;

		public ShowIfAttribute(string member, object matches)
		{
			memberName = member;
			requiredValues = new object[] { matches };
		}

		public ShowIfAttribute(string member, params object[] matches)
		{
			memberName = member;
			requiredValues = matches;
		}

		public virtual bool ShouldDraw(object obj)
		{
			var m = ReflectionUtility.FindMemberInType(obj.GetType(), memberName);
			if(m != null)
			{
				foreach(var r in requiredValues)
				{
					if(ReflectionUtility.GetMemberValue(m, obj).Equals(r))
					{
						return true;
					}
				}
				return false;
			}
			else
			{
				Debug.LogError("Failed to find field or property: " + memberName);
				return true;
			}
		}
	}
}