using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.Drawers;
using Vexe.Editor.GUIs;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor
{
	public class MemberDrawersHandler
	{
		private readonly Type[] objectDrawerTypes;
		private readonly Type[] attributeDrawerTypes;
		private readonly Type[] compositeDrawerTypes;
		private readonly Dictionary<int, List<BaseDrawer>> cachedCompositeDrawers;
		private readonly Dictionary<int, BaseDrawer> cachedMemberDrawers;
		private readonly Dictionary<int, MethodDrawer> cachedMethodDrawers;
		private readonly Type fallbackDrawerType;

		public static Func<Type, BaseDrawer> GetCachedObjectDrawer;

		public static readonly MemberDrawersHandler Instance = new MemberDrawersHandler();

		public MemberDrawersHandler()
		{
			cachedMemberDrawers    = new Dictionary<int, BaseDrawer>();
			cachedCompositeDrawers = new Dictionary<int, List<BaseDrawer>>();
			cachedMethodDrawers    = new Dictionary<int, MethodDrawer>();

			fallbackDrawerType = typeof(RecursiveDrawer);

			Type[] drawerTypes = AppDomain.CurrentDomain.GetAssemblies()
														.SelectMany(x => x.GetTypes())
														.Where(t => t.IsA<BaseDrawer>())
												   		.Where(t => !t.IsAbstract)
												   		.ToArray();

			compositeDrawerTypes = drawerTypes.Where(t => t.IsSubclassOfRawGeneric(typeof(CompositeDrawer<,>)))
											  .ToArray();

			attributeDrawerTypes = drawerTypes.Where(t => t.IsSubclassOfRawGeneric(typeof(AttributeDrawer<,>)))
											  .ToArray();

			objectDrawerTypes = drawerTypes.Except(attributeDrawerTypes)
										   .Disinclude(fallbackDrawerType)
										   .Where(t => t.IsSubclassOfRawGeneric(typeof(ObjectDrawer<>)))
										   .ToArray();

            GetCachedObjectDrawer = new Func<Type, BaseDrawer>(type =>
                GetDrawerForType(type, objectDrawerTypes, typeof(ObjectDrawer<>))).Memoize();
		}

		public List<BaseDrawer> GetCompositeDrawers(EditorMember member, Attribute[] attributes, BaseGUI gui)
		{
			List<BaseDrawer> drawers;
			if (cachedCompositeDrawers.TryGetValue(member.Id, out drawers))
				return drawers;

			drawers = new List<BaseDrawer>();

			var memberType = member.Type;

			// consider composition only if coreOnly was false, and the member type isn't a collection type,
			// or it is a collection type but it doesn't have any per attribute that signifies drawing per element
			// (in other words, the composition is applied on the collection itself, and not its elements)
			var considerComposition = !memberType.IsCollection() || !attributes.AnyDefined<DefinesElementDrawingAttribute>();

			if (considerComposition)
			{
				var compositeAttributes = attributes.OfType<CompositeAttribute>()
													.OrderBy(x => x.id)
													.ToList();

				for (int i = 0; i < compositeAttributes.Count; i++)
					drawers.AddIfNotNull(GetCompositeDrawer(memberType, compositeAttributes[i].GetType()));
			}

			cachedCompositeDrawers.Add(member.Id, drawers);
			return drawers;
		}

		public BaseDrawer GetMemberDrawer(EditorMember member, Attribute[] attributes)
		{
			BaseDrawer drawer;
			if (cachedMemberDrawers.TryGetValue(member.Id, out drawer))
				return drawer;

			var memberType = member.Type;

			// check attribute drawer first
			var drawingAttribute = attributes.GetAttribute<DrawnAttribute>();
			if (drawingAttribute != null)
				drawer = GetAttributeDrawer(memberType, drawingAttribute.GetType());

			// if still null get an object drawer
			if (drawer == null)
				drawer = GetObjectDrawer(memberType);

			cachedMemberDrawers.Add(member.Id, drawer);
			
			return drawer;
		}

		public BaseDrawer UpdateMemberDrawer(Type newObjType, Type newDrawerType, int id)
		{
			BaseDrawer drawer;
			if (cachedMemberDrawers.TryGetValue(id, out drawer))
			{
				if (drawer.GetType() == newDrawerType)
					return drawer;
			}
			drawer = GetObjectDrawer(newObjType);
			cachedMemberDrawers[id] = drawer;
			return drawer;
		}

		public MethodDrawer GetMethodDrawer(int methodId)
		{
			MethodDrawer drawer;
			if (!cachedMethodDrawers.TryGetValue(methodId, out drawer))
				cachedMethodDrawers.Add(methodId, drawer = new MethodDrawer());
			return drawer;
		}

		public BaseDrawer GetObjectDrawer(Type objectType)
		{
			return GetDrawerForType(objectType, objectDrawerTypes, typeof(ObjectDrawer<>));
		}

		public BaseDrawer GetCompositeDrawer(Type objectType, Type attributeType)
		{
			return GetDrawerForPair(objectType, attributeType, compositeDrawerTypes, typeof(CompositeDrawer<,>));
		}

		public BaseDrawer GetAttributeDrawer(Type objectType, Type attributeType)
		{
			return GetDrawerForPair(objectType, attributeType, attributeDrawerTypes, typeof(AttributeDrawer<,>));
		}

		private BaseDrawer ResolveDrawerFromTypes(Type objectType, Type drawerType, Type drawerGenArgType)
		{
            if (objectType.IsSubTypeOfRawGeneric(typeof(IDictionary<,>)))
            {
                var args = new List<Type>(objectType.GetGenericArgsInThisOrAbove());
                args.Insert(0, objectType);
                return BaseDrawer.Create(typeof(DictionaryDrawer<,,>).MakeGenericType(args.ToArray()));
            }

            if (objectType.IsSubTypeOfRawGeneric(typeof(List<>)))
            {
                var elementType = objectType.GetGenericArgsInThisOrAbove()[0];
                return BaseDrawer.Create(typeof(ListDrawer<>).MakeGenericType(elementType));
            }

			if (objectType.IsArray)
			{
                var elementType = objectType.GetElementType();
				return BaseDrawer.Create(typeof(ArrayDrawer<>).MakeGenericType(elementType));
			}

			if (objectType.IsA(drawerGenArgType))
			{
				return BaseDrawer.Create(drawerType);
			}

			if (drawerGenArgType.IsGenericType)
			{
				if (objectType.IsSubTypeOfRawGeneric(drawerGenArgType.GetGenericTypeDefinition()))
				{
                    var args = objectType.GetGenericArgsInThisOrAbove();
                    return BaseDrawer.Create(drawerType.MakeGenericType(args));
				}
			}
			else if (!drawerGenArgType.IsConstructedGenType())
			{
				var args = drawerType.GetGenericArguments();
				if (args.Length == 1 && args[0].IsGenericTypeDefinition)
					return BaseDrawer.Create(drawerType.MakeGenericType(objectType));
			}

			return null;
		}

		private BaseDrawer GetDrawerForType(Type objectType, Type[] typeCache, Type baseDrawerType)
		{
			for (int i = 0; i < typeCache.Length; i++)
			{
				var drawerType = typeCache[i];

				var ignoreFor = drawerType.GetCustomAttribute<IgnoreOnTypes>();
				if (ignoreFor != null)
				{
					var forTypes = ignoreFor.types;
					if (forTypes.Any(x => objectType.IsSubclassOfRawGeneric(x)))
						continue;
				}

				var firstGen = drawerType.GetParentGenericArguments(baseDrawerType)[0];
				var drawer = ResolveDrawerFromTypes(objectType, drawerType, firstGen);
				if (drawer != null)
					return drawer;
			}

			return BaseDrawer.Create(fallbackDrawerType);
		}

		private BaseDrawer GetDrawerForPair(Type objectType, Type attributeType, Type[] typeCache, Type baseDrawerType)
		{
			for (int i = 0; i < typeCache.Length; i++)
			{
				var drawerType = typeCache[i];

				var ignoreFor = drawerType.GetCustomAttribute<IgnoreOnTypes>();
				if (ignoreFor != null)
				{
					var forTypes = ignoreFor.types;
					if (forTypes.Any(x => objectType.IsSubclassOfRawGeneric(x)))
						continue;
				}

				var args = drawerType.GetParentGenericArguments(baseDrawerType);

				if (attributeType == args[1])
				{
					var drawer = ResolveDrawerFromTypes(objectType, drawerType, args[0]);
					if (drawer != null)
						return drawer;
				}
				else if (args[1].IsGenericParameter)
				{
					var constraints = args[1].GetGenericParameterConstraints();
					if (constraints.Length == 1 && attributeType.IsA(constraints[0]))
						return BaseDrawer.Create(drawerType.MakeGenericType(attributeType));
				}
			}

			return null;
		}

		public void ClearCache()
		{
			cachedMemberDrawers.Clear();
			cachedCompositeDrawers.Clear();
			cachedMethodDrawers.Clear();
		}

		private static class MenuItems
		{
			[MenuItem("Tools/Vexe/BetterBehaviour/Clear drawers cache")]
			public static void ClearCache()
			{
				Instance.ClearCache();;
			}
		}
	}
}