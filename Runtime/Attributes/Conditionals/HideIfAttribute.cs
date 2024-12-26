using System;

namespace UnityEssentials
{
	/// <summary>
	/// Hides an inspector field if a given field or property matches a specific value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class HideIfAttribute : ShowIfAttribute
	{
		public HideIfAttribute(string memberName, params object[] matches) : base(memberName, matches)
		{

		}

		public override bool ShouldDraw(object parentObject) => !base.ShouldDraw(parentObject);
	}
}