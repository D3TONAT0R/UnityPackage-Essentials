using UnityEngine;

namespace D3T
{
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
