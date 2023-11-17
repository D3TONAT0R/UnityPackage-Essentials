using System;
using UnityEditor;

namespace UnityEssentials
{
	public enum HelpBoxType { None, Info, Warning, Error }

	/// <summary>
	/// Add this attribute to a field to draw a help box above it in the inspector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class HelpBoxAttribute : DecoratorAttribute {

		public HelpBoxType type;
		public string message;

#if UNITY_EDITOR
		public MessageType MessageType {
			get {
				if(type == HelpBoxType.Error) return MessageType.Error;
				else if(type == HelpBoxType.Warning) return MessageType.Warning;
				else if(type == HelpBoxType.Info) return MessageType.Info;
				else return MessageType.None;
			}
		}
#endif

		public HelpBoxAttribute(string message, HelpBoxType type = HelpBoxType.Info) {
			this.message = message;
			this.type = type;
		}
	} 
}
