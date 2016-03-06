//#define PROFILE
//#define DBG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
    public class IDictionaryDrawer<TK, TV> : ObjectDrawer<IDictionary<TK, TV>>
    {
        private EditorMember _tempKeyMember;
        private List<EditorMember> _keyElements, _valueElements;
        private KVPList<TK, TV> _kvpList;
        private Attribute[] _perKeyAttributes, _perValueAttributes;
        private DictionaryOptions _options;
        private string _formatPairPattern;
        private bool _invalidKeyType;
        private TextFilter _filter;
        private string _originalDisplay;
        private int _lastUpdatedCount = -1;

        private TK _tempKey;

        public bool UpdateCount = true;

        protected override void Initialize()
        {
            _invalidKeyType = !typeof(TK).IsValueType && typeof(TK) != typeof(string);
            if (_invalidKeyType)
                return;

            _keyElements   = new List<EditorMember>();
            _valueElements = new List<EditorMember>();

            var perKey = attributes.GetAttribute<PerKeyAttribute>();
            if (perKey != null)
            {
                if (perKey.ExplicitAttributes == null)
                    _perKeyAttributes = attributes.Where(x => !(x is PerKeyAttribute)).ToArray();
                else _perKeyAttributes = attributes.Where(x => perKey.ExplicitAttributes.Contains(x.GetType().Name.Replace("Attribute", ""))).ToArray();
            }

            var perValue = attributes.GetAttribute<PerValueAttribute>();
            if (perValue != null)
            {
                if (perValue.ExplicitAttributes == null)
                    _perValueAttributes = attributes.Where(x => !(x is PerValueAttribute)).ToArray();
                else _perValueAttributes = attributes.Where(x => perValue.ExplicitAttributes.Contains(x.GetType().Name.Replace("Attribute", ""))).ToArray();
            }

            var displayAttr = attributes.GetAttribute<DisplayAttribute>();
            if (displayAttr != null)
                _formatPairPattern = displayAttr.FormatKVPair;

			_options = new DictionaryOptions(displayAttr != null ? displayAttr.DictOpt : Dict.None);

            if (_formatPairPattern.IsNullOrEmpty())
                _formatPairPattern = "[$key, $value]";

            if (_options.Readonly)
                displayText += " (Readonly)";

            _originalDisplay = displayText;

            if (_options.Filter)
                _filter = new TextFilter(null, id, true, prefs, null);

            if (memberValue == null && !_options.ManualAlloc)
                memberValue = memberType.Instance<IDictionary<TK, TV>>();

            if (memberValue != null)
                member.CollectionCount = memberValue.Count;

            if (_options.TempKey)
            {
                _tempKeyMember = EditorMember.WrapMember(GetType().GetField("_tempKey", Flags.InstanceAnyVisibility),
                        this, unityTarget, RuntimeHelper.CombineHashCodes(id, "temp"), null);
                _tempKeyMember.DisplayText = string.Empty;
                _tempKey = GetNewKey(memberValue);
            }

            #if DBG
            Log("Dictionary drawer Initialized (" + dictionaryName + ")");
            #endif
        }

        public override void OnGUI()
        {
            if (_invalidKeyType)
            {
                gui.HelpBox("Unsuported key type: {0}. Only Value-types and strings are, sorry!"
                   .FormatWith(typeof(TK)), MessageType.Error);
                return;
            }

            if (memberValue == null)
            {
                if (_options.ManualAlloc)
                {
                    using(gui.Horizontal())
                    {
                        gui.Label(member.NiceName + " (Null)");
                        if (gui.Button("New", GUIStyles.MiniRight, Layout.sFit()))
                            memberValue = memberType.Instance<IDictionary<TK, TV>>();
                    }
                }
                else memberValue = memberType.Instance<IDictionary<TK, TV>>();
            }

            if (memberValue == null)
                return;

            member.CollectionCount = memberValue.Count;

            if (UpdateCount && _lastUpdatedCount != memberValue.Count)
            {
                _lastUpdatedCount = memberValue.Count;
                displayText = Regex.Replace(_originalDisplay, @"\$count", _lastUpdatedCount.ToString());
            }

            if (_kvpList == null)
                _kvpList = new KVPList<TK, TV>();
            else _kvpList.Clear();

            // Read
            {
                var iter = memberValue.GetEnumerator();
                while(iter.MoveNext())
                {
                    var key = iter.Current.Key;
                    var value = iter.Current.Value;
                    _kvpList[key] = value;
                }
            }

            #if PROFILE
            Profiler.BeginSample("DictionaryDrawer Header");
            #endif

            // header
            if (!_options.HideHeader)
            {
                using (gui.Horizontal())
                {
                    if (_options.ForceExpand)
                        gui.Label(displayText);
                    else
                        foldout = gui.Foldout(displayText, foldout, Layout.Auto);

                    if (_options.Filter)
                        _filter.Field(gui, 70f);

                    if (!_options.Readonly)
                    {
                        if (_options.TempKey)
                        {
                            string controlName = "TempKey";
                            GUI.SetNextControlName(controlName);
                            gui.Member(_tempKeyMember);
                            var e = Event.current;
                            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == controlName)
                            {
                                AddNewPair();
                                EditorGUI.FocusTextInControl(controlName);
                            }
                        }
                        else gui.FlexibleSpace();

                        if (!_options.HideButtons)
                        { 
                            using (gui.State(_kvpList.Count > 0))
                            {
                                if (gui.ClearButton("dictionary"))
                                    _kvpList.Clear();

                                if (gui.RemoveButton("last added dictionary pair"))
                                {
                                    if (_options.AddToLast)
                                        _kvpList.RemoveLast();
                                    else
                                        _kvpList.RemoveFirst();
                                }
                            }
                            if (gui.AddButton("pair", MiniButtonStyle.ModRight))
                                AddNewPair();
                        }
                    }
                }
                gui.Space(3f);
            }

            #if PROFILE
            Profiler.EndSample();
            #endif

            if (!foldout && !_options.ForceExpand)
                return;

            if (memberValue.Count == 0)
            {
                using (gui.Indent())
                    gui.HelpBox("Dictionary is empty");
            }
            else
            {
                #if PROFILE
                Profiler.BeginSample("DictionaryDrawer Pairs");
                #endif
                using (gui.Indent())
                {
                    for (int i = 0; i < _kvpList.Count; i++)
                    {
                        var dKey   = _kvpList.Keys[i];
                        var dValue = _kvpList.Values[i];

                        #if PROFILE
                        Profiler.BeginSample("DictionaryDrawer KVP assignments");
                        #endif

                        int entryKey = RuntimeHelper.CombineHashCodes(id, i, "entry");

                        string pairStr = null;

                        if (_filter != null)
                        {

                            pairStr = FormatPair(dKey, dValue);
                            #if PROFILE
                            Profiler.BeginSample("DictionaryDrawer Filter");
                            #endif
                            bool match = _filter.IsMatch(pairStr);
                            #if PROFILE
                            Profiler.EndSample();
                            #endif
                            if (!match)
                                continue;
                        }

                        if (!_options.HorizontalPairs)
                        {
                            if (pairStr == null)
                                pairStr = FormatPair(dKey, dValue);
                            prefs[entryKey] = gui.Foldout(pairStr, prefs[entryKey], Layout.Auto);
                        }

                        #if PROFILE
                        Profiler.EndSample();
                        #endif

                        if (!prefs[entryKey] && !_options.HorizontalPairs)
                            continue;

                        #if PROFILE
                        Profiler.BeginSample("DictionaryDrawer SinglePair");
                        #endif
                        if (_options.HorizontalPairs)
                        {
                            using (gui.Horizontal())
                            {
                                DrawKey(i, entryKey + 1);
                                DrawValue(i, entryKey + 2);
                            }
                        }
                        else
                        { 
                            using (gui.Indent())
                            {
                                DrawKey(i, entryKey + 1);
                                DrawValue(i, entryKey + 2);
                            }
                        }
                        #if PROFILE
                        Profiler.EndSample();
                        #endif
                    }
                }
                #if PROFILE
                Profiler.EndSample();
                #endif

                #if PROFILE
                Profiler.BeginSample("DictionaryDrawer Write");
                #endif
                // Write
                {
                    Write();
                }
                #if PROFILE
                Profiler.EndSample();
                #endif
            }
        }

        private void Write()
        {
            memberValue.Clear();
            for (int i = 0; i < _kvpList.Count; i++)
            {
                var key = _kvpList.Keys[i];
                var value = _kvpList.Values[i];
                try
                {
                    memberValue.Add(key, value);
                }
                catch (ArgumentException) //@Todo: figure out a more forgiveful way to handle this
                {
                    Log("Key already exists: " + key);
                }
            }
        }

        public void DrawKey(int index, int id)
        {
            var keyMember = GetElement(_keyElements, _kvpList.Keys, index, id + 1);
            using(gui.If(!_options.Readonly && typeof(TK).IsNumeric(), gui.LabelWidth(15f)))
            { 
                if (_options.Readonly)
                {
                    var previous = keyMember.Value;
                    var changed = gui.Member(keyMember, @ignoreComposition: _perKeyAttributes == null);
                    if (changed)
                        keyMember.Value = previous;
                }
                else
                { 
                    gui.Member(keyMember, @ignoreComposition: _perKeyAttributes == null);
                }
            }
        }

        public void DrawValue(int index, int id)
        {
            var valueMember = GetElement(_valueElements, _kvpList.Values, index, id + 2);
            using(gui.If(!_options.Readonly && typeof(TV).IsNumeric(), gui.LabelWidth(15f)))
            {
                if (_options.Readonly)
                {
                    var previous = valueMember.Value;
                    var changed = gui.Member(valueMember, @ignoreComposition: _perValueAttributes == null);
                    if (changed)
                        valueMember.Value = previous;
                }
                else
                {
                    gui.Member(valueMember, @ignoreComposition: _perValueAttributes == null);
                }
            }
        }

        private EditorMember GetElement<T>(List<EditorMember> elements, List<T> source, int index, int id)
        {
            if (index >= elements.Count)
            {
                Attribute[] attrs;
                if (typeof(T) == typeof(TK))
                    attrs = _perKeyAttributes;
                else
                    attrs = _perValueAttributes;

                var element = EditorMember.WrapIListElement(
                    @elementName : typeof(T).IsNumeric() && !_options.Readonly ? "~" : string.Empty,
                    @elementType : typeof(T),
                    @elementId   : RuntimeHelper.CombineHashCodes(id, index),
                    @attributes  : attrs
                );
                element.InitializeIList(source, index, rawTarget, unityTarget);
                elements.Add(element);
                return element;
            }

            try
            {
                var e = elements[index];
                e.Write = Write;
                e.InitializeIList(source, index, rawTarget, unityTarget);
                return e;
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("DictionaryDrawer: Accessing element out of range. Index: {0} Count {1}. This shouldn't really happen. Please report it with information on how to replicate it".FormatWith(index, elements.Count));
                return null;
            }
        }

        private string FormatPair(TK key, TV value)
        {
            #if PROFILE
            Profiler.BeginSample("DictionaryDrawer: FormatPair");
            #endif
            string format = formatPair(new KeyValuePair<TK, TV>(key, value));
            #if PROFILE
            Profiler.EndSample();
            #endif
            return format;
        }

        private Func<KeyValuePair<TK, TV>, string> _formatPair;
        private Func<KeyValuePair<TK, TV>, string> formatPair
        {
            get
            {
                return _formatPair ?? (_formatPair = new Func<KeyValuePair<TK, TV>, string>(pair =>
                {
                    var key = pair.Key;
                    var value = pair.Value;

                    var result = _formatPairPattern;
                    result = Regex.Replace(result, @"\$keytype", key == null ? "null" : key.GetType().GetNiceName());
                    result = Regex.Replace(result, @"\$valuetype", value == null ? "null" : value.GetType().GetNiceName());
                    result = Regex.Replace(result, @"\$key", GetObjectString(key));
                    result = Regex.Replace(result, @"\$value", GetObjectString(value));
                    //Debug.Log("New format: " + result);
                    return result;
                }).Memoize());
            }
        }

        private string GetObjectString(object from)
        {
            if (from.IsObjectNull())
                return "null";

            var obj = from as UnityObject;
            if (obj != null)
                return obj.name + " (" + obj.GetType().Name + ")";

            var toStr = from.ToString();
            return toStr == null ? "null" : toStr;
        }

        TK GetNewKey(IDictionary<TK, TV> from)
        {
            TK key;

            if (typeof(TK) == typeof(string))
            {
                string prefix;
                int postfix;
                if (from.Count > 0)
                {
                    prefix = from.Last().Key as string;
                    string postfixStr = "";
                    int i = prefix.Length - 1; 
                    for (; i >= 0; i--)
                    {
                        char c = prefix[i];
                        if (!char.IsDigit(c))
                            break;
                        postfixStr = postfixStr.Insert(0, c.ToString());
                    }
                    if (int.TryParse(postfixStr, out postfix))
                        prefix = prefix.Remove(i + 1, postfixStr.Length);
                }
                else
                { 
                    prefix = "New Key ";
                    postfix = 0;
                }
                while(from.Keys.Contains((TK)(object)(prefix + postfix))) postfix++;
                key = (TK)(object)(prefix + postfix);
            }
            else if (typeof(TK) == typeof(int))
            {
                var n = 0;
                while (from.Keys.Contains((TK)(object)(n))) n++;
                key = (TK)(object)n;
            }
            else if (typeof(TK).IsEnum)
            {
                var values = Enum.GetValues(typeof(TK)) as TK[];
                var result = values.Except(from.Keys).ToList();
                if (result.Count == 0)
                    return default(TK);
                key = (TK)result[0];
            }
            else key = default(TK);

            return key;
        }

        private void AddNewPair()
        {
            var key = _options.TempKey ? _tempKey : GetNewKey(_kvpList);
            try
            {
                var value = default(TV);
                if (_options.AddToLast)
                    _kvpList.Add(key, value);
                else
                    _kvpList.Insert(0, key, value);

                memberValue.Add(key, value);

                var eKey = RuntimeHelper.CombineHashCodes(id, (_kvpList.Count - 1), "entry");
                prefs[eKey] = true;
                foldout = true;

                if (_options.TempKey)
                    _tempKey = GetNewKey(_kvpList);
            }
            catch (ArgumentException)
            {
                Log("Key already exists: " + key);
            }
        }

		private struct DictionaryOptions
		{
            public readonly bool Readonly;
            public readonly bool ForceExpand;
            public readonly bool HideHeader;
            public readonly bool HorizontalPairs;
            public readonly bool Filter;
            public readonly bool AddToLast;
            public readonly bool TempKey;
            public readonly bool ManualAlloc;
            public readonly bool HideButtons;

			public DictionaryOptions(Dict options)
			{
                Readonly           = options.HasFlag(Dict.Readonly);
                ForceExpand        = options.HasFlag(Dict.ForceExpand);
                HideHeader         = options.HasFlag(Dict.HideHeader);
                HorizontalPairs    = options.HasFlag(Dict.HorizontalPairs);
                Filter             = options.HasFlag(Dict.Filter);
                AddToLast          = options.HasFlag(Dict.AddToLast);
                TempKey            = options.HasFlag(Dict.TempKey);
                ManualAlloc        = options.HasFlag(Dict.ManualAlloc);
                HideButtons        = options.HasFlag(Dict.HideButtons);
			}
		}
    }
}
