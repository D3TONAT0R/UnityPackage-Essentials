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
		public string category;

		public ShowInInspectorAttribute(string customLabel = null, string category = null, bool editableAtRuntime = true)
		{
			this.customLabel = customLabel;
			this.category = category;
			this.editableAtRuntime = editableAtRuntime;
		}
	} 
}