using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace D3TEditor
{
	public static class MenuUtility
	{
		private static MethodInfo removeMenuItemMethod;

		public static void RemoveMenuItem(string menuPath)
		{
			if(removeMenuItemMethod == null)
			{
				removeMenuItemMethod = typeof(Menu).GetMethod("RemoveMenuItem", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			}
			removeMenuItemMethod.Invoke(null, new object[] { menuPath });
		}
	} 
}
