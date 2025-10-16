using System;
using System.Collections;
using UnityEngine;

namespace UnityEssentials
{

	/// <summary>
	/// Coroutine manager that allows for object-independent coroutines.
	/// </summary>
	public static class Coroutines
	{
		private static DummyMonoBehaviour RunnerInstance
		{
			get
			{
				if(!runnerInstance)
				{
					runnerInstance = new GameObject("Coroutine Runner").AddComponent<DummyMonoBehaviour>();
					runnerInstance.gameObject.hideFlags = HideFlags.HideAndDontSave;
				}
				return runnerInstance;
			}
		}
		private static DummyMonoBehaviour runnerInstance;

		#region Coroutine runners

		/// <summary>
		/// Runs the given routine on a persistent MonoBehaviour.
		/// </summary>
		public static Coroutine Run(IEnumerator routine)
		{
			return InvokeCoroutine(RunnerInstance, Delay.None, routine);
		}

		/// <summary>
		/// Runs the given routine on a persistent MonoBehaviour, with the given delay.
		/// </summary>
		public static Coroutine Run(Delay delay, IEnumerator routine)
		{
			return InvokeCoroutine(RunnerInstance, delay, routine);
		}

		/// <summary>
		/// Runs the given coroutine on the MonoBehaviour.
		/// </summary>
		public static Coroutine Run(MonoBehaviour owner, IEnumerator routine)
		{
			return InvokeCoroutine(owner, Delay.None, routine);
		}

		/// <summary>
		/// Runs the given coroutine on the MonoBehaviour, with the given delay.
		/// </summary>
		public static Coroutine Run(MonoBehaviour owner, Delay delay, IEnumerator routine)
		{
			return InvokeCoroutine(owner, delay, routine);
		}

		#endregion

		#region Delayed invocations

		/// <summary>
		/// Invokes the given action with a delay on a persistent MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeDelayed(Delay delay, Action action)
		{
			return InvokeActionDelayed(RunnerInstance, delay, action);
		}

		/// <summary>
		/// Invokes the given action with a one frame delay on a persistent MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(Action action)
		{
			return InvokeActionDelayed(RunnerInstance, Delay.OneFrame, action);
		}

		/// <summary>
		/// Invokes the given action with a delay on the given MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeDelayed(MonoBehaviour owner, Delay delay, Action action)
		{
			return InvokeActionDelayed(owner, delay, action);
		}

		/// <summary>
		/// Invokes the given action with a one frame delay on the given MonoBehaviour.
		/// </summary>
		public static Coroutine InvokeWithFrameDelay(MonoBehaviour owner, Action action)
		{
			return InvokeActionDelayed(owner, Delay.OneFrame, action);
		}

		#endregion

		#region Stop methods

		/// <summary>
		/// Stops the given routine running on the persistent MonoBehaviour.
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

		private static Coroutine InvokeCoroutine(MonoBehaviour owner, Delay delay, IEnumerator routine)
		{
			if(owner == null) throw new NullReferenceException("Coroutine owner object must no be null.");
			return owner.StartCoroutine(ExecuteCoroutine(routine, delay));
		}

		private static Coroutine InvokeActionDelayed(MonoBehaviour owner, Delay delay, Action action)
		{
			if(owner == null) throw new NullReferenceException("Coroutine owner object must no be null.");
			return owner.StartCoroutine(ExecuteAction(action, delay));
		}

		private static IEnumerator ExecuteCoroutine(IEnumerator routine, Delay delay)
		{
			yield return PerformDelay(delay);
			yield return routine;
		}

		private static IEnumerator ExecuteAction(Action action, Delay delay)
		{
			yield return PerformDelay(delay);
			action.Invoke();
		}

		private static IEnumerator PerformDelay(Delay delay)
		{
			if(delay.IsZero) yield break;
			switch(delay.type)
			{
				case Delay.DelayType.Seconds:
					yield return new WaitForSeconds(delay.time);
					break;
				case Delay.DelayType.SecondsRealtime:
					yield return new WaitForSecondsRealtime(delay.time);
					break;
				case Delay.DelayType.Frames:
					yield return new WaitForFrames(delay.FrameCount);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		#endregion
	}
}