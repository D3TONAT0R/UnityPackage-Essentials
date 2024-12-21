using System;

namespace UnityEssentials
{
	/// <summary>
	/// Hides inspector field if a given field or property matches a specific value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class HideIfAttribute : ShowIfAttribute
	{
		private readonly string memberName;
		private readonly object[] matches;

		public HideIfAttribute(string member, object matches) : base(member, matches)
		{
		}

		public HideIfAttribute(string member, params object[] matches) : base(member, matches)
		{
		}

		public override bool ShouldDraw(object parentObject) => !base.ShouldDraw(parentObject);
	}
}