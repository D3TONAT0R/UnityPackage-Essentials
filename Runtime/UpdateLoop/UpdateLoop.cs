using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityPlayerLoop = UnityEngine.LowLevel.PlayerLoop;

namespace D3T.PlayerLoop
{
	/// <summary>
	/// Utility class for adding subsystems to the game loop.
	/// </summary>
	public static class UpdateLoop
	{
		public enum Position
		{
			Before,
			After
		}

		public struct UpdateLoopPreUpdateEvent { }
		public struct UpdateLoopUpdateEvent { }
		public struct UpdateLoopPreLateUpdateEvent { }
		public struct UpdateLoopLateUpdateEvent { }
		public struct UpdateLoopPostLateUpdateEvent { }

		public struct UpdateLoopFixedUpdateEvent { }
		public struct UpdateLoopLateFixedUpdateEvent { }
		public struct UpdateLoopPostFixedUpdateEvent { }

		internal class InvocationList
		{
			public class InvocationTarget
			{
				public Action action;
				public Behaviour targetComponent;

				public readonly bool isComponentTarget;

				public bool ComponentWasDestroyed => isComponentTarget && targetComponent == null;

				public InvocationTarget(Action action)
				{
					this.action = action;
					targetComponent = action.Target as Behaviour;
					isComponentTarget = targetComponent != null;
				}

				public bool IsActiveAndEnabled()
				{
					if(isComponentTarget) return targetComponent != null && targetComponent.isActiveAndEnabled;
					else return true;
				}
			}

			public readonly string name;
			public List<InvocationTarget> subscribers = new List<InvocationTarget>(256);

			public InvocationList(string name)
			{
				this.name = name;
			}

			public void Add(Action action)
			{
				subscribers.Add(new InvocationTarget(action));
			}

			public void Remove(Action action)
			{
				for(int i = 0; i < subscribers.Count; i++)
				{
					if(subscribers[i]?.action == action)
					{
						subscribers.RemoveAt(i);
						return;
					}
				}
			}

			public void RemoveAll(bool? componentActiveState = null)
			{
				if(componentActiveState == true) subscribers.RemoveAll(t => t.IsActiveAndEnabled());
				else if(componentActiveState == false) subscribers.RemoveAll(t => !t.IsActiveAndEnabled());
				else subscribers.Clear();
			}

			public void EnumerateSubscribers(List<InvocationTarget> cache, bool activeComponentsOnly = true)
			{
				cache.Clear();
				for(int i = 0; i < subscribers.Count; i++)
				{
					var sub = subscribers[i];
					if(sub == null || sub.ComponentWasDestroyed)
					{
						if(Application.isPlaying) Debug.LogWarning($"Destroyed subscriber detected in {name}. Make sure to unsubscribe from the Update Loop when the object is destroyed.");
						subscribers.RemoveAt(i);
						i--;
					}
					else
					{
						if(activeComponentsOnly && !sub.IsActiveAndEnabled()) continue;
						cache.Add(sub);
					}
				}
			}
		}

		/// <summary>
		/// Event that is invoked before the regular Update period.
		/// </summary>
		public static event Action PreUpdate
		{
			add => preUpdate.Add(value);
			remove => preUpdate.Remove(value);
		}

		/// <summary>
		/// Event that is invoked during the regular Update period.
		/// </summary>
		public static event Action Update
		{
			add => update.Add(value);
			remove => update.Remove(value);
		}

		/// <summary>
		/// Event that is invoked after the Update and before the LateUpdate period.
		/// </summary>
		public static event Action PreLateUpdate
		{
			add => preLateUpdate.Add(value);
			remove => preLateUpdate.Remove(value);
		}

		/// <summary>
		/// Event that is invoked during the regular LateUpdate period.
		/// </summary>
		public static event Action LateUpdate
		{
			add => lateUpdate.Add(value);
			remove => lateUpdate.Remove(value);
		}

		/// <summary>
		/// Event that is invoked after the regular LateUpdate period.
		/// </summary>
		public static event Action PostLateUpdate
		{
			add => postLateUpdate.Add(value);
			remove => postLateUpdate.Remove(value);
		}


		/// <summary>
		/// Event that is invoked during the regular FixedUpdate period.
		/// </summary>
		public static event Action FixedUpdate
		{
			add => fixedUpdate.Add(value);
			remove => fixedUpdate.Remove(value);
		}

		/// <summary>
		/// Event that is invoked during the regular FixedUpdate period.
		/// </summary>
		public static event Action LateFixedUpdate
		{
			add => lateFixedUpdate.Add(value);
			remove => lateFixedUpdate.Remove(value);
		}

