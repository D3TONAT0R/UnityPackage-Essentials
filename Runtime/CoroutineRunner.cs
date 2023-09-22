using D3T.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace D3T {

	/// <summary>
	/// Coroutine manager class that allows for object-independent coroutines.
	/// </summary>
	public static class CoroutineRunner
	{

		static MonoInstance _runner;
		static MonoInstance Runner
		{
			get
			{
				if(!_runner)
				{
					_runner = new GameObject("Coroutine Runner").AddComponent<MonoInstance>();
					_runner.gameObject.hideFlags = HideFlags.HideAndDontSave;
				}
				return _runner;
			}
		}

		/// <summary>
		/// Runs the given routine on a persistent GameObject, with optional delay.
		/// </summary>
		public static Coroutine Run(IEnumerator routine, float delay = 0)
		{
			return Runner.StartCoroutine(Execute(routine, delay));
		}

		/// <summary>
		/// Runs the given routine on the given MonoBehaviour, with optional delay.
		/// </summary>
		public static Coroutine Run(IEnumerator routine, MonoBehaviour owner, float delay = 0)
		{
			return owner.StartCoroutine(Execute(routine, delay));
		}

		/// <summary>
		/// Invokes the given action with the given delay.
		/// </summary>
		public static Coroutine InvokeDelayed(Action action, float delay)
		{
			return Runner.StartCoroutine(Execute(action, delay));
		}

		/// <summary>
		/// Invokes the given action with delay on the given MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeDelayed(Action action, MonoBehaviour owner, float delay)
		{
			return owner.StartCoroutine(Execute(action, delay));
		}

		/// <summary>
		/// Invokes the given action with a specific frame delay.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(Action action, int frames = 1)
		{
			return Runner.StartCoroutine(ExecuteFrameDelay(action, frames));
		}

		/// <summary>
		/// Invokes the given action with a specific frame delay on the given MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(Action action, MonoBehaviour owner, int frames = 1)
		{
			return owner.StartCoroutine(ExecuteFrameDelay(action, frames));
		}

		/// <summary>
		/// Stops the given routine running on the persistent singleton instance.
		/// </summary>
		public static void Stop(Coroutine coroutine)
		{
			Runner.StopCoroutine(coroutine);
		}

		/// <summary>
		/// Stops the given routine running on the given MonoBehaviour.
		/// </summary>
		public static void Stop(Coroutine coroutine, MonoBehaviour owner)
		{
			owner.StopCoroutine(coroutine);
		}

		static IEnumerator Execute(IEnumerator routine, float delay)
		{
			if (delay > 0) yield return new WaitForSeconds(delay);
			yield return routine;
		}

		static IEnumerator Execute(Action action, float delay)
		{
			yield return new WaitForSeconds(delay);
			action.Invoke();
		}

		static IEnumerator ExecuteFrameDelay(Action action, int frames)
		{
			for(int i = 0; i < frames; i++) yield return 0;
			action.Invoke();
		}
	}
}