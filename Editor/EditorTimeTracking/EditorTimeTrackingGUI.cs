using D3T;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.TimeTracking
{
	public static class EditorTimeTrackingGUI
	{
		public static bool foldout;

		public static void DrawGUI(string foldoutTitle)
		{
			foldout = EditorGUILayout.Foldout(foldout, foldoutTitle);
			if(foldout)
			{
				EditorGUI.indentLevel++;
				float total = EditorTimeTracker.times.Sum(kv => kv.Value.CombinedTime);

				GUILayout.Space(10);
				EditorGUILayout.LabelField("Total", ToTimeString(total), EditorStyles.boldLabel);

				foreach(var kv in EditorTimeTracker.times)
				{
					GUILayout.Space(10);
					var times = kv.Value;
					EditorGUILayout.LabelField("User: " + kv.Key, ToTimeString(times.CombinedTime), EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					EditorGUILayout.LabelField("Active Editor Time", ToTimeString(times.activeEditTime));
					EditorGUILayout.LabelField("Unfocussed Editor Time", ToTimeString(times.unfocussedEditTime));
					EditorGUILayout.LabelField("Playmode Time", ToTimeString(times.playmodeTime));
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
		}

		private static string ToTimeString(float t)
		{
			return ((int)t).ConvertToTimeString(true);
		}
	}
}