		/// <summary>
		/// Event that is invoked after the regular FixedUpdate period.
		/// </summary>
		public static event Action PostFixedUpdate
		{
			add => postFixedUpdate.Add(value);
			remove => postFixedUpdate.Remove(value);
		}


		/// <summary>
		/// Event that is invoked only once for each subscriber during the regular Update event.
		/// </summary>
		public static event Action UpdateOnce
		{
			add => updateOnce.Add(value);
			remove => updateOnce.Remove(value);
		}

		/// <summary>
		/// Event that is invoked only once for each subscriber during the fixed Update event.
		/// </summary>
		public static event Action FixedUpdateOnce
		{
			add => fixedUpdateOnce.Add(value);
			remove => fixedUpdateOnce.Remove(value);
		}


		/// <summary>
		/// Event that is invoked during the OnGUI period.
		/// </summary>
		public static event Action OnGUI
		{
			add => onGUI.Add(value);
			remove => onGUI.Remove(value);
		}

		/// <summary>
		/// Event that is used to draw Gizmos in the scene view (runtime only).
		/// </summary>
		public static event Action OnDrawGizmosRuntime
		{
			add => onDrawGizmosRuntime.Add(value);
			remove => onDrawGizmosRuntime.Remove(value);
		}

		private static readonly InvocationList preUpdate = new InvocationList("PreUpdate");
		private static readonly InvocationList update = new InvocationList("Update");
		private static readonly InvocationList preLateUpdate = new InvocationList("PreLateUpdate");
		private static readonly InvocationList lateUpdate = new InvocationList("LateUpdate");
		private static readonly InvocationList postLateUpdate = new InvocationList("PostLateUpdate");

		private static readonly InvocationList fixedUpdate = new InvocationList("FixedUpdate");
		private static readonly InvocationList lateFixedUpdate = new InvocationList("LateFixedUpdate");
		private static readonly InvocationList postFixedUpdate = new InvocationList("PostFixedUpdate");

		private static readonly InvocationList updateOnce = new InvocationList("UpdateOnce");
		private static readonly InvocationList fixedUpdateOnce = new InvocationList("FixedUpdateOnce");

		private static readonly InvocationList onGUI = new InvocationList("OnGUI");
		private static readonly InvocationList onDrawGizmosRuntime = new InvocationList("OnDrawGizmosRuntime");

		private static readonly List<InvocationList.InvocationTarget> enumerationCache = new List<InvocationList.InvocationTarget>();

		private static bool IsEditorPaused
		{
			get
			{
#if UNITY_EDITOR
				return !UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPaused;
#else
				return false;
#endif
			}
		}

		private static void Invoke(InvocationList eventHandler)
		{
			if(IsEditorPaused) return;
			eventHandler.EnumerateSubscribers(enumerationCache);
			int count = enumerationCache.Count;
			for(int i = 0; i < count; i++)
			{
				enumerationCache[i].action.Invoke();
			}
		}

		private static void InvokeOnce(InvocationList eventHandler)
		{
			if(IsEditorPaused) return;
			eventHandler.EnumerateSubscribers(enumerationCache);
			int count = enumerationCache.Count;
			for(int i = 0; i < count; i++)
			{
				enumerationCache[i].action.Invoke();
			}
			eventHandler.RemoveAll(true);
		}

		internal static void InvokeOnGUI()
		{
			onGUI.EnumerateSubscribers(enumerationCache);
			int count = enumerationCache.Count;
			for(int i = 0; i < count; i++)
			{
				enumerationCache[i].action.Invoke();
				GUI.enabled = true;
				GUI.depth = 0;
				GUI.changed = false;
				GUI.tooltip = null;
				GUI.matrix = Matrix4x4.identity;
				GUI.color = Color.white;
				GUI.backgroundColor = Color.white;
				GUI.contentColor = Color.white;
			}
		}

		internal static void InvokeOnDrawGizmosRuntime()
		{
			onDrawGizmosRuntime.EnumerateSubscribers(enumerationCache);
			int count = enumerationCache.Count;
			for(int i = 0; i < count; i++)
			{
				enumerationCache[i].action.Invoke();
				Gizmos.matrix = Matrix4x4.identity;
				Gizmos.color = Color.white;
			}
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			var loop = UnityPlayerLoop.GetCurrentPlayerLoop();

			InsertSubsystem(ref loop, typeof(PreUpdate), typeof(UpdateLoopPreUpdateEvent), () => Invoke(preUpdate), null, Position.After);
			InsertSubsystem(ref loop, typeof(Update), typeof(UpdateLoopUpdateEvent), () => { Invoke(update); InvokeOnce(updateOnce); }, typeof(Update.ScriptRunBehaviourUpdate), Position.Before);
			InsertSubsystem(ref loop, typeof(Update), typeof(UpdateLoopPreLateUpdateEvent), () => Invoke(preLateUpdate), typeof(Update.ScriptRunBehaviourUpdate), Position.After);
			InsertSubsystem(ref loop, typeof(PreLateUpdate), typeof(UpdateLoopLateUpdateEvent), () => Invoke(lateUpdate), typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), Position.Before);
			InsertSubsystem(ref loop, typeof(PostLateUpdate), typeof(UpdateLoopPostLateUpdateEvent), () => Invoke(postLateUpdate), null, Position.After);

