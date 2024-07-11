using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace D3T
{
	/// <summary>
	/// Base class for all custom proeprty attributes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public abstract class PropertyModifierAttribute : PropertyAttribute
	{

		protected static bool CheckMemberCondition(object target, string memberName, object[] matches)
		{
			var m = ReflectionUtility.FindMemberInType(target.GetType(), memberName);
			if(m != null)
			{
				var memberValue = ReflectionUtility.GetMemberValue(m, target);
				//Fallback to a simple "not null" or true check
				if(matches == null || matches.Length == 0)
				{
					if(memberValue == null)
					{
						return false;
					}
					else if(memberValue is Object uObj)
					{
						return uObj != null;
					}
					else
					{
						return memberValue.Equals(true);
					}
				}
				foreach(var r in matches)
				{
					if(r == null)
					{
						if(memberValue == null)
						{
							return true;
						}
					}
					if(memberValue.Equals(r))
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
