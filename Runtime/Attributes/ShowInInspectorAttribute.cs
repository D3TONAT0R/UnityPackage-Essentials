using System;

namespace UnityEssentials
{
	/// <summary>
	/// Add this attribute to a property or non-serialized field to make it visible in the inspector.
	/// </summary>
	public class ShowInInspectorAttribute : Attribute
	{
		public string customLabel;
		public bool editableAtRuntime;

		public ShowInInspectorAttribute(string customLabel = null, bool editableAtRuntime = true)
		{
			this.customLabel = customLabel;
			this.editableAtRuntime = editableAtRuntime;
		}
	} 
}