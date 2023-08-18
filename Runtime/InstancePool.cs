using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace D3T
{
	/// <summary>
	/// A pool of GameObjects with a specific component attached.
	/// </summary>
	/// <typeparam name="T">The component attached to each instance of this pool.</typeparam>
	public class InstancePool<T> where T : Component
	{
		private List<T> inactivePool;
		private List<T> activePool;
		private Dictionary<T, float> lastActivationTimes;
		private T[] iterationCache;

		public Scene? TargetScene { get; private set; }
		public int maxPoolSize;
		public Func<T> instantiator;
		public Action<T> activator = null;
		public Action<T> deactivator = null;
		public bool replaceLast = false;
		public bool dontDestroyOnLoad = false;
		public float? maxActiveTime = null;

		private float lastUpdateTime;

		public int InactiveInstanceCount => inactivePool.Count;
		public int ActiveInstanceCount => activePool.Count;
		public int TotalInstanceCount => InactiveInstanceCount + ActiveInstanceCount;

		public InstancePool(Scene? scene, int maxPoolSize, Func<T> instantiator = null, Action<T> activator = null, Action<T> deactivator = null, bool dontDestroyOnLoad = false, float? maxActiveTime = null)
		{
			if(scene.HasValue && !scene.Value.IsValid()) throw new ArgumentException("Scene is invalid");
			inactivePool = new List<T>();
			activePool = new List<T>();
			lastActivationTimes = new Dictionary<T, float>();
			TargetScene = scene;
			this.maxPoolSize = maxPoolSize;
			this.instantiator = instantiator ?? DefaultInstantiator;
			this.activator = activator;
			this.deactivator = deactivator;
			this.dontDestroyOnLoad = dontDestroyOnLoad;
			this.maxActiveTime = maxActiveTime;
			iterationCache = new T[maxPoolSize];
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			Application.quitting += OnApplicationQuit;
#if UNITY_EDITOR
			UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += OnAssemblyReload;
#endif
		}

		/// <summary>
		/// Manually updates this pool.
		/// Updates are responsible for releasing instances when their max lifetime has exceeded (if enabled).
		/// Updates are also automatically invoked whenever an instance is activated or released.
		/// </summary>
		public void Update()
		{
			float gameTime = Time.time;
			if(gameTime == lastUpdateTime) return;
			lastUpdateTime = gameTime;
			if(maxActiveTime.HasValue)
			{
				//Release any instances that have exceeded their max life time
				int activePoolCount = activePool.Count;
				for(int i = 0; i < activePoolCount; i++)
				{
					var instance = activePool[i];
					float activeTime = gameTime - lastActivationTimes[instance];
					if(activeTime > maxActiveTime.Value)
					{
						ReleaseInstance(instance);
						activePoolCount--;
						i--;
					}
				}
			}
		}

		/// <summary>
		/// Activates an instance from this object pool. When no inactive instances are present, a new instance is instantiated.
		/// If the active objects pool is full and 'replaceLast' is enabled, the oldest active instance is recycled,
		/// otherwise this method returns null.
		/// </summary>
		/// <returns>The instance that was activated. Can be a newly created instance or a recycled one.</returns>
		public T ActivateInstance()
		{
			Update();
			T inst;
			if(inactivePool.Count > 0)
			{
				inst = inactivePool[0];
				inactivePool.RemoveAt(0);
				inst.gameObject.SetActive(true);
			}
			else
			{
				if(TotalInstanceCount < maxPoolSize)
				{
					inst = instantiator.Invoke();
					if(inst == null)
					{
						throw new NullReferenceException("Instantiated object was null.");
					}
					if(dontDestroyOnLoad)
					{
						Object.DontDestroyOnLoad(inst.gameObject);
					}
					else if(TargetScene.HasValue && inst.gameObject.scene != TargetScene)
					{
						SceneManager.MoveGameObjectToScene(inst.gameObject, TargetScene.Value);
					}
					//The instantiated object may be inactive if the prefab was stored in the scene, activate it now.
					inst.gameObject.SetActive(true);
				}
				else
				{
					if(replaceLast)
					{
						//Take the oldest active object and reactivate it.
						inst = activePool[0];
						activePool.RemoveAt(0);
						inst.gameObject.SetActive(false);
						deactivator?.Invoke(inst);
						inst.gameObject.SetActive(true);
					}
					else
					{
						Debug.LogError("Max pool size reached. Increase the max pool size or have less objects active at the same time or enable 'alwaysDecativateOldest' to keep activating instances.");
						return null;
					}
				}
			}
			activePool.Add(inst);
			lastActivationTimes[inst] = Time.time;
			activator?.Invoke(inst);
			return inst;
		}

		/// <summary>
		/// Forces activation of a new instance by deactivating an already active one if needed, regardless of the 'replaceLast' setting.
		/// </summary>
		/// <returns>The instance that was activated. Can be a newly created instance or a recycled one.</returns>
		public T ForceActivateInstance()
		{
			if(ActiveInstanceCount >= maxPoolSize)
			{
				//Deactivate the oldest active object, so it can be reactivated again.
				ReleaseInstance(activePool[0]);
			}
			return ActivateInstance();
		}

		/// <summary>
		/// Releases and deactivates the given instance from the active pool.
		/// </summary>
		public void ReleaseInstance(T inst)
		{
			Update();
			if(activePool.Remove(inst))
			{
				inst.gameObject.SetActive(false);
				deactivator?.Invoke(inst);
				inactivePool.Add(inst);
			}
			else
			{
				if(inactivePool.Contains(inst))
				{
					Debug.LogError("The given pooled object was already deactivated.");
				}
				else
				{
					throw new InvalidOperationException("The given object was not part of the pool.");
				}
			}
		}

		/// <summary>
		/// Releases all active instances from this pool.
		/// </summary>
		public void ReleaseAllInstances()
		{
			ForEachActiveInstance((inst) => ReleaseInstance(inst));
		}

		/// <summary>
		/// Destroys all instances created from this pool.
		/// </summary>
		public void DestroyAllInstances()
		{
			foreach(var active in activePool)
			{
				if(active != null) Object.Destroy(active.gameObject);
			}
			foreach(var inactive in inactivePool)
			{
				if(inactive != null) Object.Destroy(inactive.gameObject);
			}
			activePool.Clear();
			inactivePool.Clear();
			lastActivationTimes.Clear();
			//Debug.Log($"InstancePool<{typeof(T).Name}> and all of its instances have been destroyed.");
		}

		/// <summary>
		/// Iterates over all active instances in this pool.
		/// </summary>
		public void ForEachActiveInstance(Action<T> iterator)
		{
			int count = activePool.Count;
			activePool.CopyTo(iterationCache);
			for(int i = 0; i < count; i++)
			{
				iterator(iterationCache[i]);
			}
		}

		/// <summary>
		/// Iterates over all inactive instances in this pool.
		/// </summary>
		public void ForEachInactiveInstance(Action<T> iterator)
		{
			int count = inactivePool.Count;
			inactivePool.CopyTo(iterationCache);
			for(int i = 0; i < count; i++)
			{
				iterator(iterationCache[i]);
			}
		}

		/// <summary>
		/// Transfers all instances to the given scene. Newly created instances will also become part of the given scene.
		/// </summary>
		/// <param name="includeDontDestroyOnLoad">If true, instances that are marked as 'DontDestroyOnLoad' will also be moved to the scene.</param>
		public void TransferToScene(Scene newScene, bool includeDontDestroyOnLoad = false)
		{
			Update();
			foreach(var inst in activePool)
			{
				if(inst && (includeDontDestroyOnLoad || inst.gameObject.scene.name != "DontDestroyOnLoad")) SceneManager.MoveGameObjectToScene(inst.gameObject, newScene);
			}
			foreach(var inst in inactivePool)
			{
				if(inst && (includeDontDestroyOnLoad || inst.gameObject.scene.name != "DontDestroyOnLoad")) SceneManager.MoveGameObjectToScene(inst.gameObject, newScene);
			}
			TargetScene = newScene;
		}

		/// <summary>
		/// Returns the duration the given instance has been active for.
		/// </summary>
		/// <param name="skipChecks">If true, this method will not check if the given instance belongs to this pool, which may raise an exception.
		/// Recommended when invoking this inside tight loops.</param>
		public float GetInstanceActiveTime(T instance, bool skipChecks = false)
		{
			if(skipChecks || activePool.Contains(instance))
			{
				return Time.time - lastActivationTimes[instance];
			}
			else
			{
				Debug.LogError("Unable to get life time for this instance, object is not active or not part of this pool.");
				return 0;
			}
		}

		private T DefaultInstantiator()
		{
			var go = new GameObject("PooledObject");
			if(typeof(T) == typeof(Transform)) return (T)(Component)go.transform;
			else return go.AddComponent<T>();
		}

		private void OnSceneUnloaded(Scene scene)
		{
			if(scene == TargetScene) DestroyAllInstances();
		}

		private void OnApplicationQuit() => DestroyAllInstances();

#if UNITY_EDITOR
		private void OnAssemblyReload()
		{
			if(Application.isPlaying)
			{
				Debug.LogWarning("Assembly reload detected, destroying all pooled instances to avoid leaking objects.");
				DestroyAllInstances();
			}
		}
#endif
	}
}
