//#define dbg

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Serialization;
using UnityObject = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using System.Text.RegularExpressions;
#endif

namespace Vexe.Runtime.Types
{ 
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class HasRequirementsAttribute : Attribute
	{
	}

	/// <summary>
	/// A system that could automatically resolve requirements (dependencies) for your objects using attributes that tell it how to resolve those requirements
	/// Annotate your object (System.Object or BetterBehaviour) with [HasRequirements] to let the system know that this object has requirements and needs them resolved
	/// The system could be triggered automatically on editor update in a certain interval specified in Startup.ResolveInterval (see below) - defaults to 1 sec
	/// or manually (use Tools/Vexe/Requirements to toggle between auto/manual)
	/// To resolve requirements manually, you have to collapse the "Script" header foldout of your behavior, and click the resolve button (the button won't appear if your object isn't annotated with HasRequirements)
	/// PS: the system picks up serializble members only
	/// </summary>
	public static class Requirements
	{
		/// <summary>
		/// Resolves the whole scene by getting all BetterBehaviours that are tagged with HasRequirements
		/// </summary>
		public static void ResolveScene()
		{
#if dbg
			Debug.Log("resolving scene");
#endif
			var behaviours = UnityObject.FindObjectsOfType<BetterBehaviour>();
			for (int i = 0; i < behaviours.Length; i++)
			{
				var x = behaviours[i];
				if (x.GetType().IsDefined<HasRequirementsAttribute>(true))
					Resolve(x, x.gameObject);
			}
		}

		public static void Resolve(object target, GameObject gameObject)
		{
#if dbg
			Debug.Log("resovling " + target);
#endif
			var members = VFWSerializationLogic.Instance.CachedGetSerializableMembers(target.GetType());
			for (int i = 0; i < members.Length; i++)
			{
				var member = members[i];
				member.Target = target;

				if (!member.Type.IsA<UnityObject>())
				{
					if (member.Type.IsDefined<HasRequirementsAttribute>(true))
					{ 
#if dbg
						Debug.Log("recursively resolving non-UnityObject " + member.Type.Name + " in " + target);
#endif
						if (member.Value != null)
							Resolve(member.Value, gameObject);
					}
					continue;
				}

				var req = member.Info.GetCustomAttribute<RequiredAttribute>();
				if (req == null)
					continue;

				if (!member.Value.IsObjectNull())
				{ 
#if dbg
					Debug.Log("ignoring member " + member.Name + " cause it's not null. no need to resolve");
#endif
					continue;
				}

				Func<object, GameObject, RuntimeMember, RequiredAttribute, UnityObject> resolver;
				if (!resolvers.TryGetValue(req.GetType(), out resolver))
				{
#if dbg
					Debug.Log("No requirement resolver found for attribute type: " + req.GetType().Name);
#endif
					continue;
				}

				member.Value = resolver.Invoke(target, gameObject, member, req);
			}
		}

		private static readonly Dictionary<Type, Func<object, GameObject, RuntimeMember, RequiredAttribute, UnityObject>> resolvers =
			new Dictionary<Type, Func<object, GameObject, RuntimeMember, RequiredAttribute, UnityObject>>()
		{
			{ typeof(RequiredAttribute), DontResolve },
			{ typeof(RequiredSingleAttribute), ResolveSingle },
			{ typeof(RequiredFromThisAttribute), ResolveFromTargetGO },
			{ typeof(RequiredFromChildrenAttribute), ResolveFromTargetChildren },
			{ typeof(RequiredFromParentsAttribute), ResolveFromTargetParents },
		};

		private static UnityObject DontResolve(object target, GameObject gameObject, RuntimeMember member, RequiredAttribute attribute)
		{
			return null;
		}

		private static UnityObject ResolveSingle(object target, GameObject gameObject, RuntimeMember member, RequiredAttribute attribute)
		{
			var single = (attribute as RequiredSingleAttribute).FromResources ?
				Resources.FindObjectsOfTypeAll(member.Type).FirstOrDefault() :
				UnityObject.FindObjectOfType(member.Type);
#if dbg
			Debug.Log("found single " + single + " for " + memberType.Name + " in " + target);
#endif
			return single;
		}

		private static UnityObject ResolveFromTargetGO(object target, GameObject gameObject, RuntimeMember member, RequiredAttribute attribute)
		{
			if (member.Type == typeof(GameObject))
			{
				return gameObject;
			}

			var c = gameObject.GetComponent(member.Type);
			if (c == null)
			{
				if ((attribute as RequiredFromThisAttribute).Add)
				{ 
#if dbg
					Debug.Log("adding component " + memberType.Name + " to " + gameObject.name);
#endif
					c = gameObject.AddComponent(member.Type);
				}
			}
			return c;
		}

