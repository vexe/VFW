//#define PROFILE
//#define DBG

using System;
using System.Collections.Generic;
using System.Reflection;
using Vexe.Editor.Drawers;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.GUIs
{
    public abstract partial class BaseGUI
    {
        static readonly Attribute[] Empty = new Attribute[0];

        public bool Member(MemberInfo info, object rawTarget, UnityObject unityTarget, int id, bool ignoreComposition)
        {
            EditorMember member;
            return Member(info, rawTarget, unityTarget, id, ignoreComposition, out member);
        }

        public bool Member(MemberInfo member, object rawTarget, UnityObject unityTarget, int id, bool ignoreComposition, out EditorMember wrappedMember)
        {
            if (member.MemberType == MemberTypes.Method)
            {
                var method = member as MethodInfo;
                var methodKey = Cache.GetMethodKey(Tuple.Create(id, method));
                var methodDrawer = MemberDrawersHandler.GetMethodDrawer(methodKey);
                methodDrawer.Initialize(method, rawTarget, unityTarget, methodKey, this);
                wrappedMember = null;
                return methodDrawer.OnGUI();
            }
            else
            {
                var cachedMember = Cache.GetMember(Tuple.Create(member, id));
                cachedMember.RawTarget = rawTarget;
                cachedMember.UnityTarget = unityTarget;
                wrappedMember = cachedMember;

                return Member(cachedMember, ignoreComposition);
            }
        }

        public bool Member(EditorMember member)
        {
            return Member(member, false);
        }

        public bool Member(EditorMember member, bool ignoreComposition)
        {
            return Member(member, member.Attributes, ignoreComposition);
        }

        public bool Member(EditorMember member, BaseDrawer memberDrawer)
        {
            return Member(member, member.Attributes, memberDrawer, false);
        }

        public bool Member(EditorMember member, Attribute[] attributes, bool ignoreComposition)
        {
            attributes = attributes ?? Empty;
            var memberDrawer = MemberDrawersHandler.GetMemberDrawer(member, attributes, ignoreComposition);
            memberDrawer.Initialize(member, attributes, this);
            return Member(member, attributes, memberDrawer, ignoreComposition);
        }

        public bool Member(EditorMember member, Attribute[] attributes, BaseDrawer memberDrawer, bool ignoreComposition)
        {
            List<BaseDrawer> composites = null;

            if (!ignoreComposition)
                composites = MemberDrawersHandler.GetCompositeDrawers(member, attributes);

            #if DBG
            Label(member.Id);
            Debug.Log("Got drawer " + memberDrawer.GetType().Name + " for member " + member.Name + " Key: " + handlerKey);
            #endif

            if (composites == null || composites.Count == 0)
            {
                #if PROFILE
                Profiler.BeginSample(memberDrawer.GetType().Name + " OnGUI (C)");
                #endif

                BeginCheck();
                {
                    memberDrawer.OnGUI();
                }
                #if PROFILE
                Profiler.EndSample();
                #endif
                return HasChanged();
            }

            for (int i = 0; i < composites.Count; i++)
                composites[i].Initialize(member, attributes, this);

            bool changed = false;

            #if PROFILE
            Profiler.BeginSample("OnUpperGUI " + member.Name);
            #endif
            for (int i = 0; i < composites.Count; i++)
                composites[i].OnUpperGUI();
            #if PROFILE
            Profiler.EndSample();
            #endif
            using (Horizontal())
            {
                #if PROFILE
                Profiler.BeginSample("OnLeftGUI " + member.Name);
                #endif
                for (int i = 0; i < composites.Count; i++)
                    composites[i].OnLeftGUI();
                #if PROFILE
                Profiler.EndSample();
                #endif
                using (Vertical())
                {
                    #if PROFILE
                    Profiler.BeginSample(memberDrawer.GetType().Name + " OnGUI");
                    #endif
                    BeginCheck();
                    {
                        memberDrawer.OnGUI();
                    }
                    #if PROFILE
                    Profiler.EndSample();
                    #endif
                    changed = HasChanged();

                    #if PROFILE
                    Profiler.BeginSample("OnMemberDrawn" + member.Name);
                    #endif
                    for (int i = 0; i < composites.Count; i++)
                        composites[i].OnMemberDrawn(LastRect);
                    #if PROFILE
                    Profiler.EndSample();
                    #endif
                }

                #if PROFILE
                Profiler.BeginSample("OnRightGUI " + member.Name);
                #endif
                for (int i = 0; i < composites.Count; i++)
                    composites[i].OnRightGUI();
                #if PROFILE
                Profiler.EndSample();
                #endif
            }

            #if PROFILE
            Profiler.BeginSample("OnLowerGUI " + member.Name);
            #endif
            for (int i = 0; i < composites.Count; i++)
                composites[i].OnLowerGUI();
            #if PROFILE
            Profiler.EndSample();
            #endif
            return changed;
        }

        private static class Cache
        {
            private static Func<Tuple<int, MethodInfo>, int> _getMethodKey;
            public static Func<Tuple<int, MethodInfo>, int> GetMethodKey
            {
                get
                {
                    //@Note: I expected the hash code for a MethodInfo to be persistent but it turns out I was wrong
                    //This was leading to the method foldout not being persistent between assembly reloads
                    //So I went for the method full name as a hash instead
                    return _getMethodKey ?? (_getMethodKey = new Func<Tuple<int, MethodInfo>, int>(x =>
                        RuntimeHelper.CombineHashCodes(x.Item1, x.Item2.GetFullName())).Memoize());
                }
            }

            private static Func<Tuple<MemberInfo, int>, EditorMember> _getMember;
            public static Func<Tuple<MemberInfo, int>, EditorMember> GetMember
            {
                get
                {
                    return _getMember ?? (_getMember = new Func<Tuple<MemberInfo, int>, EditorMember>(x =>
                        EditorMember.WrapMember(x.Item1, null, null, x.Item2)).Memoize());
                }
            }
        }
    }
}
