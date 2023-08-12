using D3T.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace D3T
{
	public static class UpdateLoop
	{
		public static event Action PreUpdate;
		public static event Action Update;
		public static event Action PreLateUpdate;
		public static event Action LateUpdate;
		public static event Action PostLateUpdate;

		public static event Action FixedUpdate;
		public static event Action PostFixedUpdate;

		public static event Action UpdateOnce;

		public static event Action OnGUI;
		public static event Action OnDrawGizmosRuntime;

		private static void Invoke(Action eventHandler)
		{
			ForEachSubscriberIfEnabled(eventHandler, (a) =>
			{
				a.Invoke();
				GUI.enabled = true;
				GUI.depth = 0;
				GUI.changed = false;
				GUI.tooltip = null;
				GUI.matrix = Matrix4x4.identity;
				GUI.color = Color.white;
				GUI.backgroundColor = Color.white;
				GUI.contentColor = Color.white;
			});
		}

		private static void InvokeOnce(ref Action eventHandler)
		{
			if(ForEachSubscriberIfEnabled(eventHandler, (a) => a.Invoke()))
			{
				eventHandler = null;
			}
		}

		internal static void InvokeOnGUI()
		{
			ForEachSubscriberIfEnabled(OnGUI, (a) =>
			{
				a.Invoke();
				GUI.enabled = true;
				GUI.depth = 0;
				GUI.changed = false;
				GUI.tooltip = null;
				GUI.matrix = Matrix4x4.identity;
				GUI.color = Color.white;
				GUI.backgroundColor = Color.white;
				GUI.contentColor = Color.white;
			});
		}

		internal static void InvokeOnDrawGizmosRuntime()
		{
			ForEachSubscriberIfEnabled(OnDrawGizmosRuntime, (a) =>
			{
				a.Invoke();
				Gizmos.matrix = Matrix4x4.identity;
				Gizmos.color = Color.white;
			}, true);
		}

		private static bool ForEachSubscriberIfEnabled(Action eventHandler, Action<Action> func, bool ignorePause = false)
		{
#if UNITY_EDITOR
			if(!ignorePause && (!UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPaused))
			{
				return false;
			}
#endif
			if(eventHandler == null) return false;
			var delegates = eventHandler.GetInvocationList();
			int length = delegates.Length;
			for(int i = 0; i < length; i++)
			{
				var d = delegates[i];
				if(d != null)
				{
					if(d.Target != null && d.Target is Behaviour b)
					{
						if(b == null || !b.enabled || !b.gameObject.activeInHierarchy)
						{
							continue;
						}
					}
					func((Action)d);
				}
			}
			return true;
		}

		public struct UpdateLoopPreUpdateEvent { }
		public struct UpdateLoopUpdateEvent { }
		public struct UpdateLoopPreLateUpdateEvent { }
		public struct UpdateLoopLateUpdateEvent { }
		public struct UpdateLoopPostLateUpdateEvent { }

		public struct UpdateLoopFixedUpdateEvent { }
		public struct UpdateLoopPostFixedUpdateEvent { }

		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			var loop = PlayerLoop.GetCurrentPlayerLoop();

			InsertSubsystem(ref loop, typeof(PreUpdate), typeof(UpdateLoopPreUpdateEvent), () => Invoke(PreUpdate), null, false);
			InsertSubsystem(ref loop, typeof(Update), typeof(UpdateLoopUpdateEvent), () => { Invoke(Update); InvokeOnce(ref UpdateOnce); }, typeof(Update.ScriptRunBehaviourUpdate), true);
			InsertSubsystem(ref loop, typeof(Update), typeof(UpdateLoopPreLateUpdateEvent), () => Invoke(PreLateUpdate), typeof(Update.ScriptRunBehaviourUpdate), false);
			InsertSubsystem(ref loop, typeof(PreLateUpdate), typeof(UpdateLoopLateUpdateEvent), () => Invoke(LateUpdate), typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), true);
			InsertSubsystem(ref loop, typeof(PostLateUpdate), typeof(UpdateLoopPostLateUpdateEvent), () => Invoke(PostLateUpdate), null, false);

			InsertSubsystem(ref loop, typeof(FixedUpdate), typeof(UpdateLoopFixedUpdateEvent), () => Invoke(FixedUpdate), typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), true);
			InsertSubsystem(ref loop, typeof(FixedUpdate), typeof(UpdateLoopPostFixedUpdateEvent), () => Invoke(PostFixedUpdate), typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), false);

			PlayerLoop.SetPlayerLoop(loop);

			UpdateLoopScriptInstance.Init();
			SubscribeMethodsWithAttributes();

			Application.quitting += Cleanup;
		}

		private static void Cleanup()
		{
#if UNITY_EDITOR
			PreUpdate = null;
			Update = null;
			PreLateUpdate = null;
			LateUpdate = null;
			PostLateUpdate = null;
			FixedUpdate = null;
			PostFixedUpdate = null;
			UpdateOnce = null;
			OnGUI = null;
			OnDrawGizmosRuntime = null;
			UpdateLoopScriptInstance.Cleanup();
#endif
		}

		private static void SubscribeMethodsWithAttributes()
		{
			SubscribeAttributeToEvent<PreUpdateAttribute>(ref PreUpdate);
			SubscribeAttributeToEvent<UpdateAttribute>(ref Update);
			SubscribeAttributeToEvent<PreLateUpdateAttribute>(ref PreLateUpdate);
			SubscribeAttributeToEvent<LateUpdateAttribute>(ref LateUpdate);
			SubscribeAttributeToEvent<PostLateUpdateAttribute>(ref PostLateUpdate);
			SubscribeAttributeToEvent<FixedUpdateAttribute>(ref FixedUpdate);
			SubscribeAttributeToEvent<PostFixedUpdateAttribute>(ref PostFixedUpdate);
			SubscribeAttributeToEvent<UpdateOnceAttribute>(ref UpdateOnce);
			SubscribeAttributeToEvent<OnGUIAttribute>(ref OnGUI);
			SubscribeAttributeToEvent<OnDrawGizmosRuntimeAttribute>(ref OnDrawGizmosRuntime);
		}

		private static void SubscribeAttributeToEvent<A>(ref Action eventHandler) where A : Attribute
		{
			foreach(var m in ReflectionUtility.GetMethodsWithAttribute<A>(true))
			{
				var action = (Action)m.CreateDelegate(typeof(Action));
				eventHandler += action;
			}
			foreach(var m in ReflectionUtility.GetMethodsWithAttribute<A>(false))
			{
				Debug.LogError($"The Attribute '{typeof(A)}' is only valid on static methods ({m.DeclaringType}:{m.Name}). The method will not be invoked.");
			}
		}

		public static void AddSubsystemFirst(Type subSystemRoot, Type add, PlayerLoopSystem.UpdateFunction invocationTarget)
		{
			var loop = PlayerLoop.GetCurrentPlayerLoop();
			InsertSubsystem(ref loop, subSystemRoot, add, invocationTarget, null, true);
			PlayerLoop.SetPlayerLoop(loop);
		}

		public static void AddSubsystemLast(Type subSystemRoot, Type add, PlayerLoopSystem.UpdateFunction invocationTarget)
		{
			var loop = PlayerLoop.GetCurrentPlayerLoop();
			InsertSubsystem(ref loop, subSystemRoot, add, invocationTarget, null, false);
			PlayerLoop.SetPlayerLoop(loop);
		}

		public static void AddSubsystemBefore(Type subSystemRoot, Type add, PlayerLoopSystem.UpdateFunction invocationTarget, Type beforeSubSystem)
		{
			var loop = PlayerLoop.GetCurrentPlayerLoop();
			InsertSubsystem(ref loop, subSystemRoot, add, invocationTarget, beforeSubSystem, false);
			PlayerLoop.SetPlayerLoop(loop);
		}

		private static void InsertSubsystem(ref PlayerLoopSystem root, Type subSystemRoot, Type typeToAdd, PlayerLoopSystem.UpdateFunction invocationTarget, Type referenceSubSystem, bool before)
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
			Insert(ref sub, new PlayerLoopSystem() { updateDelegate = invocationTarget, type = typeToAdd }, referenceSubSystem, before);
			root.subSystemList[index] = sub;
		}

		private static void Insert(ref PlayerLoopSystem system, PlayerLoopSystem systemToAdd, Type reference, bool before)
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
				if(before)
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
				if(before)
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

		public static void InsertCallback(ref PlayerLoopSystem system, Type parentSystem, Type childSystem, PlayerLoopSystem.UpdateFunction callback)
		{
			bool added = false;
			for(int i = 0; i < system.subSystemList.Length; i++)
			{
				if(system.subSystemList[i].GetType() == parentSystem)
				{
					for(int j = 0; j < system.subSystemList[i].subSystemList.Length; j++)
					{
						if(system.subSystemList[i].subSystemList[j].GetType() == childSystem)
						{
							system.subSystemList[i].subSystemList[j].updateDelegate += callback;
							added = true;
						}
					}
				}
			}
			if(!added)
			{
				throw new NullReferenceException($"Failed to add callback to subsystem '{childSystem}'.");
			}
		}

		public static void PrintCurrentPlayerLoop(bool includeFunctionPtrs)
		{
			var sb = new StringBuilder();
			PrintLoopRecursive(sb, PlayerLoop.GetCurrentPlayerLoop(), 0, includeFunctionPtrs);
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
