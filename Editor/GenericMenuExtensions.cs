using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	public static class GenericMenuExtensions
	{
		public static void AddItem(this GenericMenu m, string content, bool enabled, bool on, GenericMenu.MenuFunction func)
		{
			if(enabled) m.AddItem(new GUIContent(content), on, func);
			else m.AddDisabledItem(new GUIContent(content), on);
		}

		public static void AddItem(this GenericMenu m, GUIContent content, bool enabled, bool on, GenericMenu.MenuFunction func)
		{
			if(enabled) m.AddItem(content, on, func);
			else m.AddDisabledItem(content, on);
		}
	} 
}
