using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Vexe.Editor.Drawers {
    public class AnimVarDrawer : AttributeDrawer<AnimVarAttribute> {

        private abstract class TargetValue {

            protected readonly EditorMember Member;

            protected TargetValue(EditorMember member) {
                Member = member;
            }

            public abstract int FindCurrent(AnimatorControllerParameter[] parameters, string[] names);

            public abstract void SetValue(AnimatorControllerParameter acp);

        }

        private class HashValue : TargetValue {

            public HashValue(EditorMember member) : base(member) { }

            public override int FindCurrent(AnimatorControllerParameter[] parameters, string[] names) {
                return parameters.Select(param => param.nameHash).ToArray().IndexOf((int) Member.Value);
            }

            public override void SetValue(AnimatorControllerParameter acp) {
                Member.Value = acp == null ? 0 : acp.nameHash;
            }

        }

        private class StringValue : TargetValue {

            public StringValue(EditorMember member) : base(member) { }

            public override int FindCurrent(AnimatorControllerParameter[] parameters, string[] names) {
                return names.IndexOf(Member.Value as string);
            }

            public override void SetValue(AnimatorControllerParameter acp) {
                Member.Value = acp == null ? "" : acp.name;
            }

        }

        private class ACPValue : TargetValue {

            public ACPValue(EditorMember member) : base(member) { }

            public override int FindCurrent(AnimatorControllerParameter[] parameters, string[] names) {
                return parameters.IndexOf(Member.Value);
            }

            public override void SetValue(AnimatorControllerParameter acp) {
                Member.Value = acp;
            }

        }

        // A mapping of which AnimatorControllerParameterType goes to which ParameterType value
        private static Dictionary<AnimatorControllerParameterType, ParameterType> mapping;

        // All valid controller parameters to be displayed
        private AnimatorControllerParameter[] _parameters;

        // A cached array of names for each of the valid controller parameters
        // For use in the Popup control used by this drawer
        private string[] _names;

        // The currently selected index on the Popup
        private int _current;

        // The currently selected parameter
        private AnimatorControllerParameter _currentParameter;

        // A mask for which kinds of controller parameters to show
        // By default, this value should show all parameter types
        // This would be of type AnimatorControllerParameterType, but Unity explicitly chose values that were hard to make masks with
        // So a custom enum with mask-friendly values was made
        private ParameterType _mask;

        // The Animator component that corresponds with this drawer
        private Animator _animator;

        // A polymorphic object 
        private TargetValue _value;

        private Animator Animator {
            get {
                if (_animator == null) {
                    string getterMethod = attribute.GetAnimatorMethod;
                    if (getterMethod.IsNullOrEmpty())
                        _animator = gameObject.GetComponent<Animator>();
                    else
                        _animator = targetType.GetMethod(getterMethod, Flags.InstanceAnyVisibility)
                                              .Invoke(rawTarget, null) as Animator;
                }
                return _animator;
            }
        }

        private AnimatorControllerParameter Current {
            get {
                if (!Ready)
                    return _currentParameter = null;
                return _currentParameter;
            }
            set {
                _current = _parameters.IndexOfZeroIfNotFound(value);
                _currentParameter = (!Ready || _parameters.IsEmpty()) ? null : _parameters[_current];
                _value.SetValue(_currentParameter);
            }
        }

        // A simple check to see if both a animator is attached, and a controller is used in it
        private bool Ready {
            get { return Animator != null && Animator.runtimeAnimatorController != null; }
        }

        protected override void Initialize() {
            if (mapping == null) {
                mapping = new Dictionary<AnimatorControllerParameterType, ParameterType>();
                foreach (string name in Enum.GetNames(typeof(AnimatorControllerParameterType))) {
                    var acpt = (AnimatorControllerParameterType) Enum.Parse(typeof(AnimatorControllerParameterType), name, true);
                    var pt = (ParameterType) Enum.Parse(typeof(ParameterType), name, true);
                    mapping[acpt] = pt;
                }
            }
            if (memberType.IsA<string>())
                _value = new StringValue(member);
            else if (memberType.IsA<int>())
                _value = new HashValue(member);
            else //Assuming it is a AnimatorControllerParameter
                _value = new ACPValue(member);
            _mask = attribute.Filter;
            FetchVariables();
        }

        private void FetchVariables() {
            if (!Ready)
                return;

            // Select matching parameters from the Animator
            _parameters = Animator.parameters.Where(param => (_mask & mapping[param.type]) != 0).ToArray();
            _names = _parameters.Select(param => param.name).ToArray();

            if (_parameters.IsEmpty()) {
                _parameters = new AnimatorControllerParameter[] { null };
                _names = new[] { "N/A" };
            } else {
                // Try to find a match
                int index = _value.FindCurrent(_parameters, _names);

                // Attempt to automatch if the search fails
                if (!attribute.AutoMatch.IsNullOrEmpty() && index < 0) {
                    string match = displayText.Remove(displayText.IndexOf(attribute.AutoMatch));
                    match = Regex.Replace(match, @"\s+", "");
                    index = _names.IndexOf(match);
                }

                // Assign the current value
                Current = index >= 0 ? _parameters[index] : _parameters[0];
            }
        }

        public override void OnGUI() {
            if (!Ready) {
                if (memberType.IsA<string>())
                    member.Value = gui.Text(displayText, (string) member.Value);
                else {
                    using (gui.Horizontal()) {
                        gui.Label(displayText);
                        gui.Label("N/A: Please Attach Animator with Controller");
                    }
                }
            } else {
                if (_parameters.IsNullOrEmpty())
                    FetchVariables();

                var selection = gui.Popup(displayText, _current, _names);
                {
                    if (_current != selection || Current != _parameters[selection])
                        Current = _parameters[_current = selection];
                }
            }
        }

        public override bool CanHandle(Type memberType) {
            return memberType.IsA<string>() || memberType.IsA<int>() || memberType.IsA<AnimatorControllerParameter>();
        }
    }
}