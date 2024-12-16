using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Struct representing a delay for use in <see cref="Coroutines"/>
	/// </summary>
	[System.Serializable]
	public struct Delay
	{
		public enum DelayType : byte
		{
			Frames = 0,
			Seconds = 1,
			SecondsRealtime = 2,
		}

		/// <summary>
		/// The type of delay this represents.
		/// </summary>
		public DelayType type;
		/// <summary>
		/// The time of the delay, in either frames or seconds.
		/// </summary>
		public float time;

		public bool IsZero => type == DelayType.Frames ? time < 1 : time <= 0;

		public int FrameCount => type == DelayType.Frames ? Mathf.FloorToInt(time) : -1;

		public static Delay None => new Delay(DelayType.Frames, 0);
		public static Delay OneFrame => new Delay(DelayType.Frames, 1);

		public Delay(DelayType type, float time)
		{
			this.type = type;
			this.time = time;
		}

		public static Delay Frames(int frames)
		{
			return new Delay(DelayType.Frames, frames);
		}

		public static Delay Seconds(float time)
		{
			return new Delay(DelayType.Seconds, time);
		}

		public static Delay SecondsRealtime(float time)
		{
			return new Delay(DelayType.SecondsRealtime, time);
		}

		public override string ToString()
		{
			return $"({type}: {time})";
		}
	}
}
