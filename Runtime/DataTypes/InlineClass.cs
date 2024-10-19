namespace UnityEssentials
{

	/// <summary>
	/// Base class for subclasses whose members should be drawn on the same line in the inspector.
	/// </summary>
	public abstract class InlineClass
	{
		/// <summary>
		/// Override this property to control whether or not to apply inlined layout to this object.
		/// </summary>
		public virtual bool InheritInlinedLayout => true;
	}
}
