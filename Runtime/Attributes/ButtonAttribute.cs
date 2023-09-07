using System;
using UnityEngine;

namespace D3T
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class ButtonAttribute : PropertyAttribute
	{

		public string buttonText;
		public string methodName;
		public bool below;
		public bool enabledOutsidePlayMode;

		public ButtonAttribute(string buttonText, string methodName, bool below = false, bool enabledOutsidePlayMode = false)
		{
			this.buttonText = buttonText;
			this.methodName = methodName;
			this.below = below;
			this.enabledOutsidePlayMode = enabledOutsidePlayMode;
		}
	}
}