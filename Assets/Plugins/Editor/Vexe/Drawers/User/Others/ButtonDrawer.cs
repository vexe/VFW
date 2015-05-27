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
        private bool _singleParam;

        protected override void Initialize()
        {
            if (attribute.Method.IsNullOrEmpty())
            {
                Debug.LogError("Button method is empty!");
                return;
            }

            var method = targetType.GetMethod(attribute.Method, Flags.StaticInstanceAnyVisibility);
            if (method == null)
            {
                Debug.LogError("Button method {0} not found inside {1}"
                     .FormatWith(attribute.Method, targetType));
                return;
            }

            var parameters = method.GetParameters();
            if (member.ElementIndex == -1) // is this not a member of a sequence?
            { 
                if (parameters.Length != 1)
                {
                    Debug.LogError("Button method {0} must take only one parameter of type {1}"
                         .FormatWith(attribute.Method, memberTypeName));
                    return;
                }
            }
            else
            {
                if (parameters.Length > 2 || parameters.Length < 1)
                { 
                    Debug.LogError("Button method {0} must take either one value parameter of type {1} or two parameters, first of type {2} and an integer index"
                         .FormatWith(attribute.Method, memberTypeName, memberTypeName));
                    return;
                }
            }

            if (!parameters[0].ParameterType.IsA(memberType))
            {
                Debug.LogError("Button method ({0}) first parameter must be of a '{1}' type"
                     .FormatWith(attribute.Method, memberTypeName));
                return;
            }

            _singleParam = parameters.Length == 1;

            if (!_singleParam && parameters[1].ParameterType != typeof(int))
            {
                Debug.LogError("Button method {0} second parameter must be an integer index"
                     .FormatWith(attribute.Method));
                return;
            }

            _callback = method.DelegateForCall();

            _buttonStyle = attribute.Style;

            float width = _buttonStyle.CalcSize(new GUIContent(attribute.DisplayText)).x;
            _buttonLayout = Layout.sWidth(width);
        }

        public override void OnRightGUI()
        {
            if (gui.Button(attribute.DisplayText, _buttonStyle, _buttonLayout) && _callback != null)
            {
                if (_singleParam)
                { 
                    _1param[0] = memberValue;
                    _callback(rawTarget, _1param);
                }
                else
                {
                    _2param[0] = memberValue;
                    _2param[1] = member.ElementIndex;
                    _callback(rawTarget, _2param);
                }
            }
        }
    }
}
