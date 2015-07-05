using System;
using System.Reflection;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers
{
    public class ButtonDrawer : CompositeDrawer<object, ButtonAttribute>
    {
        private MethodCaller<object, object> _callback;
        private Layout _buttonLayout;
        private GUIStyle _buttonStyle = GUIStyles.Button;
        private static object[] _1param = new object[1];
        private static object[] _2param = new object[2];
        private int _params;

        protected override void Initialize()
        {
            if (attribute.Method.IsNullOrEmpty())
            {
                Debug.LogError("Button method is empty!");
                goto Init;
            }

            var method = targetType.GetMemberFromAll(attribute.Method, Flags.StaticInstanceAnyVisibility) as MethodInfo;
            if (method == null)
            {
                Debug.LogError("Button method {0} not found inside {1}"
                     .FormatWith(attribute.Method, targetType));
                goto Init;
            }

            var parameters = method.GetParameters();
            _params = parameters.Length;
            if (_params > 2)
            {
                Debug.LogError("Button method {0} must take either no parameters, " +
                                "one parameter of type {1} or two parameters if the member was a list/array, " + 
                                "first parameter must be of the list/array element type, second is an integer index"
                                .FormatWith(attribute.Method, memberTypeName));
                goto Init;
            }
            if (_params > 0)
            {
                if (_params == 1)
                {
                    if (!parameters[0].ParameterType.IsA(memberType))
                    {
                        Debug.LogError("Button method ({0}) first parameter must be of a '{1}' type"
                             .FormatWith(attribute.Method, memberTypeName));
                        goto Init;
                    }
                }
                else
                {
                    Type elementType;
                    if (member.IsCollection)
                        elementType = memberType;
                    else if (!memberType.TryGetSequenceElementType(out elementType))
                    {
                        Debug.LogError("Method {0} has two parameters, member {1} must be a sequence (list/array)"
                             .FormatWith(attribute.Method, member.Name));
                        goto Init;
                    }
                    if (!parameters[0].ParameterType.IsA(elementType))
                    {
                        Debug.LogError("Button method ({0}) first parameter must be of a '{1}' type"
                             .FormatWith(attribute.Method, memberTypeName));
                        goto Init;
                    }
                    if (parameters[1].ParameterType != typeof(int))
                    {
                        Debug.LogError("Button method {0} second parameter must be an integer index"
                             .FormatWith(attribute.Method));
                        goto Init;
                    }
                }
            }

            _callback = method.DelegateForCall();

            Init:

            _buttonStyle = attribute.Style;

            float width = _buttonStyle.CalcSize(new GUIContent(attribute.DisplayText)).x;
            _buttonLayout = Layout.sWidth(width);
        }

        public override void OnRightGUI()
        {
            if (gui.Button(attribute.DisplayText, _buttonStyle, _buttonLayout) && _callback != null)
            {
                switch(_params)
                {
                    case 0:
                        _callback(rawTarget, null);
                        break;
                    case 1:
                        _1param[0] = memberValue;
                        _callback(rawTarget, _1param);
                        break;
                    case 2:
                        _2param[0] = memberValue;
                        _2param[1] = member.ElementIndex;
                        _callback(rawTarget, _2param);
                        break;
                }
            }
        }
    }
}
