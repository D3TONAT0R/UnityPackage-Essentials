using System;
using UnityEngine;

namespace D3T
{
	public static class CoroutineExtensions
	{
		#region Action extensions

		/// <summary>
		/// Invokes this action with a delay on a persistent MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeDelayed(this Action a, Delay delay)
		{
			return Coroutines.InvokeDelayed(delay, a);
		}

		/// <summary>
		/// Invokes this action with a delay.
		/// </summary>
		public static Coroutine InvokeDelayed(this Action a, MonoBehaviour owner, Delay delay)
		{
			return Coroutines.InvokeDelayed(owner, delay, a);
		}

		/// <summary>
		/// Invokes this action with a one frame delay on a persistent MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(this Action a)
		{
			return Coroutines.InvokeWithFrameDelay(a);
		}

		/// <summary>
		/// Invokes this action with a one frame delay on the given MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(this Action a, MonoBehaviour owner)
		{
			return Coroutines.InvokeWithFrameDelay(owner, a);
		}

		#endregion
		#region MonoBehaviour extensions

		/// <summary>
		/// Invokes the given action with a specific delay on this MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeDelayed(this MonoBehaviour m, Delay delay, Action action)
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

		#endregion
	}
}
