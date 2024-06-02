using System;
using UnityEngine;

namespace D3T
{
	public static class CoroutineExtensions
	{
		/// <summary>
		/// Invokes the given action with a specific delay on this MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeDelayed(this MonoBehaviour m, float delay, Action action)
		{
			return Coroutines.InvokeDelayed(m, delay, action);
		}

		/// <summary>
		/// Invokes the given action with a one frame delay on this MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(this MonoBehaviour m, Action action)
		{
			return Coroutines.InvokeWithFrameDelay(m, action);
		}

		/// <summary>
		/// Invokes the given action with a specific frame delay on this MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(this MonoBehaviour m, int frames, Action action)
		{
			return Coroutines.InvokeWithFrameDelay(m, action);
		}

		/// <summary>
		/// Invokes this action with a delay.
		/// </summary>
		public static Coroutine InvokeDelayed(this Action a, float delay, bool delayInRealTime = false)
		{
			return Coroutines.InvokeDelayed(delay, delayInRealTime, a);
		}

		/// <summary>
		/// Invokes this action with a delay.
		/// </summary>
		public static Coroutine InvokeDelayed(this Action a, MonoBehaviour owner, float delay, bool delayInRealTime = false)
		{
			return Coroutines.InvokeDelayed(owner, delay, delayInRealTime, a);
		}

		/// <summary>
		/// Invokes this action with a one frame delay.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(this Action a)
		{
			return Coroutines.InvokeWithFrameDelay(a);
		}


		/// <summary>
		/// Invokes this action with a one frame delay.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(this Action a, MonoBehaviour owner)
		{
			return Coroutines.InvokeWithFrameDelay(owner, a);
		}

		/// <summary>
		/// Invokes this action with a given frame delay.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(this Action a, int frames)
		{
			return Coroutines.InvokeWithFrameDelay(frames, a);
		}


		/// <summary>
		/// Invokes this action with a given frame delay.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(this Action a, MonoBehaviour owner, int frames)
		{
			return Coroutines.InvokeWithFrameDelay(owner, frames, a);
		}
	}
}
