using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityEssentials
{
	[Flags]
	public enum SearchFlags
	{
		None = 0,
		Assemblies = 1 << 0,

		Classes = 1 << 1,
		Structs = 1 << 2,
		Types = Classes | Structs,

		Methods = 1 << 3,
		Properties = 1 << 4,
		Fields = 1 << 5,
		Events = 1 << 6,
		Members = Methods | Properties | Fields | Events,

		All = Assemblies | Types | Members
	}


	public class AttributeDefinition<T> where T : Attribute
	{
		public T Attribute { get; private set; }

		public Assembly TargetAssembly { get; private set; } = null;
		public Type TargetType { get; private set; } = null;
		public MemberInfo TargetMember { get; private set; } = null;

		public AttributeDefinition(T attribute, Assembly targetAssembly)
		{
			Attribute = attribute;
			TargetAssembly = targetAssembly;
		}

		public AttributeDefinition(T attribute, Type targetType)
		{
			Attribute = attribute;
			TargetType = targetType;
		}

		public AttributeDefinition(T attribute, MemberInfo targetMember)
		{
			Attribute = attribute;
			TargetMember = targetMember;
		}

		public override string ToString()
		{
			if(TargetMember != null)
			{
				return $"{Attribute.GetType().Name} in {TargetMember.DeclaringType.FullName}.{TargetMember.Name}";
			}
			else if(TargetType != null)
			{
				return $"{Attribute.GetType().Name} in {TargetType.FullName}";
			}
			else if(TargetAssembly != null)
			{
				return $"{Attribute.GetType().Name} in Assembly {TargetAssembly.FullName}";
			}
			else
			{
				return $"{Attribute.GetType().Name} in Unknown Target";
			}
		}
	}

	/// <summary>
	/// Utility class for common reflection functions.
	/// </summary>
	public static class ReflectionUtility
	{
		public const BindingFlags allInclusiveBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		private static readonly string[] excludedAssemblyPrefixesNoUnity = new string[]
		{
			"UnityEngine",
			"UnityEditor",
			"Unity.",
			"mscorlib",
			"System",
			"Mono.",
			"SyntaxTree",
			"netstandard"
		};

		private static readonly string[] excludedAssemblyPrefixesWithUnity = new string[]
		{
			"mscorlib",
			"System",
			"Mono.",
			"SyntaxTree",
			"netstandard"
		};

		private static Assembly[] assemblyCache;
		private static Assembly[] gameAssemblyCache;
		private static Assembly[] gameAssemblyWithUnityCache;

		private static Dictionary<Type, Type[]> interfaceCache = new Dictionary<Type, Type[]>();

		/// <summary>
		/// Returns all game related assemblies (excluding unity assemblies).
		/// </summary>
		public static Assembly[] GetGameAssemblies()
		{
			if(gameAssemblyCache == null)
			{
				gameAssemblyCache = GetAssembliesExcluding(excludedAssemblyPrefixesNoUnity);
			}
			return gameAssemblyCache;
		}

		/// <summary>
		/// Returns all game related and unity assemblies.
		/// </summary>
		public static Assembly[] GetGameAssembliesIncludingUnity()
		{
			if(gameAssemblyWithUnityCache == null)
			{
				gameAssemblyWithUnityCache = GetAssembliesExcluding(excludedAssemblyPrefixesWithUnity);
			}
			return gameAssemblyWithUnityCache;
		}

		/// <summary>
		/// Returns all interfaces implemented by the given type.
		/// </summary>
		public static Type[] GetInterfaces(Type type)
		{
			if(interfaceCache.TryGetValue(type, out var interfaces))
			{
				return interfaces;
			}
			else
			{
				interfaces = type.GetInterfaces();
				interfaceCache.Add(type, interfaces);
				return interfaces;
			}
		}

		private static bool ShouldIgnoreAssembly(Assembly assembly, string[] excludePrefixes)
		{
			foreach(var prefix in excludePrefixes)
			{
				if(assembly.FullName.StartsWith(prefix))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns all types that inherit from the given type.
		/// </summary>
		public static IEnumerable<Type> GetClassesOfType(Type baseType, bool includeUnityAssembly = false)
		{
			var types = new List<Type>();
			var assemblies = includeUnityAssembly ? GetGameAssembliesIncludingUnity() : GetGameAssemblies();
			foreach(var assembly in assemblies)
			{
				try
				{
					foreach(var t in assembly.GetTypes())
					{
						if(baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
						{
							types.Add(t);
						}
					}
				}
				catch(ReflectionTypeLoadException)
				{
					//Ignore this exception
				}
			}
			return types;
		}

		/// <summary>
		/// Returns all attributes of the given type.
		/// </summary>
		public static List<AttributeDefinition<T>> GetAllAttributeDefinitions<T>(SearchFlags searchFlags = SearchFlags.All) where T : Attribute
		{
			var condition = GetClassOrStructCondition(searchFlags);

			var defs = new List<AttributeDefinition<T>>();
			var assemblies = GetGameAssemblies();
			foreach(var assembly in assemblies)
			{
				try
				{
					if(searchFlags.HasFlag(SearchFlags.Assemblies))
					{
						foreach(var attr in assembly.GetCustomAttributes<T>())
						{
							defs.Add(new AttributeDefinition<T>(attr, assembly));
						}
					}
					if(searchFlags.HasFlag(SearchFlags.Classes)
						|| searchFlags.HasFlag(SearchFlags.Structs)
						|| searchFlags.HasFlag(SearchFlags.Properties)
						|| searchFlags.HasFlag(SearchFlags.Fields)
						|| searchFlags.HasFlag(SearchFlags.Methods)
						|| searchFlags.HasFlag(SearchFlags.Events)
						)
					{
						foreach(var t in assembly.GetTypes())
						{
							if(condition(t))
							{
								foreach(var attr in t.GetCustomAttributes<T>(false))
								{
									defs.Add(new AttributeDefinition<T>(attr, t));
								}
							}
							if(searchFlags.HasFlag(SearchFlags.Properties))
							{
								foreach(var p in t.GetProperties())
								{
									foreach(var attr in p.GetCustomAttributes<T>(false))
									{
										defs.Add(new AttributeDefinition<T>(attr, p));
									}
								}
							}
							if(searchFlags.HasFlag(SearchFlags.Fields))
							{
								foreach(var f in t.GetFields())
								{
									foreach(var attr in f.GetCustomAttributes<T>(false))
									{
										defs.Add(new AttributeDefinition<T>(attr, f));
									}
								}
							}
							if(searchFlags.HasFlag(SearchFlags.Methods))
							{
								foreach(var m in t.GetMethods())
								{
									foreach(var attr in m.GetCustomAttributes<T>(false))
									{
										defs.Add(new AttributeDefinition<T>(attr, m));
									}
								}
							}
							if(searchFlags.HasFlag(SearchFlags.Events))
							{
								foreach(var e in t.GetEvents())
								{
									foreach(var attr in e.GetCustomAttributes<T>(false))
									{
										defs.Add(new AttributeDefinition<T>(attr, e));
									}
								}
							}
						}
					}
					/*
					//Test if this is really needed
					foreach(var t in assembly.GetTypes())
					{
						if(condition(t))
						{
							foreach(var attr in t.GetCustomAttributes<T>(false))
							{
								defs.Add(new AttributeDefinition<T>(attr, t));
							}
							foreach(var m in t.GetMembers())
							{
								foreach(var attr in m.GetCustomAttributes<T>(false))
								{
									defs.Add(new AttributeDefinition<T>(attr, m));
								}
							}
						}
					}
					*/
				}
				catch(ReflectionTypeLoadException)
				{
					//Ignore this exception
				}
			}
			return defs;
		}

		/// <summary>
		/// Returns all methods with attribute T.
		/// </summary>
		/// <typeparam name="T">The type of attribute to search for.</typeparam>
		/// <param name="staticMethods"><c>true</c> to list <see langword="static"/> methods, <c>false</c> to list instance methods.</param>
		/// <param name="includeNonPublic">Whether to include private/protected/internal methods in the returned list.</param>
		public static List<MethodInfo> GetMethodsWithAttribute<T>(bool staticMethods, bool includeNonPublic = true, bool includeStructs = true) where T : Attribute
		{
			var condition = GetClassOrStructCondition(includeStructs ? SearchFlags.Classes | SearchFlags.Structs : SearchFlags.Classes);
			var bindingFlags = GetBindingFlags(staticMethods, includeNonPublic);

			var methods = new List<MethodInfo>();
			foreach(var assembly in GetGameAssemblies())
			{
				try
				{
					methods.AddRange(assembly.GetTypes() // returns all types defined in this assembly
						.Where(condition) // classes only
						.SelectMany(x => x.GetMethods(bindingFlags)) // returns all methods
						.Where(x => x.GetCustomAttributes(typeof(T), false).FirstOrDefault() != null) // returns only methods that have the Attribute
					);
				}
				catch(ReflectionTypeLoadException)
				{
					//Ignore this exception
				}
			}
			
			return methods;
		}

		/// <summary>
		/// Returns all classes and assembly attributes of the given type.
		/// </summary>
		public static List<AttributeDefinition<T>> GetClassAndAssemblyAttributes<T>(bool includeUnityAssembly) where T : Attribute
		{
			var assemblies = includeUnityAssembly ? GetGameAssembliesIncludingUnity() : GetGameAssemblies();
			var defs = new List<AttributeDefinition<T>>();
			foreach(var assembly in assemblies)
			{
				try
				{
					foreach(var attr in assembly.GetCustomAttributes<T>())
					{
						defs.Add(new AttributeDefinition<T>(attr, assembly));
					}
					foreach(var t in assembly.GetTypes())
					{
						foreach(var attr in t.GetCustomAttributes<T>(false))
						{
							defs.Add(new AttributeDefinition<T>(attr, t));
						}
					}
				}
				catch(ReflectionTypeLoadException)
				{
					//Ignore this exception
				}
			}
			return defs;
		}

		/// <summary>
		/// Returns the MemberInfo located at the given path (separated by dots), starting at the given type.
		/// </summary>
		public static MemberInfo FindMemberInType(Type type, string name, bool throwException = false)
		{
			var members = type.GetMember(name, allInclusiveBindingFlags);
			if(members.Length > 0)
			{
				return members[0];
			}
			else if(type.BaseType != null)
			{
				return FindMemberInType(type.BaseType, name);
			}
			else
			{
				if(throwException) throw new NullReferenceException($"Member '{name}' not found in type '{type.Name}'.");
				return null;
			}
		}

		/// <summary>
		/// Returns a member value via name.
		/// </summary>
		public static object GetMemberValueByName(object source, string name)
		{
			var m = FindMemberInType(source.GetType(), name);
			if(m != null)
			{
				return GetMemberValue(m, source);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Returns the value of the given member.
		/// </summary>
		public static object GetMemberValue(MemberInfo m, object obj)
		{
			if(m is FieldInfo f)
			{
				return f.GetValue(obj);
			}
			else if(m is PropertyInfo p)
			{
				return p.GetValue(obj);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Invokes all static methods with the given attribute.
		/// </summary>
		public static void InvokeStaticMethodsWithAttribute<T>(bool allowParameterlessMethods = true, params object[] parameters) where T : Attribute
		{
			InvokeStaticMethods(GetMethodsWithAttribute<T>(true), allowParameterlessMethods, parameters);
		}

		/// <summary>
		/// Invokes all static methods in the given collection.
		/// </summary>
		public static void InvokeStaticMethods(IEnumerable<MethodInfo> methods, bool allowParameterlessMethods, params object[] parameters)
		{
			if(parameters == null) parameters = new object[0];
			foreach(var m in methods)
			{
				try
				{
					if(!m.IsStatic)
					{
						throw new MethodAccessException($"Method '{m.DeclaringType.FullName}.{m.Name}' is not static.");
					}
					if(allowParameterlessMethods)
					{
						int paramCount = m.GetParameters().Length;
						if(paramCount > 0 && paramCount != parameters.Length)
						{
							throw new MethodAccessException($"Method '{m.DeclaringType.FullName}.{m.Name}' cannot be called due to parameter count mismatch.");
						}
						try
						{
							m.Invoke(null, paramCount == 0 ? null : parameters);
						}
						catch(Exception e)
						{
							Debug.LogException(e);
						}
					}
					else
					{
						if(m.GetParameters().Length != parameters.Length)
						{
							throw new MethodAccessException($"Method '{m.DeclaringType.FullName}.{m.Name}' cannot be called due to parameter count mismatch.");
						}
						try
						{
							m.Invoke(null, parameters);
						}
						catch(Exception e)
						{
							Debug.LogException(e);
						}
					}
				}
				catch(Exception e)
				{
					e.LogException($"Failed to invoke method '{m.DeclaringType.Name}.{m.Name}'");
				}
			}
		}

		/// <summary>
		/// Sets the value for the member at the given path (separated by dots).
		/// </summary>
		public static void SetValueAtPath(object root, MemberInfo[] path, object value)
		{
			var obj = root;
			Resolve(ref obj, path, 0, (o) =>
			{
				SetValueOfMember(o, path[path.Length - 1], value);
			}, true);
		}

		/// <summary>
		/// Adds the value for the member at the given path (separated by dots).
		/// </summary>
		public static void AddValueAtPath(object root, MemberInfo[] path, object value)
		{
			var obj = root;
			Resolve(ref obj, path, 0, (o) =>
			{
				var previous = GetValueOfMember(o, path[path.Length - 1]);
				value = AddValues(previous, value);
				SetValueOfMember(o, path[path.Length - 1], value);
			}, true);
		}

		/// <summary>
		/// Returns the value for the member at the given path (separated by dots).
		/// </summary>
		public static object GetValueAtPath(object root, MemberInfo[] path)
		{
			var obj = root;
			Resolve(ref obj, path, 0, (o) =>
			{
				obj = GetValueOfMember(obj, path[path.Length - 1]);
			}, false);
			return obj;
		}

		/// <summary>
		/// Sets the value of the given member.
		/// </summary>
		public static void SetValueOfMember(object obj, MemberInfo member, object value)
		{
			if(member is FieldInfo f) f.SetValue(obj, value);
			else if(member is PropertyInfo p) p.SetValue(obj, value);
			else throw new InvalidOperationException();
		}

		/// <summary>
		/// Returns the value of the given member.
		/// </summary>
		public static object GetValueOfMember(object obj, MemberInfo member)
		{
			if(member == null) throw new ArgumentNullException("member");
			if(member is FieldInfo f) return f.GetValue(obj);
			else if(member is PropertyInfo p) return p.GetValue(obj);
			else throw new InvalidOperationException("Invalid member type: " + member.GetType().Name);
		}

		/// <summary>
		/// Returns the sum of the two values.
		/// </summary>
		public static object AddValues(object a, object b)
		{
			if(a is float f) return f + (float)b;
			else if(a is int i) return i + (int)b;
			else if(a is Vector2 v2) return v2 + (Vector2)b;
			else if(a is Vector3 v3) return v3 + (Vector3)b;
			else if(a is Vector4 v4) return v4 + (Vector4)b;
			else if(a is short s) return s + (short)b;
			else if(a is long l) return l + (long)b;
			else if(a is double d) return d + (double)b;
			else throw new InvalidOperationException("The given type does not support addition.");
		}

		/// <summary>
		/// Return the path to the given member as an array of MemberInfos.
		/// </summary>
		public static MemberInfo[] GetMemberPath(object root, string path)
		{
			object obj = root;
			string[] pathParts = path.Split('.');
			var memberPath = new MemberInfo[pathParts.Length];
			for(int i = 0; i < pathParts.Length; i++)
			{
				memberPath[i] = FindMemberInType(obj.GetType(), pathParts[i]);
				obj = GetValueOfMember(obj, memberPath[i]);
			}
			return memberPath;
		}

		private static Assembly[] GetAssembliesExcluding(string[] excludePrefixes)
		{
			if(assemblyCache == null)
			{
				assemblyCache = AppDomain.CurrentDomain.GetAssemblies();
			}

			var list = new List<Assembly>();
			foreach(var assembly in assemblyCache)
			{
				if(!ShouldIgnoreAssembly(assembly, excludePrefixes))
				{
					list.Add(assembly);
				}
			}
			return list.ToArray();
		}

		private static void Resolve(ref object obj, MemberInfo[] path, int index, Action<object> action, bool set)
		{
			while(index < path.Length - 1)
			{
				var memberInfo = path[index];
				var lObj = obj;
				obj = GetValueOfMember(obj, memberInfo);
				if(obj.GetType().IsValueType)
				{
					Resolve(ref obj, path, index + 1, action, set);
					if(set)
					{
						SetValueOfMember(lObj, memberInfo, obj);
					}
					return;
				}
				else
				{
					index++;
				}
			}
			action?.Invoke(obj);
		}

		private static Func<Type, bool> GetClassOrStructCondition(SearchFlags flags)
		{
			if(flags.HasFlag(SearchFlags.Types))
			{
				return x => x.IsClass || x.IsValueType;
			}
			else if(flags.HasFlag(SearchFlags.Classes))
			{
				return x => x.IsClass;
			}
			else if(flags.HasFlag(SearchFlags.Structs))
			{
				return x => x.IsValueType;
			}
			else
			{
				return x => false;
			}
		}

		private static BindingFlags GetBindingFlags(bool staticOnly, bool inclideNonPublic)
		{
			var flags = BindingFlags.Public;
			flags |= staticOnly ? BindingFlags.Static : BindingFlags.Instance;
			if(inclideNonPublic) flags |= BindingFlags.NonPublic;
			return flags;
		}
	}
}