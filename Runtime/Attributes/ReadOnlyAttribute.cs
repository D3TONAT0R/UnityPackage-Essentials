using System;
using UnityEngine;

namespace D3T
{
	/// <summary>
	/// Makes an inspector field read-only.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ReadOnlyAttribute : PropertyAttribute
	{
		public bool asFields;

		public ReadOnlyAttribute() : this(false)
		{

		}

		public ReadOnlyAttribute(bool asFields)
		{
			this.asFields = asFields;
		}
	}
}
