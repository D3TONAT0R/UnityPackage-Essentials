using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using UnityEssentials.Reflection;
using UnityPlayerLoop = UnityEngine.LowLevel.PlayerLoop;

namespace UnityEssentials.PlayerLoop
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

		public struct UpdateLoopPreUpdateEvent
		{
		}

		public struct UpdateLoopUpdateEvent
		{
		}

		public struct UpdateLoopPreLateUpdateEvent
		{
		}

		public struct UpdateLoopLateUpdateEvent
		{
		}

		public struct UpdateLoopPostLateUpdateEvent
		{
		}

		public struct UpdateLoopPreFixedUpdateEvent
		{
		}

		public struct UpdateLoopFixedUpdateEvent
		{
		}

		public struct UpdateLoopLateFixedUpdateEvent
		{
		}

		public struct UpdateLoopPostFixedUpdateEvent
		{
		}

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
					targetComponent = GetBehaviourTarget(action?.Target);
					isComponentTarget = targetComponent != null;
				}

				public bool IsActiveAndEnabled()
				{
					if (isComponentTarget) return targetComponent != null && targetComponent.isActiveAndEnabled;
					else return true;
				}
			}

			public readonly string name;
			public List<InvocationTarget> subscribers = new List<InvocationTarget>(256);

			public InvocationList(string name)
			{
				this.name = name;
			}

			public void Add(Action action, bool allowDuplicates = false)
			{
				if (!allowDuplicates)
				{
					for (int i = 0; i < subscribers.Count; i++)
					{
						if (subscribers[i]?.action == action)
						{
							Debug.LogWarning($"Duplicate subscription to {name} ignored.");
							return;
						}
					}
				}
				subscribers.Add(new InvocationTarget(action));
			}

			public void Remove(Action action)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					if (subscribers[i]?.action == action)
					{
						subscribers.RemoveAt(i);
						i--;
					}
				}
			}

			public void RemoveAll(bool? componentActiveState = null)
			{
				if (componentActiveState == true) subscribers.RemoveAll(t => t.IsActiveAndEnabled());
				else if (componentActiveState == false) subscribers.RemoveAll(t => !t.IsActiveAndEnabled());
				else subscribers.Clear();
			}

			public void EnumerateSubscribers(List<InvocationTarget> cache, bool activeComponentsOnly = true)
			{
				cache.Clear();
				for (int i = 0; i < subscribers.Count; i++)
				{
					var sub = subscribers[i];
					if (sub == null || sub.ComponentWasDestroyed)
					{
						if (Application.isPlaying)
							Debug.LogWarning(
								$"Destroyed subscriber detected in {name}. Make sure to unsubscribe from the Update Loop when the object is destroyed.");
						subscribers.RemoveAt(i);
						i--;
					}
					else
					{
						if (activeComponentsOnly && !sub.IsActiveAndEnabled()) continue;
						cache.Add(sub);
					}
				}
			}
		}

		private static readonly Dictionary<Type, FieldInfo[]> behaviourTargetFieldCache = new Dictionary<Type, FieldInfo[]>();
		private static readonly BindingFlags behaviourTargetFieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

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
		/// Event that is invoked before the regular FixedUpdate period.
		/// </summary>
		public static event Action PreFixedUpdate
		{
			add => preFixedUpdate.Add(value);
			remove => preFixedUpdate.Remove(value);
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
		/// Event that is invoked during the regular LateFixedUpdate period.
		/// </summary>
		public static event Action LateFixedUpdate
		{
			add => lateFixedUpdate.Add(value);
			remove => lateFixedUpdate.Remove(value);
		}

		/// <summary>
		/// Event that is invoked after the regular LateFixedUpdate period.
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
			add
			{
				onGUI.Add(value);
				UpdateLoopScriptInstance.CheckInitialization();
			}
			remove => onGUI.Remove(value);
		}

		/// <summary>
		/// Event that is used to draw Gizmos in the scene view (runtime only).
		/// </summary>
		public static event Action OnDrawGizmosRuntime
		{
			add
			{
				onDrawGizmosRuntime.Add(value);
				UpdateLoopScriptInstance.CheckInitialization();
			}
			remove => onDrawGizmosRuntime.Remove(value);
		}

		private static readonly InvocationList preUpdate = new InvocationList("PreUpdate");
		private static readonly InvocationList update = new InvocationList("Update");
		private static readonly InvocationList preLateUpdate = new InvocationList("PreLateUpdate");
		private static readonly InvocationList lateUpdate = new InvocationList("LateUpdate");
		private static readonly InvocationList postLateUpdate = new InvocationList("PostLateUpdate");

		private static readonly InvocationList preFixedUpdate = new InvocationList("PreFixedUpdate");
		private static readonly InvocationList fixedUpdate = new InvocationList("FixedUpdate");
		private static readonly InvocationList lateFixedUpdate = new InvocationList("LateFixedUpdate");
		private static readonly InvocationList postFixedUpdate = new InvocationList("PostFixedUpdate");

		private static readonly InvocationList updateOnce = new InvocationList("UpdateOnce");
		private static readonly InvocationList fixedUpdateOnce = new InvocationList("FixedUpdateOnce");

		private static readonly InvocationList onGUI = new InvocationList("OnGUI");
		private static readonly InvocationList onDrawGizmosRuntime = new InvocationList("OnDrawGizmosRuntime");

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
			if (IsEditorPaused) return;
			var enumerationCache = new List<InvocationList.InvocationTarget>(eventHandler.subscribers.Count);
			eventHandler.EnumerateSubscribers(enumerationCache);
			InvokeEnumeratedSubscribers(enumerationCache);
		}

		private static void InvokeOnce(InvocationList eventHandler)
		{
			if (IsEditorPaused) return;
			var enumerationCache = new List<InvocationList.InvocationTarget>(eventHandler.subscribers.Count);
			eventHandler.EnumerateSubscribers(enumerationCache);
			InvokeEnumeratedSubscribers(enumerationCache);
			eventHandler.RemoveAll(true);
		}

		private static void InvokeEnumeratedSubscribers(List<InvocationList.InvocationTarget> enumerationCache)
		{
			int count = enumerationCache.Count;
			for (int i = 0; i < count; i++)
			{
				try
				{
					enumerationCache[i].action.Invoke();
				}
				catch (Exception e)
				{
					e.LogException();
				}
			}
		}

		internal static void InvokeOnGUI()
		{
			var enumerationCache = new List<InvocationList.InvocationTarget>(onGUI.subscribers.Count);
			onGUI.EnumerateSubscribers(enumerationCache);
			int count = enumerationCache.Count;
			for (int i = 0; i < count; i++)
			{
				try
				{
					enumerationCache[i].action.Invoke();
				}
				catch (Exception e)
				{
					e.LogException();
				}
				finally
				{
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
		}

		internal static void InvokeOnDrawGizmosRuntime()
		{
			var enumerationCache = new List<InvocationList.InvocationTarget>(onDrawGizmosRuntime.subscribers.Count);
			onDrawGizmosRuntime.EnumerateSubscribers(enumerationCache);
			int count = enumerationCache.Count;
			for (int i = 0; i < count; i++)
			{
				try
				{
					enumerationCache[i].action.Invoke();
				}
				catch (Exception e)
				{
					e.LogException();
				}
				finally
				{
					Gizmos.matrix = Matrix4x4.identity;
					Gizmos.color = Color.white;
				}
			}
		}

		private static Behaviour GetBehaviourTarget(object target)
		{
			return GetBehaviourTarget(target, new List<object>());
		}

		private static Behaviour GetBehaviourTarget(object target, List<object> visitedTargets)
		{
			if (target == null) return null;
			if (target is Behaviour behaviour) return behaviour;
			if (target is UnityEngine.Object) return null;

			var targetType = target.GetType();
			if (targetType.IsPrimitive || targetType.IsEnum || targetType == typeof(string)) return null;

			for (int i = 0; i < visitedTargets.Count; i++)
			{
				if (ReferenceEquals(visitedTargets[i], target)) return null;
			}
			visitedTargets.Add(target);

			foreach (var field in GetBehaviourTargetFields(targetType))
			{
				object fieldValue;
				try
				{
					fieldValue = field.GetValue(target);
				}
				catch
				{
					continue;
				}

				if (fieldValue == null) continue;
				if (fieldValue is Behaviour behaviourField) return behaviourField;
				if (fieldValue is UnityEngine.Object) continue;

				var nestedBehaviour = GetBehaviourTarget(fieldValue, visitedTargets);
				if (nestedBehaviour != null) return nestedBehaviour;
			}

			return null;
		}

		private static FieldInfo[] GetBehaviourTargetFields(Type type)
		{
			if (behaviourTargetFieldCache.TryGetValue(type, out var fields)) return fields;

			var collectedFields = new List<FieldInfo>();
			for (var current = type; current != null && current != typeof(object); current = current.BaseType)
			{
				collectedFields.AddRange(current.GetFields(behaviourTargetFieldFlags | BindingFlags.DeclaredOnly));
			}

			fields = collectedFields.ToArray();
			behaviourTargetFieldCache[type] = fields;
			return fields;
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			var loop = UnityPlayerLoop.GetCurrentPlayerLoop();

			InsertSubsystem(ref loop, typeof(PreUpdate), typeof(UpdateLoopPreUpdateEvent), () => Invoke(preUpdate), null, Position.After);
			InsertSubsystem(ref loop, typeof(Update), typeof(UpdateLoopUpdateEvent), () =>
			{
				Invoke(update);
				InvokeOnce(updateOnce);
			}, typeof(Update.ScriptRunBehaviourUpdate), Position.Before);
			InsertSubsystem(ref loop, typeof(Update), typeof(UpdateLoopPreLateUpdateEvent), () => Invoke(preLateUpdate),
				typeof(Update.ScriptRunBehaviourUpdate), Position.After);
			InsertSubsystem(ref loop, typeof(PreLateUpdate), typeof(UpdateLoopLateUpdateEvent), () => Invoke(lateUpdate),
				typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), Position.Before);
			InsertSubsystem(ref loop, typeof(PostLateUpdate), typeof(UpdateLoopPostLateUpdateEvent), () => Invoke(postLateUpdate), null,
				Position.After);

			InsertSubsystem(ref loop, typeof(FixedUpdate), typeof(UpdateLoopPreFixedUpdateEvent), () => Invoke(preFixedUpdate),
				typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), Position.Before);
			InsertSubsystem(ref loop, typeof(FixedUpdate), typeof(UpdateLoopFixedUpdateEvent), () =>
			{
				Invoke(fixedUpdate);
				InvokeOnce(fixedUpdateOnce);
			}, typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), Position.Before);
			InsertSubsystem(ref loop, typeof(FixedUpdate), typeof(UpdateLoopLateFixedUpdateEvent), () => Invoke(lateFixedUpdate),
				typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), Position.After);
			InsertSubsystem(ref loop, typeof(FixedUpdate), typeof(UpdateLoopPostFixedUpdateEvent), () => Invoke(postFixedUpdate),
				typeof(UpdateLoopLateFixedUpdateEvent), Position.After);

			UnityPlayerLoop.SetPlayerLoop(loop);

			SubscribeMethodsWithAttributes();

			//LogCurrentPlayerLoop(false);

			Application.quitting += Cleanup;
		}

		private static void Cleanup()
		{
			preUpdate.RemoveAll();
			update.RemoveAll();
			preLateUpdate.RemoveAll();
			lateUpdate.RemoveAll();
			postLateUpdate.RemoveAll();
			preFixedUpdate.RemoveAll();
			fixedUpdate.RemoveAll();
			lateFixedUpdate.RemoveAll();
			postFixedUpdate.RemoveAll();
			updateOnce.RemoveAll();
			fixedUpdateOnce.RemoveAll();
			onGUI.RemoveAll();
			onDrawGizmosRuntime.RemoveAll();
			UpdateLoopScriptInstance.Cleanup();
		}

		private static void SubscribeMethodsWithAttributes()
		{
			SubscribeAttributeToEvent<PreUpdateAttribute>(preUpdate);
			SubscribeAttributeToEvent<UpdateAttribute>(update);
			SubscribeAttributeToEvent<PreLateUpdateAttribute>(preLateUpdate);
			SubscribeAttributeToEvent<LateUpdateAttribute>(lateUpdate);
			SubscribeAttributeToEvent<PostLateUpdateAttribute>(postLateUpdate);
			SubscribeAttributeToEvent<PreFixedUpdateAttribute>(preFixedUpdate);
			SubscribeAttributeToEvent<FixedUpdateAttribute>(fixedUpdate);
			SubscribeAttributeToEvent<LateFixedUpdateAttribute>(lateFixedUpdate);
			SubscribeAttributeToEvent<PostFixedUpdateAttribute>(postFixedUpdate);
			SubscribeAttributeToEvent<UpdateOnceAttribute>(updateOnce);
			SubscribeAttributeToEvent<FixedUpdateOnceAttribute>(fixedUpdateOnce);
			SubscribeAttributeToEvent<OnGUIAttribute>(onGUI);
			SubscribeAttributeToEvent<OnDrawGizmosRuntimeAttribute>(onDrawGizmosRuntime);
			if (onDrawGizmosRuntime.subscribers.Count > 0)
			{
				UpdateLoopScriptInstance.CheckInitialization();
			}
		}

		private static void SubscribeAttributeToEvent<A>(InvocationList eventHandler) where A : Attribute
		{
			foreach (var m in ReflectionUtility.GetMethodsWithAttribute<A>(true))
			{
				if (m.ReturnType != typeof(void) || m.GetParameters().Length != 0)
				{
					Debug.LogError(
						$"The Attribute '{typeof(A)}' is only valid on static parameterless void methods ({m.DeclaringType}:{m.Name}). The method will not be invoked.");
					continue;
				}

				try
				{
					var action = (Action)m.CreateDelegate(typeof(Action));
					eventHandler.Add(action);
				}
				catch (Exception e)
				{
					Debug.LogError(
						$"Failed to subscribe method ({m.DeclaringType}:{m.Name}) to '{typeof(A)}'. The method will not be invoked.");
					e.LogException();
				}
			}
			foreach (var m in ReflectionUtility.GetMethodsWithAttribute<A>(false))
			{
				Debug.LogError(
					$"The Attribute '{typeof(A)}' is only valid on static methods ({m.DeclaringType}:{m.Name}). The method will not be invoked.");
			}
		}

		/// <summary>
		/// Adds a subsystem to the given root system, before all other child systems.
		/// </summary>
		public static void AddSubsystemFirst(Type subSystemRoot, Type add, PlayerLoopSystem.UpdateFunction invocationTarget)
		{
			var loop = UnityPlayerLoop.GetCurrentPlayerLoop();
			InsertSubsystem(ref loop, subSystemRoot, add, invocationTarget, null, Position.Before);
			UnityPlayerLoop.SetPlayerLoop(loop);
		}

		/// <summary>
		/// Adds a subsystem to the given root system, before the given child system.
		/// </summary>
		public static void AddSubsystemBefore(Type subSystemRoot, Type add, PlayerLoopSystem.UpdateFunction invocationTarget, Type beforeSubSystem)
		{
			var loop = UnityPlayerLoop.GetCurrentPlayerLoop();
			InsertSubsystem(ref loop, subSystemRoot, add, invocationTarget, beforeSubSystem, Position.Before);
			UnityPlayerLoop.SetPlayerLoop(loop);
		}
		
		/// <summary>
		/// Adds a subsystem to the given root system, after the given child system.
		/// </summary>
		public static void AddSubsystemAfter(Type subSystemRoot, Type add, PlayerLoopSystem.UpdateFunction invocationTarget, Type afterSubSystem)
		{
			var loop = UnityPlayerLoop.GetCurrentPlayerLoop();
			InsertSubsystem(ref loop, subSystemRoot, add, invocationTarget, afterSubSystem, Position.After);
			UnityPlayerLoop.SetPlayerLoop(loop);
		}
		
		/// <summary>
		/// Adds a subsystem to the given root system, after all other child systems.
		/// </summary>
		public static void AddSubsystemLast(Type subSystemRoot, Type add, PlayerLoopSystem.UpdateFunction invocationTarget)
		{
			var loop = UnityPlayerLoop.GetCurrentPlayerLoop();
			InsertSubsystem(ref loop, subSystemRoot, add, invocationTarget, null, Position.After);
			UnityPlayerLoop.SetPlayerLoop(loop);
		}

		private static void InsertSubsystem(ref PlayerLoopSystem root, Type subSystemRoot, Type typeToAdd,
			PlayerLoopSystem.UpdateFunction invocationTarget, Type referenceSubSystem, Position position)
		{
			int index = -1;
			for (int i = 0; i < root.subSystemList.Length; i++)
			{
				if (root.subSystemList[i].type != null && root.subSystemList[i].type.Name == subSystemRoot.Name)
				{
					index = i;
					break;
				}
			}

			if (index < 0)
			{
				throw new InvalidOperationException($"Subsystem of type '{subSystemRoot}' not found.");
			}

			var sub = root.subSystemList[index];
			Insert(ref sub, new PlayerLoopSystem() { updateDelegate = invocationTarget, type = typeToAdd }, referenceSubSystem, position);
			root.subSystemList[index] = sub;
		}

		private static void Insert(ref PlayerLoopSystem system, PlayerLoopSystem systemToAdd, Type reference, Position position)
		{
			List<PlayerLoopSystem> subsystems;
			if (system.subSystemList == null)
			{
				subsystems = new List<PlayerLoopSystem>();
			}
			else
			{
				subsystems = new List<PlayerLoopSystem>(system.subSystemList);
			}

			if (reference != null)
			{
				var index = subsystems.FindIndex((s) => s.type == reference);
				if (index < 0)
				{
					throw new InvalidOperationException($"Subsystem of type '{reference}' not found, system not added.");
				}
				systemToAdd.loopConditionFunction = subsystems[index].loopConditionFunction;
				if (position == Position.Before)
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
				if (position == Position.Before)
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
			for (int i = 0; i < indentLevel; i++) sb.Append("\t");
			string name = root.type?.Name ?? (indentLevel == 0 ? "PlayerLoop" : "<NULL>");
			sb.Append(name);
			if (includeFunctionPtrs)
			{
				ulong loopConditionPtr = (ulong)root.loopConditionFunction;
				ulong updatePtr = (ulong)root.updateFunction;
				if (loopConditionPtr != 0 || updatePtr != 0)
				{
					sb.Append("    [");
					if (loopConditionPtr != 0) sb.Append($"LoopCondition: 0x{loopConditionPtr:X}");
					if (loopConditionPtr != 0 && updatePtr != 0) sb.Append(" ");
					if (updatePtr != 0) sb.Append($"Update: 0x{updatePtr:X}");
					sb.Append("]");
				}
			}
			sb.AppendLine();

			if (root.subSystemList != null)
			{
				indentLevel++;
				foreach (var s in root.subSystemList)
				{
					PrintLoopRecursive(sb, s, indentLevel, includeFunctionPtrs);
				}
			}
		}
	}
}