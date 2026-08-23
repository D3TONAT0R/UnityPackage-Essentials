using System;

namespace UnityEssentials
{
	/// <summary>
	/// The type of help box to display.
	/// </summary>
	public enum HelpBoxType
	{
		None,
		Info,
		Warning,
		Error
	}

	/// <summary>
	/// Add this attribute to a field to draw a help box above it in the inspector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class HelpBoxAttribute : DecoratorAttribute
	{	

		public HelpBoxType type;
		public string message;

#if UNITY_EDITOR
		public UnityEditor.MessageType MessageType
		{
			get
			{
				if(type == HelpBoxType.Error) return UnityEditor.MessageType.Error;
				else if(type == HelpBoxType.Warning) return UnityEditor.MessageType.Warning;
				else if(type == HelpBoxType.Info) return UnityEditor.MessageType.Info;
				else return UnityEditor.MessageType.None;
			}
		}
#endif

		public HelpBoxAttribute(string message, HelpBoxType type = HelpBoxType.Info)
		{
			this.message = message;
			this.type = type;
		}
	}
}
