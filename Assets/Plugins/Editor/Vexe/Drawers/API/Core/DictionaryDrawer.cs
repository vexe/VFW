//#define PROFILE
//#define DBG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
    public class IDictionaryDrawer<TD, TK, TV> : ObjectDrawer<TD> where TD : class, IDictionary<TK, TV>, new() where TK : new()
    {
        private List<EditorMember> _keyElements;
        private List<EditorMember> _valueElements;
        private KVPList<TK, TV> _kvpList;
        private string _formatPairPattern;
        private bool _perKeyDrawing, _perValueDrawing;
        private bool _invalidKeyType, _isReadonly;

        protected override void Initialize()
        {
            _invalidKeyType = !typeof(TK).IsValueType && typeof(TK) != typeof(string);
            if (_invalidKeyType)
                return;

            _keyElements   = new List<EditorMember>();
            _valueElements = new List<EditorMember>();

            _perKeyDrawing   = attributes.AnyIs<PerKeyAttribute>();
            _perValueDrawing = attributes.AnyIs<PerValueAttribute>();

            var displayAttr = attributes.GetAttribute<DisplayAttribute>();
            if (displayAttr != null)
            { 
                _formatPairPattern = displayAttr.FormatKVPair;
                _isReadonly = (displayAttr.DictOpt & Dict.Readonly) != 0;
            }

            if (_formatPairPattern.IsNullOrEmpty())
                _formatPairPattern = "[$key, $value]";

            if (_isReadonly)
                displayText += " (Readonly)";

            #if DBG
            Log("Dictionary drawer Initialized (" + dictionaryName + ")");
            #endif
        }

        public class AddInfo
        {
            public TK key;
            public TV value;
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
                memberValue = new TD();

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

            using (gui.Horizontal())
            {
                foldout = gui.Foldout(displayText, foldout, Layout.sExpandWidth());

                if (!_isReadonly)
                {
                    gui.FlexibleSpace();

                    using (gui.State(_kvpList.Count > 0))
                    {
                        if (gui.ClearButton("dictionary"))
                            _kvpList.Clear();

                        if (gui.RemoveButton("last dictionary pair"))
                            _kvpList.RemoveFirst();
                    }

                    if (gui.AddButton("pair", MiniButtonStyle.ModRight))
                        AddNewPair();
                }
            }

            #if PROFILE
            Profiler.EndSample();
            #endif

            if (!foldout)
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

                        var pairStr        = FormatPair(dKey, dValue);
                        var entryKey       = RuntimeHelper.CombineHashCodes(id, i, "entry");
                        foldouts[entryKey] = gui.Foldout(pairStr, foldouts[entryKey], Layout.sExpandWidth());

                        #if PROFILE
                        Profiler.EndSample();
                        #endif

                        if (!foldouts[entryKey])
                            continue;

                        #if PROFILE
                        Profiler.BeginSample("DictionaryDrawer SinglePair");
                        #endif
                        using (gui.Indent())
                        {
                            var keyMember = GetElement(_keyElements, _kvpList.Keys, i, entryKey + 1);
                            gui.Member(keyMember, !_perKeyDrawing);

                            var valueMember = GetElement(_valueElements, _kvpList.Values, i, entryKey + 2);
                            gui.Member(valueMember, !_perValueDrawing);
                        }
                        #if PROFILE
                        Profiler.EndSample();
                        #endif
                    }
                }
                #if PROFILE
                Profiler.EndSample();
                #endif

                // Write
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
                        catch (ArgumentException)
                        {
                            Log("Key already exists: " + key);
                        }
                    }
                }
            }
        }

        private EditorMember GetElement<T>(List<EditorMember> elements, List<T> source, int index, int id)
        {
            if (index >= elements.Count)
            {
                var element = EditorMember.WrapIListElement(
                    @elementName : string.Empty,
                    @elementType : typeof(T),
                    @elementId   : RuntimeHelper.CombineHashCodes(id, index),
                    @attributes  : attributes
                );
                element.InitializeIList(source, index, rawTarget, unityTarget);
                elements.Add(element);
                return element;
            }

            try
            {
                var e = elements[index];
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
            return formatPair(new KeyValuePair<TK, TV>(key, value));
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
                    return result;
                }).Memoize());
            }
        }

        private string GetObjectString(object from)
        {
            if (from.IsObjectNull())
                return "null";
            var obj = from as UnityObject;
            return (obj != null) ? (obj.name + " (" + obj.GetType().Name + ")") : from.ToString();
        }

        private void AddNewPair()
        {
            TK defKey;

            if (typeof(TK) == typeof(string))
            {
                var x = "New Key ";
                var n = 0;
                while(_kvpList.Keys.Contains((TK)(object)(x + n))) n++;
                defKey = (TK)(object)(x + n);
            }
            else if (typeof(TK) == typeof(int))
            {
                var n = 0;
                while (_kvpList.Keys.Contains((TK)(object)(n))) n++;
                defKey = (TK)(object)n;
            }
            else if (typeof(TK).IsEnum)
            {
                var values = Enum.GetValues(typeof(TK)) as TK[];
                var result = values.Except(_kvpList.Keys).ToList();
                if (result.Count == 0)
                    return;
                defKey = (TK)result[0];
            }
            else defKey = default(TK);

            try
            {
                var defValue = default(TV); 
                _kvpList.Insert(0, defKey, defValue);
                memberValue.Add(defKey, defValue);

                var eKey = RuntimeHelper.CombineHashCodes(id, (_kvpList.Count - 1), "entry");
                foldouts[eKey] = true;
            }
            catch (ArgumentException)
            {
                Log("Key already exists: " + defKey);
            }
        }
    }
}