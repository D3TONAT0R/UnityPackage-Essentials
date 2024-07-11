using UnityEngine;

namespace D3T.PlayerLoop
{
	internal class UpdateLoopScriptInstance : MonoBehaviour
	{
		internal static UpdateLoopScriptInstance instance;

		internal static void Init()
		{
			if(instance) return;
			var go = new GameObject("UpdateLoopScriptInstance")
			{
				hideFlags = HideFlags.HideInInspector | HideFlags.DontSave
			};
			if(Application.isPlaying)
			{
				DontDestroyOnLoad(go);
			}
			instance = go.AddComponent<UpdateLoopScriptInstance>();
		}

		private void OnApplicationQuit()
		{
			DestroyInstance(this);
		}

		internal static void Cleanup()
		{
			if(instance)
			{
				DestroyInstance(instance);
			}
		}

		private static void DestroyInstance(UpdateLoopScriptInstance inst)
		{
			if(inst == null) return;
			if(Application.isPlaying) Destroy(inst.gameObject);
			else DestroyImmediate(inst.gameObject);
		}

		private void OnGUI()
		{
			if(CheckInstance())
			{
				UpdateLoop.InvokeOnGUI();
			}
		}

		private void OnDrawGizmos()
		{
			if(CheckInstance())
			{
				UpdateLoop.InvokeOnDrawGizmosRuntime();
			}
		}

		private bool CheckInstance()
		{
			if(instance == null)
			{
				Debug.LogError("Missing UpdateLoopScriptInstance detected.");
				DestroyInstance(this);
				return false;
			}
			else if(instance != this)
			{
				Debug.LogError("Duplicate UpdateLoopScriptInstance detected.");
				DestroyInstance(instance);
				return false;
			}
			return true;
		}
	}
}
