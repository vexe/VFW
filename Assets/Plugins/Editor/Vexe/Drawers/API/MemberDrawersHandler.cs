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

        public static readonly TypeDrawerMapper Mapper;

		public static readonly Func<Type, BaseDrawer> CachedGetObjectDrawer;

		static MemberDrawersHandler()
		{
            Mapper = new TypeDrawerMapper();
            Mapper.AddBuiltinTypes();

			_cachedMemberDrawers    = new Dictionary<int, BaseDrawer>();
			_cachedCompositeDrawers = new Dictionary<int, List<BaseDrawer>>();
			_cachedMethodDrawers    = new Dictionary<int, MethodDrawer>();

            //TODO: check if this is still needed
            CachedGetObjectDrawer = new Func<Type, BaseDrawer>(Mapper.GetDrawer).Memoize();
		}

		public static List<BaseDrawer> GetCompositeDrawers(EditorMember member, Attribute[] attributes)
		{
			List<BaseDrawer> drawers;
			if (_cachedCompositeDrawers.TryGetValue(member.Id, out drawers))
				return drawers;

			drawers = new List<BaseDrawer>();

            var applied = GetAppliedAttributes(member.Type, attributes);
			if (applied != null)
			{
				var compositeAttributes = applied.OfType<CompositeAttribute>()
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

            if (!ignoreAttributes)
            {
                var applied = GetAppliedAttributes(member.Type, attributes);
                if (applied != null)
                { 
                    var drawingAttribute = applied.GetAttribute<DrawnAttribute>();
                    if (drawingAttribute != null)
                        drawer = NewDrawer(drawingAttribute.GetType());
                }
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

        public static bool IsApplicableAttribute(Type memberType, Attribute attribute, Attribute[] attributes)
        {
            var applied = GetAppliedAttributes(memberType, attributes);
            if (applied == null)
                return false;
            return applied.ContainsValue(attribute);
        }

        private static Attribute[] GetAppliedAttributes(Type memberType, Attribute[] attributes)
        {
            if (!memberType.IsCollection())
                return attributes;

            PerKeyAttribute perKey = null;
            PerValueAttribute perValue = null;

            var isDictionary = memberType.IsImplementerOfRawGeneric(typeof(IDictionary<,>));

            for (int i = 0; i < attributes.Length; i++)
            {
                var attr = attributes[i];
                var perItem = attr as PerItemAttribute;
                if (perItem != null)
                {
                    if (isDictionary)
                    {
                        Debug.LogError("PerItem should be applied on lists or arrays, not dictionaries!");
                        return null;
                    }

                    if (perItem.ExplicitAttributes == null)
                        return null;

                    return attributes.Where(x => !perItem.ExplicitAttributes.Contains(x.GetType().Name.Replace("Attribute", ""))).ToArray();
                }

                if (!isDictionary)
                    continue;

                if (perKey != null && perValue != null)
                    break;

                if (perKey == null)
                    perKey = attr as PerKeyAttribute;

                if (perValue == null)
                    perValue = attr as PerValueAttribute;
            }

            if (perKey == null && perValue == null)
                return attributes;

            if (perKey != null && perValue != null &&
                (perKey.ExplicitAttributes == null || perValue.ExplicitAttributes == null))
            {
                Debug.LogError("Confusion: If you use both PerKey and PerValue, then you should explictly mention which attributes are applied per key and which per value! (Info: MemberType ({0}) Attributes({1})".FormatWith(memberType.GetNiceName(), string.Join(", ", attributes.Select(x=>x.GetType().Name).ToArray())));
                return null;
            }

            if (perKey != null)
            {
                if (perKey.ExplicitAttributes != null)
                    attributes = attributes.Where(x => !perKey.ExplicitAttributes.Contains(x.GetType().Name.Replace("Attribute", ""))).ToArray();
                else
                    attributes = new Attribute[] { perKey };
            }

            if (perValue != null)
            {
                if (perValue.ExplicitAttributes != null)
                    attributes = attributes.Where(x => !perValue.ExplicitAttributes.Contains(x.GetType().Name.Replace("Attribute", ""))).ToArray();
                else 
                    attributes = new Attribute[] { perValue };
            }

            return attributes;
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
