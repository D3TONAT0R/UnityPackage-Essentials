using System;

namespace D3T.Collections
{
	/// <summary>
	/// Enables polymorphic support for a <see cref=""/> or <see cref="PolymorphicList"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class PolymorphicAttribute : Attribute
	{
		public Type[] specificTypes = null;

		public PolymorphicAttribute()
		{

		}

		public PolymorphicAttribute(params Type[] specificTypes)
		{
			this.specificTypes = specificTypes;
		}
	} 
}
