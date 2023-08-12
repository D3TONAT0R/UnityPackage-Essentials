using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace D3T
{
	public class InstancePool<T> where T : Component
	{
		private List<T> inactivePool;
		private List<T> activePool;

		public int maxPoolSize;
		public Func<T> instantiator;
		public Action<T> activator = null;
		public Action<T> deactivator = null;
		public bool hideInstancesInHierarchy = false;
		public bool alwaysDeactivateOldest = false;
		public bool dontDestroyOnLoad = false;

		public int InactiveCount => inactivePool.Count;
		public int ActiveCount => activePool.Count;
		public int TotalCount => InactiveCount + ActiveCount;

		public InstancePool(int maxPoolSize, Func<T> instantiator = null, Action<T> activator = null, Action<T> deactivator = null, bool hideInstancesInHierarchy = true, bool dontDestroyOnLoad = false)
		{
			inactivePool = new List<T>();
			activePool = new List<T>();
			this.maxPoolSize = maxPoolSize;
			this.instantiator = instantiator ?? DefaultInstantiator;
			this.activator = activator;
			this.deactivator = deactivator;
			this.hideInstancesInHierarchy = hideInstancesInHierarchy;
			this.dontDestroyOnLoad = dontDestroyOnLoad;
		}

		~InstancePool()
		{
			foreach(var active in activePool)
			{
				if(active != null) Object.Destroy(active);
			}
			foreach(var inactive in inactivePool)
			{
				if(inactive != null) Object.Destroy(inactive);
			}
			//Debug.Log($"InstancePool<{typeof(T).Name}> and all of it's instances have been destroyed.");
		}

		private T DefaultInstantiator()
		{
			var go = new GameObject("PooledObject");
			return go.AddComponent<T>();
		}

		public T ActivateInstance()
		{
			T inst;
			if(inactivePool.Count > 0)
			{
				inst = inactivePool[0];
				inactivePool.RemoveAt(0);
				inst.gameObject.SetActive(true);
			}
			else
			{
				if(TotalCount < maxPoolSize)
				{
					inst = instantiator();
					if(inst == null)
					{
						throw new NullReferenceException("Instantiated object was null.");
					}
					if(dontDestroyOnLoad) Object.DontDestroyOnLoad(inst.gameObject);
					//The instantiated object may be inactive if the prefab was stored in the scene, activate it now.
					inst.gameObject.SetActive(true);
					inst.hideFlags = hideInstancesInHierarchy ? HideFlags.HideAndDontSave : HideFlags.DontSave;
				}
				else
				{
					if(alwaysDeactivateOldest)
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
			activator?.Invoke(inst);
			return inst;
		}

		public T ForceActivateInstance()
		{
			if(ActiveCount >= maxPoolSize)
			{
				//Deactivate the oldest active object, so it can be reactivated again.
				ReleaseInstance(activePool[0]);
			}
			return ActivateInstance();
		}

		public void ReleaseInstance(T inst)
		{
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

		public void ReleaseAllInstances()
		{
			IterateActiveInstances((inst) => ReleaseInstance(inst));
		}

		public void IterateActiveInstances(Action<T> iterator)
		{
			var pool = activePool.ToArray();
			foreach(var a in pool) iterator(a);
		}

		public void IterateInactiveInstances(Action<T> iterator)
		{
			var pool = inactivePool.ToArray();
			foreach(var a in pool) iterator(a);
		}

		public void TransferToScene(Scene scene)
		{
			foreach(var inst in activePool)
			{
				if(inst && inst.gameObject.scene.name != "DontDestroyOnLoad") SceneManager.MoveGameObjectToScene(inst.gameObject, scene);
			}
			foreach(var inst in inactivePool)
			{
				if(inst && inst.gameObject.scene.name != "DontDestroyOnLoad") SceneManager.MoveGameObjectToScene(inst.gameObject, scene);
			}
		}
	}
}
