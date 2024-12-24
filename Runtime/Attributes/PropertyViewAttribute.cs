using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Add this attribute to a <see cref="PropertyView"/> to define which categories to show.
	/// PropertyViews without this attribute will display all uncategorized properties.
	/// </summary>
	/// <seealso cref="PropertyView"/>
	/// <seealso cref="ShowInInspectorAttribute"/>
	[AttributeUsage(AttributeTargets.Field)]
	public class PropertyViewAttribute : PropertyAttribute
	{
		public string category;
		public bool showTitle = true;

		public PropertyViewAttribute(string category = null)
		{
			this.category = category;
		}
	}
}