			InsertSubsystem(ref loop, typeof(FixedUpdate), typeof(UpdateLoopFixedUpdateEvent), () => { Invoke(fixedUpdate); InvokeOnce(fixedUpdateOnce); }, typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), Position.Before);
			InsertSubsystem(ref loop, typeof(FixedUpdate), typeof(UpdateLoopLateFixedUpdateEvent), () => Invoke(lateFixedUpdate), typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), Position.After);
			InsertSubsystem(ref loop, typeof(FixedUpdate), typeof(UpdateLoopPostFixedUpdateEvent), () => Invoke(postFixedUpdate), typeof(UpdateLoopLateFixedUpdateEvent), Position.After);

			UnityPlayerLoop.SetPlayerLoop(loop);

			UpdateLoopScriptInstance.Init();
			SubscribeMethodsWithAttributes();

			//LogCurrentPlayerLoop(false);

			Application.quitting += Cleanup;
		}

		private static void Cleanup()
		{
#if UNITY_EDITOR
			preUpdate.RemoveAll();
			update.RemoveAll();
			preLateUpdate.RemoveAll();
			lateUpdate.RemoveAll();
			postLateUpdate.RemoveAll();
			fixedUpdate.RemoveAll();
			postFixedUpdate.RemoveAll();
			updateOnce.RemoveAll();
			onGUI.RemoveAll();
			onDrawGizmosRuntime.RemoveAll();
			UpdateLoopScriptInstance.Cleanup();
#endif
		}

		private static void SubscribeMethodsWithAttributes()
		{
			SubscribeAttributeToEvent<PreUpdateAttribute>(preUpdate);
			SubscribeAttributeToEvent<UpdateAttribute>(update);
			SubscribeAttributeToEvent<PreLateUpdateAttribute>(preLateUpdate);
			SubscribeAttributeToEvent<LateUpdateAttribute>(lateUpdate);
			SubscribeAttributeToEvent<PostLateUpdateAttribute>(postLateUpdate);
			SubscribeAttributeToEvent<FixedUpdateAttribute>(fixedUpdate);
			SubscribeAttributeToEvent<LateFixedUpdateAttribute>(lateFixedUpdate);
			SubscribeAttributeToEvent<PostFixedUpdateAttribute>(postFixedUpdate);
			SubscribeAttributeToEvent<UpdateOnceAttribute>(updateOnce);
			SubscribeAttributeToEvent<FixedUpdateOnceAttribute>(fixedUpdateOnce);
			SubscribeAttributeToEvent<OnGUIAttribute>(onGUI);
			SubscribeAttributeToEvent<OnDrawGizmosRuntimeAttribute>(onDrawGizmosRuntime);
		}

		private static void SubscribeAttributeToEvent<A>(InvocationList eventHandler) where A : Attribute
		{
			foreach(var m in ReflectionUtility.GetMethodsWithAttribute<A>(true))
			{
				var action = (Action)m.CreateDelegate(typeof(Action));
				eventHandler.Add(action);
			}
			foreach(var m in ReflectionUtility.GetMethodsWithAttribute<A>(false))
			{
				Debug.LogError($"The Attribute '{typeof(A)}' is only valid on static methods ({m.DeclaringType}:{m.Name}). The method will not be invoked.");
			}
		}

		/// <summary>
		/// Adds a subsystem to the given root system before all other child systems.
		/// </summary>
		public static void AddSubsystemFirst(Type subSystemRoot, Type add, PlayerLoopSystem.UpdateFunction invocationTarget)
		{
			var loop = UnityPlayerLoop.GetCurrentPlayerLoop();
			InsertSubsystem(ref loop, subSystemRoot, add, invocationTarget, null, Position.Before);
			UnityPlayerLoop.SetPlayerLoop(loop);
		}

		/// <summary>
		/// Adds a subsystem to the given root system after all other child systems.
		/// </summary>
		public static void AddSubsystemLast(Type subSystemRoot, Type add, PlayerLoopSystem.UpdateFunction invocationTarget)
		{
			var loop = UnityPlayerLoop.GetCurrentPlayerLoop();
			InsertSubsystem(ref loop, subSystemRoot, add, invocationTarget, null, Position.After);
			UnityPlayerLoop.SetPlayerLoop(loop);
		}

		/// <summary>
		/// Adds a subsystem to the given root system and after the given child system.
		/// </summary>
		public static void AddSubsystemBefore(Type subSystemRoot, Type add, PlayerLoopSystem.UpdateFunction invocationTarget, Type beforeSubSystem)
		{
			var loop = UnityPlayerLoop.GetCurrentPlayerLoop();
			InsertSubsystem(ref loop, subSystemRoot, add, invocationTarget, beforeSubSystem, Position.Before);
			UnityPlayerLoop.SetPlayerLoop(loop);
		}

		/// <summary>
		/// Adds a subsystem to the given root system and after the given child system.
		/// </summary>
		public static void AddSubsystemAfter(Type subSystemRoot, Type add, PlayerLoopSystem.UpdateFunction invocationTarget, Type afterSubSystem)
		{
			var loop = UnityPlayerLoop.GetCurrentPlayerLoop();
			InsertSubsystem(ref loop, subSystemRoot, add, invocationTarget, afterSubSystem, Position.After);
			UnityPlayerLoop.SetPlayerLoop(loop);
		}

		private static void InsertSubsystem(ref PlayerLoopSystem root, Type subSystemRoot, Type typeToAdd, PlayerLoopSystem.UpdateFunction invocationTarget, Type referenceSubSystem, Position position)
		{
			int index = -1;
			for(int i = 0; i < root.subSystemList.Length; i++)
			{
				if(root.subSystemList[i].type != null && root.subSystemList[i].type.Name == subSystemRoot.Name)
				{
					index = i;
					break;
				}
			}

			if(index < 0)
			{
				throw new NullReferenceException($"Subsystem of type '{subSystemRoot}' not found.");
			}

			var sub = root.subSystemList[index];
			Insert(ref sub, new PlayerLoopSystem() { updateDelegate = invocationTarget, type = typeToAdd }, referenceSubSystem, position);
			root.subSystemList[index] = sub;
		}

		private static void Insert(ref PlayerLoopSystem system, PlayerLoopSystem systemToAdd, Type reference, Position position)
		{
			List<PlayerLoopSystem> subsystems;
			if(system.subSystemList == null)
			{
				subsystems = new List<PlayerLoopSystem>();
			}
			else
			{
				subsystems = new List<PlayerLoopSystem>(system.subSystemList);
			}

			if(reference != null)
			{
				var index = subsystems.FindIndex((s) => s.type == reference);
				if(index < 0)
				{
					throw new NullReferenceException($"Subsystem of type '{reference}' not found, system not added.");
				}
				systemToAdd.loopConditionFunction = subsystems[index].loopConditionFunction;
				if(position == Position.Before)
				{
					subsystems.Insert(index, systemToAdd);
				}
				else
				{
					subsystems.Insert(index + 1, systemToAdd);
				}
			}
			else
			{
				if(position == Position.Before)
				{
					subsystems.Insert(0, systemToAdd);
				}
				else
				{
					subsystems.Add(systemToAdd);
				}
			}
			system.subSystemList = subsystems.ToArray();
		}

		/// <summary>
		/// Logs the entire hierarchy of player loop systems to the console.
		/// </summary>
		public static void LogCurrentPlayerLoop(bool includeFunctionPtrs)
		{
			var sb = new StringBuilder();
			PrintLoopRecursive(sb, UnityPlayerLoop.GetCurrentPlayerLoop(), 0, includeFunctionPtrs);
			Debug.Log(sb.ToString());
		}

		private static void PrintLoopRecursive(StringBuilder sb, PlayerLoopSystem root, int indentLevel, bool includeFunctionPtrs)
		{
			for(int i = 0; i < indentLevel; i++) sb.Append("\t");
			string name = root.type?.Name ?? (indentLevel == 0 ? "PlayerLoop" : "<NULL>");
			sb.Append(name);
			if(includeFunctionPtrs)
			{
				int loopConditionPtr = (int)root.loopConditionFunction;
				int updatePtr = (int)root.updateFunction;
				if(loopConditionPtr != 0 || updatePtr != 0)
				{
					sb.Append("    [");
					if(loopConditionPtr != 0) sb.Append($"LoopCondition: 0x{loopConditionPtr:X}");
					if(loopConditionPtr != 0 && updatePtr != 0) sb.Append(" ");
					if(updatePtr != 0) sb.Append($"Update: 0x{updatePtr:X}");
					sb.Append("]");
				}
			}
			sb.AppendLine();

			if(root.subSystemList != null)
			{
				indentLevel++;
				foreach(var s in root.subSystemList)
				{
					PrintLoopRecursive(sb, s, indentLevel, includeFunctionPtrs);
				}
			}
		}
	}
}
