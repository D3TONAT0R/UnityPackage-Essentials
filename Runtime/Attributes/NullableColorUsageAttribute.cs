using UnityEngine;

namespace D3T
{
	public class NullableColorUsageAttribute : PropertyAttribute
	{
		public bool showAlpha = true;
		public bool hdr = false;

		public NullableColorUsageAttribute(bool showAlpha)
		{
			this.showAlpha = showAlpha;
		}

		public NullableColorUsageAttribute(bool showAlpha, bool hdr)
		{
			this.showAlpha = showAlpha;
			this.hdr = hdr;
		}
	} 
}
