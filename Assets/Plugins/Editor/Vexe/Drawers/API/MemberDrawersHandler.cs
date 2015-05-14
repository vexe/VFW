using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.Drawers;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor
{
    [InitializeOnLoad]
	public static class MemberDrawersHandler
	{
		static readonly Dictionary<int, List<BaseDrawer>> _cachedCompositeDrawers;
		static readonly Dictionary<int, BaseDrawer> _cachedMemberDrawers;
		static readonly Dictionary<int, MethodDrawer> _cachedMethodDrawers;

        static readonly HashSet<Type> _definesElementDrawingAttributes;

        public static readonly TypeDrawerMapper Mapper;

		public static readonly Func<Type, BaseDrawer> CachedGetObjectDrawer;

		static MemberDrawersHandler()
		{
            Mapper = new TypeDrawerMapper();
            Mapper.AddBuiltinTypes();

			_cachedMemberDrawers    = new Dictionary<int, BaseDrawer>();
			_cachedCompositeDrawers = new Dictionary<int, List<BaseDrawer>>();
			_cachedMethodDrawers    = new Dictionary<int, MethodDrawer>();

            _definesElementDrawingAttributes = new HashSet<Type>()
            {
                typeof(PerItemAttribute), typeof(PerKeyAttribute), typeof(PerValueAttribute)
            };

            //TODO: check if this is still needed
            CachedGetObjectDrawer = new Func<Type, BaseDrawer>(Mapper.GetDrawer).Memoize();
		}

		public static List<BaseDrawer> GetCompositeDrawers(EditorMember member, Attribute[] attributes)
		{
			List<BaseDrawer> drawers;
			if (_cachedCompositeDrawers.TryGetValue(member.Id, out drawers))
				return drawers;

			drawers = new List<BaseDrawer>();

			// consider composition only if the member type isn't a collection type,
			// or it is a collection type but it doesn't have any per attribute that signifies drawing per element
			// (in other words, the composition is applied on the collection itself, and not its elements)
			var considerComposition = !member.Type.IsCollection() || !attributes.Any(x => _definesElementDrawingAttributes.Contains(x.GetType()));
			if (considerComposition)
			{
				var compositeAttributes = attributes.OfType<CompositeAttribute>()
													.OrderBy(x => x.id)
													.ToList();

				for (int i = 0; i < compositeAttributes.Count; i++)
                {
                    var drawer = NewCompositeDrawer(compositeAttributes[i].GetType());
                    if (!drawer.CanHandle(member.Type))
                    {
                        Debug.LogError("Drawer {0} can't seem to handle type {1}. Make sure you're not applying attributes on the wrong members"
                             .FormatWith(drawer.GetType().GetNiceName(), member.TypeNiceName));
                        continue;
                    }
					drawers.Add(drawer);
                }
			}

			_cachedCompositeDrawers.Add(member.Id, drawers);
			return drawers;
		}

		public static BaseDrawer GetMemberDrawer(EditorMember member, Attribute[] attributes, bool ignoreAttributes)
		{
			BaseDrawer drawer;
			if (_cachedMemberDrawers.TryGetValue(member.Id, out drawer))
				return drawer;

			var canApplyDrawer = !member.Type.IsCollection() || !attributes.Any(x => _definesElementDrawingAttributes.Contains(x.GetType()));
            if (canApplyDrawer && !ignoreAttributes)
            {
                var drawingAttribute = attributes.GetAttribute<DrawnAttribute>();
                if (drawingAttribute != null)
                    drawer = NewDrawer(drawingAttribute.GetType());
            }

            if (drawer == null)
				drawer = NewObjectDrawer(member.Type);

            if (!drawer.CanHandle(member.Type))
            {
                Debug.LogError("Drawer `{0}` can't seem to handle type `{1}`. Make sure you're not applying attributes on the wrong members"
                    .FormatWith(drawer.GetType().GetNiceName(), member.TypeNiceName));

                drawer = Mapper.GetDrawer(member.Type);
            }

			_cachedMemberDrawers.Add(member.Id, drawer);

			return drawer;
		}

		public static MethodDrawer GetMethodDrawer(int methodId)
		{
			MethodDrawer drawer;
			if (!_cachedMethodDrawers.TryGetValue(methodId, out drawer))
				_cachedMethodDrawers.Add(methodId, drawer = new MethodDrawer());
			return drawer;
		}

		public static BaseDrawer NewObjectDrawer(Type objectType)
		{
            return Mapper.GetDrawer(objectType);
		}

		public static BaseDrawer NewCompositeDrawer(Type attributeType)
		{
            return Mapper.GetDrawer(attributeType);
		}

		public static BaseDrawer NewDrawer(Type attributeType)
		{
            return Mapper.GetDrawer(attributeType);
		}

		public static void ClearCache()
		{
			_cachedMemberDrawers.Clear();
			_cachedCompositeDrawers.Clear();
			_cachedMethodDrawers.Clear();
		}

		static class MenuItems
		{
			[MenuItem("Tools/Vexe/Debug/Clear drawers cache")]
			public static void ClearCache()
			{
				MemberDrawersHandler.ClearCache();
			}
		}
	}
}
