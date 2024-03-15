using D3T.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace D3T {

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

		/// <summary>
		/// Runs the given routine on a persistent GameObject, with optional delay.
		/// </summary>
		public static Coroutine Run(IEnumerator routine, float delay = 0, bool delayInRealTime = false)
		{
			return RunnerInstance.StartCoroutine(Execute(routine, delay, delayInRealTime));
		}

		/// <summary>
		/// Runs the given routine on the given MonoBehaviour, with optional delay.
		/// </summary>
		public static Coroutine Run(IEnumerator routine, MonoBehaviour owner, float delay = 0, bool delayInRealTime = false)
		{
			return owner.StartCoroutine(Execute(routine, delay, delayInRealTime));
		}

		/// <summary>
		/// Invokes the given action with a delay.
		/// </summary>
		public static Coroutine InvokeDelayed(Action action, float delay, bool delayInRealTime = false)
		{
			return RunnerInstance.StartCoroutine(Execute(action, delay, delayInRealTime));
		}

		/// <summary>
		/// Invokes the given action with delay on the given MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeDelayed(Action action, MonoBehaviour owner, float delay, bool delayInRealTime = false)
		{
			return owner.StartCoroutine(Execute(action, delay, delayInRealTime));
		}

		/// <summary>
		/// Invokes the given action with a specific frame delay.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(Action action, int frames = 1)
		{
			return RunnerInstance.StartCoroutine(ExecuteWithFrameDelay(action, frames));
		}

		/// <summary>
		/// Invokes the given action with a specific frame delay on the given MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(Action action, MonoBehaviour owner, int frames = 1)
		{
			return owner.StartCoroutine(ExecuteWithFrameDelay(action, frames));
		}

		/// <summary>
		/// Stops the given routine running on the coroutine manager.
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

		private static IEnumerator Execute(IEnumerator routine, float delay, bool realtime)
		{
			if(delay > 0)
			{
				if(realtime) yield return new WaitForSeconds(delay);
				else yield return new WaitForSecondsRealtime(delay);
			}
			yield return routine;
		}

		private static IEnumerator Execute(Action action, float delay, bool realtime)
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
	}
}