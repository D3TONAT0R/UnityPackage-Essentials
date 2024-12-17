using UnityEssentials;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEssentialsEditor.TimeTracking
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

				GUILayout.Space(5);
				float total = EditorTimeTracker.users.Sum(kv => kv.Value.GetTotalTime());
				EditorGUILayout.LabelField("All Users Total", ToTimeString(total), EditorStyles.boldLabel);
				float totalActive = EditorTimeTracker.users.Sum(kv => kv.Value.GetTotalTime(TrackedTimeType.AllActive));
				EditorGUILayout.LabelField("All Users Total (active)", ToTimeString(totalActive), EditorStyles.boldLabel);

				foreach(var kv in EditorTimeTracker.users)
				{
					bool isLocalUser = CloudProjectSettings.userId == kv.Key;
					GUI.contentColor = isLocalUser ? new Color(0.3f, 1f, 0.3f) : Color.white;
					GUILayout.Space(10);
					var user = kv.Value;
					EditorGUILayout.LabelField("User: " + kv.Key);
					EditorGUI.indentLevel++;
					EditorGUILayout.LabelField("Total", ToTimeString(user.GetTotalTime()), EditorStyles.boldLabel);
					EditorGUILayout.LabelField("Total (active)", ToTimeString(user.GetTotalTime(TrackedTimeType.AllActive)), EditorStyles.boldLabel);
					GUILayout.Space(5);
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
