using UnityEngine;

namespace D3T
{
	public class EnabledIfAttribute : PropertyAttribute
	{
		private string memberName;
		private object requiredValue;
		private bool invertCondition;

		public EnabledIfAttribute(string member, object matches, bool invert = false)
		{
			memberName = member;
			requiredValue = matches;
			invertCondition = invert;
		}

		public virtual bool MatchesCondition(object obj)
		{
			var type = obj.GetType();
			var field = type.GetField(memberName);
			if(field != null)
			{
				bool result = field.GetValue(obj).Equals(requiredValue);
				if(invertCondition) result = !result;
				return result;
			}
			var prop = type.GetProperty(memberName);
			if(prop != null)
			{
				bool result = prop.GetValue(obj).Equals(requiredValue);
				if(invertCondition) result = !result;
				return result;
			}
			Debug.LogError("Failed to find field or property: " + memberName);
			return true;
		}
	} 
}
