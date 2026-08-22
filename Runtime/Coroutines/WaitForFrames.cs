using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Suspends the coroutine execution for a specified amount of frames.
	/// </summary>
	public class WaitForFrames : CustomYieldInstruction
	{
		private int waitUntilFrame;

		public override bool keepWaiting
		{
			get
			{
				return Time.frameCount < waitUntilFrame;
			}
		}

		public WaitForFrames(int frames)
		{
			if(frames <= 0) Debug.LogWarning("Attempted to wait for 0 or less frames. Minimum delay is 1 frame.");
			waitUntilFrame = Time.frameCount + frames;
		}
	}
}