using System;
using UnityEngine;

namespace UnityEssentials
{
	/// <summary>
	/// A scope that measures the time taken for a block of code to execute and logs it to the console.
	/// </summary>
	public class TimingScope : IDisposable
	{
		private readonly System.Diagnostics.Stopwatch stopwatch;
		private readonly string name;

		public TimingScope(string name)
		{
			this.name = name;
			stopwatch = System.Diagnostics.Stopwatch.StartNew();
		}

		public void Dispose()
		{
			stopwatch.Stop();
			var elapsedMS = stopwatch.Elapsed.TotalMilliseconds;
			if (elapsedMS > 1000f)
			{
				Debug.Log($"{name} took {(elapsedMS / 1000f):F3} s");
			}
			else
			{
				Debug.Log($"{name} took {elapsedMS:F3} ms");
			}
		}
	}
}