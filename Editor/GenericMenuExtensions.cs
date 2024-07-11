using System;
using System.Collections;
using System.Linq;
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

		public static void InsertItem(this GenericMenu m, int index, string content, bool enabled, bool on, GenericMenu.MenuFunction func)
		{
			InsertItem(m, index, new GUIContent(content), enabled, on, func);
		}

		public static void InsertItem(this GenericMenu m, int index, GUIContent content, bool enabled, bool on, GenericMenu.MenuFunction func)
		{
			GetMenuItemList(m).Insert(index, CreateMenuItem(content, false, enabled, on, func));
		}

		public static void InsertSeparator(this GenericMenu m, int index)
		{
			GUIContent path;
			if(index > 0)
			{
				var split = GetContentAtIndex(m, index - 1).text.Split('/').ToList();
				split.RemoveAt(split.Count - 1);
				path = new GUIContent(string.Join("/", split));
			}
			else
			{
				path = new GUIContent("");
			}
			GetMenuItemList(m).Insert(index, CreateMenuItem(path, true, false, false, null));
		}

		public static bool RemoveItem(this GenericMenu m, string menuName)
		{
			var index = GetIndexOf(m, menuName);
			if(index >= 0) GetMenuItemList(m).RemoveAt(index);
			return index >= 0;
		}

		public static void RemoveItemAt(this GenericMenu m, int index)
		{
			GetMenuItemList(m).RemoveAt(index);
		}

		public static int GetIndexOf(this GenericMenu m, string menuName)
		{
			var list = GetMenuItemList(m);
			for(int i = 0; i < list.Count; i++)
			{
				var content = GetContentAtIndex(m, i);
				if(content.text == menuName) return i;
			}
			return -1;
		}

		public static GUIContent GetContentAtIndex(this GenericMenu m, int index)
		{
			var item = GetMenuItemList(m)[index];
			return item.GetType().GetField("content").GetValue(item) as GUIContent;
		}

		private static IList GetMenuItemList(this GenericMenu m)
		{
			return (IList)typeof(GenericMenu).GetField("m_MenuItems", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(m);
		}

		private static object CreateMenuItem(GUIContent content, bool separator, bool enabled, bool on, GenericMenu.MenuFunction func)
		{
			var type = typeof(GenericMenu).GetNestedType("MenuItem", System.Reflection.BindingFlags.NonPublic);
			return Activator.CreateInstance(type, new object[] { content, separator, on, enabled ? func : null });
		}
	}
}
