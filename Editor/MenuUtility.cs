using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	public static class MenuUtility
	{
		private static MethodInfo addMenuItemMethod;
		private static MethodInfo removeMenuItemMethod;

		public static void RemoveMenuItem(string menuPath)
		{
			removeMenuItemMethod ??= typeof(Menu).GetMethod("RemoveMenuItem", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			removeMenuItemMethod.Invoke(null, new object[] { menuPath });
		}

		public static void AddMenuItem(string name, string shortcut, int priority, Action execute, bool isChecked = false, Func<bool> validate = null)
		{
			addMenuItemMethod ??= typeof(Menu).GetMethod("AddMenuItem", BindingFlags.NonPublic | BindingFlags.Static);
			addMenuItemMethod.Invoke(null, new object[] { name, shortcut, isChecked, priority, execute, validate });
		}

		public static void ReorganizeAssetMenu()
		{
			//TODO: remove unity menus & add them manually again
		}
	} 
}
