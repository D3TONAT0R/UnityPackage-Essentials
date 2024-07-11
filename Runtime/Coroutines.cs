using System;
using System.Collections;
using UnityEngine;

namespace D3T
{

	/// <summary>
	/// Coroutine manager that allows for object-independent coroutines.
	/// </summary>
	public static class Coroutines
	{

		private static MonoInstance runnerInstance;
		private static MonoInstance RunnerInstance
		{
			get
			{
				if(!runnerInstance)
				{
					runnerInstance = new GameObject("Coroutine Runner").AddComponent<MonoInstance>();
					runnerInstance.gameObject.hideFlags = HideFlags.HideAndDontSave;
				}
				return runnerInstance;
			}
		}

		#region Coroutine runners

		/// <summary>
		/// Runs the given routine on a persistent GameObject.
		/// </summary>
		public static Coroutine Run(IEnumerator routine)
		{
			return InvokeCoroutine(RunnerInstance, 0, false, routine);
		}

		/// <summary>
		/// Runs the given routine on a persistent GameObject, with the given delay.
		/// </summary>
		public static Coroutine Run(float delay, IEnumerator routine)
		{
			return InvokeCoroutine(RunnerInstance, delay, false, routine);
		}

		/// <summary>
		/// Runs the given routine on a persistent GameObject, with the given delay.
		/// </summary>
		public static Coroutine Run(float delay, bool delayInRealTime, IEnumerator routine)
		{
			return InvokeCoroutine(RunnerInstance, delay, delayInRealTime, routine);
		}

		/// <summary>
		/// Runs the given coroutine on the MonoBehaviour.
		/// </summary>
		public static Coroutine Run(MonoBehaviour owner, IEnumerator routine)
		{
			return InvokeCoroutine(owner, 0, false, routine);
		}

		/// <summary>
		/// Runs the given coroutine on the MonoBehaviour, with the given delay.
		/// </summary>
		public static Coroutine Run(MonoBehaviour owner, float delay, IEnumerator routine)
		{
			return InvokeCoroutine(owner, delay, false, routine);
		}

		/// <summary>
		/// Runs the given coroutine on the MonoBehaviour, with the given delay.
		/// </summary>
		public static Coroutine Run(MonoBehaviour owner, float delay, bool delayInRealTime, IEnumerator routine)
		{
			return InvokeCoroutine(owner, delay, delayInRealTime, routine);
		}

		#endregion

		#region Delayed invocations

		/// <summary>
		/// Invokes the given action with a delay on a persistent GameObject.
		/// </summary>
		public static Coroutine InvokeDelayed(float delay, Action action)
		{
			return InvokeActionDelayed(RunnerInstance, delay, false, action);
		}

		/// <summary>
		/// Invokes the given action with a delay on a persistent GameObject.
		/// </summary>
		public static Coroutine InvokeDelayed(float delay, bool delayInRealTime, Action action)
		{
			return InvokeActionDelayed(RunnerInstance, delay, delayInRealTime, action);
		}

		/// <summary>
		/// Invokes the given action with a one frame delay on a persistent GameObject.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(Action action)
		{
			return InvokeActionFrameDelayed(RunnerInstance, 1, action);
		}

		/// <summary>
		/// Invokes the given action with a specific frame delay on a persistent GameObject.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(int frames, Action action)
		{
			return InvokeActionFrameDelayed(RunnerInstance, frames, action);
		}

		/// <summary>
		/// Invokes the given action with a delay on the given MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeDelayed(MonoBehaviour owner, float delay, Action action)
		{
			return InvokeActionDelayed(owner, delay, false, action);
		}

		/// <summary>
		/// Invokes the given action with a delay on the given MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeDelayed(MonoBehaviour owner, float delay, bool delayInRealTime, Action action)
		{
			return InvokeActionDelayed(owner, delay, delayInRealTime, action);
		}

		/// <summary>
		/// Invokes the given action with a one frame delay on the given MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(MonoBehaviour owner, Action action)
		{
			return InvokeActionFrameDelayed(owner, 1, action);
		}

		/// <summary>
		/// Invokes the given action with a specific frame delay on the given MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(MonoBehaviour owner, int frames, Action action)
		{
			return InvokeActionFrameDelayed(owner, frames, action);
		}

		#endregion

		#region Stop methods

		/// <summary>
		/// Stops the given routine running on the persistent coroutine manager.
		/// </summary>
		public static void Stop(Coroutine coroutine)
		{
			RunnerInstance.StopCoroutine(coroutine);
		}

		/// <summary>
		/// Stops the given routine running on the given MonoBehaviour.
		/// </summary>
		public static void Stop(Coroutine coroutine, MonoBehaviour owner)
		{
			owner.StopCoroutine(coroutine);
		}

		#endregion

		#region Private methods

		private static Coroutine InvokeCoroutine(MonoBehaviour owner, float delay, bool delayInRealTime, IEnumerator routine)
		{
			if(owner == null) throw new NullReferenceException();
			return owner.StartCoroutine(ExecuteCoroutine(routine, delay, delayInRealTime));
		}

		private static Coroutine InvokeActionDelayed(MonoBehaviour owner, float delay, bool delayInRealTime, Action action)
		{
			if(owner == null) throw new NullReferenceException();
			return owner.StartCoroutine(ExecuteAction(action, delay, delayInRealTime));
		}

		private static Coroutine InvokeActionFrameDelayed(MonoBehaviour owner, int frames, Action action)
		{
			if(owner == null) throw new NullReferenceException();
			return owner.StartCoroutine(ExecuteWithFrameDelay(action, frames));
		}

		private static IEnumerator ExecuteCoroutine(IEnumerator routine, float delay, bool realtime)
		{
			if(delay > 0)
			{
				if(realtime) yield return new WaitForSeconds(delay);
				else yield return new WaitForSecondsRealtime(delay);
			}
			yield return routine;
		}

		private static IEnumerator ExecuteAction(Action action, float delay, bool realtime)
		{
			if(delay > 0)
			{
				if(realtime) yield return new WaitForSeconds(delay);
				else yield return new WaitForSecondsRealtime(delay);
			}
			action.Invoke();
		}

		private static IEnumerator ExecuteWithFrameDelay(Action action, int frames)
		{
			for(int i = 0; i < frames; i++) yield return 0;
			action.Invoke();
		}

		#endregion
	}
}