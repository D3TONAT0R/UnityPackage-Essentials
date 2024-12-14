using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// Suspends the coroutine execution for a specified amount of frames.
	/// </summary>
	public class WaitForFrames : CustomYieldInstruction
	{
		private int framesLeft;

		public override bool keepWaiting
		{
			get
			{
				framesLeft--;
				return framesLeft >= 0;
			}
		}

		public WaitForFrames(int frames)
		{
			framesLeft = frames;
		}
	}
}