		private static UnityObject ResolveFromTargetChildren(object target, GameObject gameObject, RuntimeMember member, RequiredAttribute attribute)
		{
			return ResolveFromTargetRelative(target, gameObject, member, attribute as RequiredFromRelativeAttribute,
				gameObject.GetOrAddChildAtPath, gameObject.GetChildAtPath, gameObject.GetComponentInChildren);
		}

		private static UnityObject ResolveFromTargetParents(object target, GameObject gameObject, RuntimeMember member, RequiredAttribute attribute)
		{
			return ResolveFromTargetRelative(target, gameObject, member, attribute as RequiredFromRelativeAttribute,
				gameObject.GetOrAddParentAtPath, gameObject.GetParentAtPath, gameObject.GetComponentInParent);
		}


		private static UnityObject ResolveFromTargetRelative(object target, GameObject gameObject, RuntimeMember member, RequiredFromRelativeAttribute attribute,
			Func<string, GameObject> GetOrAddRelativeAtPath,
			Func<string, GameObject> GetRelativeAtPath,
			Func<Type, Component> GetComponentInRelative)
		{
			if (member.Type == typeof(GameObject))
			{
				var path = ProcessPath(attribute.Path, member.Name);
				if (path.IsNullOrEmpty())
					return null;

				if (attribute.Create)
					return GetOrAddRelativeAtPath(path);

				try { return GetRelativeAtPath(path); }
				catch { return null; }
			}
			else
			{
				Component c = null;
				var path = attribute.Path;
				if (path.IsNullOrEmpty())
				{
					c = GetComponentInRelative(member.Type);
				}
				else
				{
					path = ProcessPath(attribute.Path, member.Name);

					GameObject relative;

					if (attribute.Create)
						relative = GetOrAddRelativeAtPath(path);
					else
					{
						try { relative = GetRelativeAtPath(path); }
						catch { relative = null; }
					}

					if (relative == null)
						return null;

					c = relative.GetComponent(member.Type);
					if (c == null && attribute.Add)
					{
						if (member.Type.IsAbstract)
							Debug.Log("Can't add component `" + member.Type.Name + "` because it's abstract");
						else
							c = relative.AddComponent(member.Type);
					}
				}
				return c;
			}
		}

		private static string ProcessPath(string path, string memberName)
		{
			if (path == "$Name")
			{
				return memberName.SplitPascalCase().Split(' ')[0];
			}
			if (path == "$name")
			{
				return memberName.SplitCamelCase().Split(' ')[0];
			}
			if (path == "$FullName")
			{
				return memberName.ToUpperAt(0);
			}
			if (path == "$fullName")
			{
				return memberName;
			}
			if (path == "$Full Name")
			{
				return memberName.SplitPascalCase();
			}
			return path;
		}

#if UNITY_EDITOR
		[InitializeOnLoad]
		public static class Startup
		{
			public static double ResolveInterval = 1.0;

			static double nextTime;
			static int kReqAuto = "Requirements_Auto".GetHashCode();

			static Startup()
			{
#if dbg
				Debug.Log("Requirements::Startup");
#endif
				EditorApplication.update += ResolveTick;
			}

			static void ResolveTick()
			{
				if (!BetterPrefs.GetEditorInstance().Bools.ValueOrDefault(kReqAuto))
					return;

				if (EditorApplication.timeSinceStartup > nextTime)
				{
					Requirements.ResolveScene();
					nextTime = EditorApplication.timeSinceStartup + ResolveInterval;
				}
			}

			public static class MenuItems
			{
				[MenuItem("Tools/Vexe/Requirements/ResolveScene")]
				public static void ResolveSceneRequirments()
				{
					Profiler.BeginSample("ResolveSceneReq");
					Requirements.ResolveScene();
					Profiler.EndSample();
				}

				[MenuItem("Tools/Vexe/Requirements/Manual")]
				public static void Manual()
				{
					BetterPrefs.GetEditorInstance().Bools[kReqAuto] = false;
				}

				[MenuItem("Tools/Vexe/Requirements/Automatic")]
				public static void Automatic()
				{
					BetterPrefs.GetEditorInstance().Bools[kReqAuto] = true;
				}
			}
		}
#endif
	}
}
