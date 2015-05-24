using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Internal
{ 
    public static class ConditionalVisibility
    {
        static Dictionary<MemberInfo, MethodCaller<object, bool>> _isVisibleCache = new Dictionary<MemberInfo, MethodCaller<object, bool>>();

        public static bool IsVisible(MemberInfo member, object target)
        {
            MethodCaller<object, bool> isVisible;
            if (!_isVisibleCache.TryGetValue(member, out isVisible))
            {
                //Debug.Log("Building delegate for conditionally visible member: " + member.Name);

                var attr = member.GetCustomAttribute<VisibleWhenAttribute>();
                if (attr == null)
                { 
                    _isVisibleCache[member] = AlwaysVisible;
                    return true;
                }

                var targetType = target.GetType();
                var conditionMemberNames = attr.ConditionMembers;
                var conditions = new List<MethodCaller<object, bool>>(conditionMemberNames.Length);

                for (int i = 0; i < conditionMemberNames.Length; i++)
                {
                    var conditionMemberName = conditionMemberNames[i];

                    if (string.IsNullOrEmpty(conditionMemberName))
                    {
                        Debug.Log("Empty condition is used in VisibleWhen annotated on member: " + member.Name);
                        continue;
                    }

                    bool negate = conditionMemberName[0] == '!';
                    if (negate)
                        conditionMemberName = conditionMemberName.Remove(0, 1);

                    var conditionMember = targetType.GetMemberFromAll(conditionMemberName, Flags.StaticInstanceAnyVisibility);
                    if (conditionMember == null)
                    {
                        Debug.Log("Member not found: " + conditionMemberName);
                        _isVisibleCache[conditionMember] = AlwaysVisible;
                        return true;
                    }

                    Assert.True(attr.Operator == '|' || attr.Operator == '&',
                        "Only AND ('&') and OR ('|') operators are supported");

                    MethodCaller<object, bool> condition = null;
                    switch(conditionMember.MemberType)
                    {
                        case MemberTypes.Field:
                            // I feel like there should be a shorter way of doing this...
                            if (negate)
                                condition = (x, y) => !(bool)(conditionMember as FieldInfo).GetValue(x);
                            else 
                                condition = (x, y) => (bool)(conditionMember as FieldInfo).GetValue(x);
                            break;
                        case MemberTypes.Property:
                            if (negate)
                                condition = (x, y) => !(bool)(conditionMember as PropertyInfo).GetValue(x, null);
                            else 
                                condition = (x, y) => (bool)(conditionMember as PropertyInfo).GetValue(x, null);
                            break;
                        case MemberTypes.Method:
                            if (negate)
                                condition = (x, y) => !(bool)(conditionMember as MethodInfo).Invoke(x, y);
                            else 
                                condition = (x, y) => (bool)(conditionMember as MethodInfo).Invoke(x, y);
                            break;
                    }

                    Assert.NotNull(condition, "Should have assigned a condition by now for member type: " + conditionMember.MemberType);
                    conditions.Add(condition);
                }

                isVisible = (tgt, args) =>
                {
                    bool ret = attr.Operator == '&';
                    for (int i = 0; i < conditions.Count; i++)
                    {
                        var condition = conditions[i];
                        if (attr.Operator == '&')
                            ret &= condition(tgt, args);
                        else ret |= condition(tgt, args);
                    }
                    return ret;
                };

                //TODO: Fix FastReflection bug generating methods when target is 'object'
                //isVisible = method.DelegateForCall<object, bool>();
                //FastReflection.GenDebugAssembly("IsVisible_" + method.Name + ".dll", null, null, method, null, null);

                _isVisibleCache[member] = isVisible;
            }

            var result = isVisible(target, null);
            return result;
        }

        static bool AlwaysVisible(object target, object[] args) { return true; }
    }
}
