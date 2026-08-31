using UnityEssentials;
using UnityEditor;
using UnityEngine.UIElements;

namespace UnityEssentialsEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
	public class HelpBoxDecoratorDrawer : DecoratorDrawer
	{
		public override VisualElement CreatePropertyGUI()
		{
			var attr = (HelpBoxAttribute)attribute;
			HelpBoxMessageType msgType;
			switch (attr.MessageType)
			{
				case MessageType.Info:
					msgType = HelpBoxMessageType.Info;
					break;
				case MessageType.Warning:
					msgType = HelpBoxMessageType.Warning;
					break;
				case MessageType.Error:
					msgType = HelpBoxMessageType.Error;
					break;
				default:
					msgType = HelpBoxMessageType.None;
					break;
			}
			var helpBox = new HelpBox(attr.message, msgType);
			helpBox.style.marginTop = EditorGUIUtility.standardVerticalSpacing;
			helpBox.style.marginBottom = EditorGUIUtility.standardVerticalSpacing;
			return helpBox;
		}
	}
}