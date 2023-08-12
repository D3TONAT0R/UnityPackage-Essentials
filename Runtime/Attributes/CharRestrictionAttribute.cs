using System;
using UnityEngine;

namespace D3T
{

	public enum CharRestrictionType
	{
	    None,
		Underscored,
	    LowercaseUnderscored,
	    UppercaseUnderscored,
	    DotPath
	}

	/// <summary>
	/// Add this attribute to a string field to restrict which characters are allowed in the string.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class CharRestrictionAttribute : PropertyAttribute {

		public const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
		public const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public const string digits = "0123456789";
		public const string dashes = "_-";

		//public CharRestrictionType type;
		public readonly string allowedChars = null;
		public readonly char replacementChar = '_';
		public bool? forcedCase = null;

		public CharRestrictionAttribute(CharRestrictionType type, string additionalAllowedCharacters = "") {
			//this.type = type;
			if(type == CharRestrictionType.Underscored)
			{
				allowedChars = lowercaseChars + uppercaseChars + digits + dashes + additionalAllowedCharacters;
				forcedCase = null;
			}
			else if(type == CharRestrictionType.LowercaseUnderscored)
			{
				allowedChars = lowercaseChars + digits + dashes + additionalAllowedCharacters;
				forcedCase = false;
			}
			else if(type == CharRestrictionType.UppercaseUnderscored)
			{
				allowedChars = uppercaseChars + digits + dashes + additionalAllowedCharacters;
				forcedCase = true;
			}
			else if(type == CharRestrictionType.DotPath)
			{
				allowedChars = lowercaseChars + digits + "." + additionalAllowedCharacters;
				forcedCase = false;
				replacementChar = '.';
			}
		}
	} 
}
