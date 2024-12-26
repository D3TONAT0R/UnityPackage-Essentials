using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Makes an inspector field read-only.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ReadOnlyAttribute : PropertyAttribute
	{
		public bool drawAsFields;

		public ReadOnlyAttribute() : this(false)
		{
			order = -20;
		}

		public ReadOnlyAttribute(bool drawAsFields)
		{
			this.drawAsFields = drawAsFields;
		}
	}
}
