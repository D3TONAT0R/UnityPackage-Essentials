using System;

namespace UnityEssentials.Collections
{
	/// <summary>
	/// Enables polymorphic support for a UnityDictionary.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class PolymorphicDictionaryAttribute : Attribute
	{
		public Type[] specificTypes = null;

		public PolymorphicDictionaryAttribute()
		{

		}

		public PolymorphicDictionaryAttribute(params Type[] specificTypes)
		{
			this.specificTypes = specificTypes;
		}
	} 
}
