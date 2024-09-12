using D3T;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace D3TEditor.TimeTracking
{
	internal static class EditorTimeTrackingGUI
	{
		public static bool foldout;

		public static void DrawGUI(string foldoutTitle)
		{
			foldout = EditorGUILayout.Foldout(foldout, foldoutTitle);
			if(foldout)
			{
				EditorGUI.indentLevel++;
				float total = EditorTimeTracker.users.Sum(kv => kv.Value.GetTotalTime());

				GUILayout.Space(5);
				EditorGUILayout.LabelField("Total", ToTimeString(total), EditorStyles.boldLabel);

				foreach(var kv in EditorTimeTracker.users)
				{
					bool isLocalUser = CloudProjectSettings.userId == kv.Key;
					GUI.contentColor = isLocalUser ? new Color(0.3f, 1f, 0.3f) : Color.white;
					GUILayout.Space(5);
					var user = kv.Value;
					EditorGUILayout.LabelField("User: " + kv.Key, ToTimeString(user.GetTotalTime()), EditorStyles.boldLabel);
					EditorGUI.indentLevel++;
					EditorGUILayout.LabelField("Active Editor Time", ToTimeString(user.GetTotalTime(TrackedTimeType.ActiveEditorTime)));
					EditorGUILayout.LabelField("Unfocussed Editor Time", ToTimeString(user.GetTotalTime(TrackedTimeType.UnfocusedEditorTime)));
					EditorGUILayout.LabelField("Playmode Time", ToTimeString(user.GetTotalTime(TrackedTimeType.PlaymodeTime)));
					EditorGUILayout.LabelField("Inactive Time", ToTimeString(user.GetTotalTime(TrackedTimeType.InactiveTime)));
					EditorGUI.indentLevel--;
				}
				EditorGUI.indentLevel--;
			}
			GUI.contentColor = Color.white;
		}

		private static string ToTimeString(float t)
		{
			return ((int)t).ToTimeString(true);
		}
	}
}
