using System.Text;
using UnityEngine;

namespace D3T
{
	public static class Extensions
	{

		#region Transforms


		#endregion
		#region Numerics

		public static bool IsBetween(this float f, float min, float max) => f >= min && f <= max;

		public static bool IsBetweenExcluding(this float f, float min, float max) => f > min && f < max;

		public static bool IsBetweenIncluding(this int i, int min, int max) => i >= min && i <= max;

		public static bool IsBetweenExcluding(this int i, int min, int max) => i > min && i < max;

		public static float Round(this float f, int decNumbers)
		{
			int exp = (int)Mathf.Pow(10, decNumbers);
			return decNumbers <= 0 ? Mathf.RoundToInt(f) : Mathf.Round(f * exp) / exp;
		}

		public static float RoundTo(this float f, float rounding) => Mathf.Round(f * rounding) / rounding;

		public static float Abs(this float f) => Mathf.Abs(f);

		public static int Abs(this int i) => Mathf.Abs(i);

		public static int Sign(this float f) => f > 0 ? 1 : f < 0 ? -1 : 0;
		public static int Sign(this int i) => i > 0 ? 1 : i < 0 ? -1 : 0;

		public static float ClampAngle(this float f)
		{
			while(f > 180) f -= 360;
			while(f < -180) f += 360;
			return f;
		}

		#endregion

		/// <summary>
		/// Returns true if this <see cref="LayerMask"/> contains the given layer.
		/// </summary>
		public static bool ContainsLayer(this LayerMask m, int layer)
		{
			return m == (m | (1 << layer));
		}

		/// <summary>
		/// Draws this GUIStyle during the repaint phase.
		/// </summary>
		public static void DrawOnRepaint(this GUIStyle s, Rect r)
		{
			if (Event.current.type == EventType.Repaint) s.Draw(r, false, false, false, false);
		}

		/// <summary>
		/// Draws this GUIStyle during the repaint phase.
		/// </summary>
		public static void DrawOnRepaint(this GUIStyle s, Rect r, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
		{
			if (Event.current.type == EventType.Repaint) s.Draw(r, isHover, isActive, on, hasKeyboardFocus);
		}

		/// <summary>
		/// Returns a formatted time string from this integer as seconds.
		/// </summary>
		public static string ToTimeString(this int t, bool hours)
		{
			t = Mathf.Abs(t);
			string sec = (t % 60).ToString("D2");
			string min = (hours ? t / 60 % 60 : t / 60).ToString("D2");
			if(hours)
			{
				string hrs = (t / 3600).ToString("D2");
				return $"{hrs}:{min}:{sec}";
			}
			else
			{
				return $"{min}:{sec}";
			}
		}

		/// <summary>
		/// Fills all pixels of this texture with a given color.
		/// </summary>
		public static void FillWithColor(this Texture2D tex, Color c)
		{
			Color32[] cols = new Color32[tex.width * tex.height];
			for (int i = 0; i < cols.Length; i++) cols[i] = c;
			tex.SetPixels32(cols);
			tex.Apply();
		}

		/// <summary>
		/// Returns a copy of this texture which is read/write enabled.
		/// </summary>
		public static Texture2D GetReadableCopy(this Texture2D tex)
		{
			if (tex.isReadable)
			{
				Debug.LogWarning($"Texture '{tex.name}' is already readable.");
				return tex;
			}
			Texture2D copy = new Texture2D(tex.width, tex.height, tex.format, tex.mipmapCount, false)
			{
				wrapMode = tex.wrapMode,
				filterMode = tex.filterMode
			};
			Graphics.CopyTexture(tex, copy);
			copy.Apply();
			return copy;
		}

		/// <summary>
		/// Adds a custom error message to this exception.
		/// </summary>
		public static MessagedException AddMessage(this System.Exception e, string message)
		{
			return new MessagedException(message, e);
		}

		/// <summary>
		/// Logs this exception with a custom error message.
		/// </summary>
		public static void LogException(this System.Exception e, string message = null, Object context = null)
		{
			Debug.LogException(e.AddMessage(message), context);
		}

		/// <summary>
		/// (EDITOR ONLY) Invokes the given action with a slight delay. Useful for avoiding "SendMessage" related warnings during an OnValidate event.
		/// </summary>
		public static void EditorDelayCall(this MonoBehaviour m, System.Action onValidateAction)
		{
#if UNITY_EDITOR
			bool wasPlaying = Application.isPlaying;
			UnityEditor.EditorApplication.delayCall += _OnValidate;

			void _OnValidate()
			{
				UnityEditor.EditorApplication.delayCall -= _OnValidate;
				if(Application.isPlaying == wasPlaying)
				{
					onValidateAction();
				}
			}
#endif
		}
	}
}
