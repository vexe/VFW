//#define PROFILE
//#define DBG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.Helpers;
using Vexe.Editor.Types;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.Drawers
{
    public class DictionaryDrawer<TD, TK, TV> : ObjectDrawer<TD> where TD : class, IDictionary<TK, TV>, new() where TK : new()
    {
        private List<ElementMember<TK>> keyElements;
        private List<ElementMember<TV>> valueElements;
        private KVPList<TK, TV> kvpList;
        private string dictionaryName;
        private string pairFormatPattern;
        private MethodCaller<object, object> pairFormatMethod;
        private bool perKeyDrawing, perValueDrawing;
        private bool shouldRead = true, shouldWrite, invalidKeyType;
        private Color dupKeyColor, shouldWriteColor;
        private bool isKvpList;

        public bool Readonly { get; set; }

        protected override void OnSingleInitialization()
        {
            isKvpList = memberType.IsA<KVPList<TK, TV>>();

            shouldWriteColor = GUIHelper.OrangeColorDuo.FirstColor;
            dupKeyColor = GUIHelper.RedColorDuo.FirstColor;

            var kt = typeof(TK);

            if (kt.IsAbstract || kt.IsA<UnityObject>())
            { 
                //Log("key type is abstract or a unityobject");
                invalidKeyType = true;
            }

            if (!invalidKeyType && !kt.IsValueType && kt != typeof(string))
            {
                try
                {
                    kt.ActivatorInstance();
                }
                catch (MissingMemberException)
                {
                    //Log("key type is not newable");
                    invalidKeyType = true;
                }
            }

            if (invalidKeyType)
                return;

            keyElements   = new List<ElementMember<TK>>();
            valueElements = new List<ElementMember<TV>>();

            perKeyDrawing   = attributes.AnyIs<PerKeyAttribute>();
            perValueDrawing = attributes.AnyIs<PerValueAttribute>();
            Readonly		= attributes.AnyIs<ReadonlyAttribute>();

            var formatMember = attributes.OfType<FormatMemberAttribute>().FirstOrDefault();
            if (formatMember == null || string.IsNullOrEmpty(formatMember.pattern))
            { 
                dictionaryName  = niceName;
                dictionaryName += " (" + memberType.GetNiceName() + ")";
            }
            else
            {
                dictionaryName = formatMember.Format(niceName, memberType.GetNiceName());
            }

            var pairFormat = attributes.GetAttribute<FormatPairAttribute>();
            if (pairFormat != null)
            {
                if (!string.IsNullOrEmpty(pairFormat.Method))
                    pairFormatMethod = rawTarget.GetType().DelegateForCall(pairFormat.Method, Flags.InstanceAnyVisibility, typeof(TK), typeof(TV));
                else if (!string.IsNullOrEmpty(pairFormat.Pattern))
                    pairFormatPattern = pairFormat.Pattern;
            }

            if (Readonly)
                dictionaryName += " (Readonly)";

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
            if (invalidKeyType)
            {
                gui.HelpBox("Key type {0} must either be a ValueType or a 'new'able ReferenceType (not an abstract type/a UnityEngine.Object and has an empty or implicit/compiler-generated constructor)".FormatWith(typeof(TK).Name),
                    MessageType.Error);
                return;
            }

            if (memberValue == null)
            { 
                #if DBG
                Log("Dictionary null " + dictionaryName);
                #endif
                memberValue = new TD();
            }

            // if the member is a kvpList, we can read immediately because we don't have to worry about allocation or anything it's just an assignment
            if (isKvpList)
            {
                kvpList = memberValue as KVPList<TK, TV>;
            }
            else
            {
                shouldRead |= (kvpList == null || (!shouldWrite && memberValue.Count != kvpList.Count));

                if (shouldRead)
                {
                    #if DBG
                    Log("Reading " + dictionaryName);
                    #endif
                    kvpList = memberValue.ToKVPList();
                    shouldRead = false;
                }
            }


            #if PROFILE
            Profiler.BeginSample("DictionaryDrawer Header");
            #endif

            using (gui.Horizontal())
            {
                foldout = gui.Foldout(dictionaryName, foldout, Layout.sExpandWidth());

                if (!Readonly)
                {
                    gui.FlexibleSpace();

                    using (gui.State(kvpList.Count > 0))
                    {
                        if (gui.ClearButton("dictionary"))
                        {
                            kvpList.Clear();
                            shouldWrite = true;
                        }

                        if (gui.RemoveButton("last dictionary pair"))
                        {
                            kvpList.RemoveFirst();
                            shouldWrite = true;
                        }
                    }

                    if (gui.AddButton("pair"))
                    {
                        AddNewPair();
                        shouldWrite = true;
                    }

                    Color col;
                    if (!kvpList.Keys.IsUnique())
                        col = dupKeyColor;
                    else if (shouldWrite)
                        col = shouldWriteColor;
                    else col = Color.white;

                    using (gui.ColorBlock(col))
                    if (gui.MiniButton("w", "Write dictionary (Orange means you modified the dictionary and should write, Red means you have a duplicate key and must address it before writing)", MiniButtonStyle.ModRight))
                    {
                        #if DBG
                        Log("Writing " + dictionaryName);
                        #endif
                        if (isKvpList)
                        {
                            memberValue = kvpList as TD;
                        }
                        else
                        { 
                            try
                            {
                                var newDict = new TD();
                                for (int i = 0; i < kvpList.Count; i++)
                                {
                                    var k = kvpList.Keys[i];
                                    var v = kvpList.Values[i];
                                    newDict.Add(k, v);
                                }
                                memberValue = newDict;
                            }
                            catch (ArgumentException e)
                            {
                                Log(e.Message);
                            }
                        }

                        shouldWrite = false;
                    }
                }
            }

            #if PROFILE
            Profiler.EndSample();
            #endif

            if (!foldout)
                return;

            if (kvpList.Count == 0)
            {
                gui.HelpBox("Dictionary is empty");
            }
            else
            { 
                #if PROFILE
                Profiler.BeginSample("DictionaryDrawer Pairs");
                #endif
                using (gui.Indent())
                {
                    for (int i = 0; i < kvpList.Count; i++)
                    {
                        var dKey   = kvpList.Keys[i];
                        var dValue = kvpList.Values[i];

                        #if PROFILE
                        Profiler.BeginSample("DictionaryDrawer KVP assignments");
                        #endif

                        var pairStr        = FormatPair(dKey, dValue);
                        var entryKey       = RTHelper.CombineHashCodes(id, i, "entry");
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
                            var keyMember = GetElement(keyElements, kvpList.Keys, i, entryKey + 1);
                            shouldWrite |= gui.Member(keyMember, !perKeyDrawing);

                            var valueMember = GetElement(valueElements, kvpList.Values, i, entryKey + 2);
                            shouldWrite |= gui.Member(valueMember, !perValueDrawing);
                        }
                        #if PROFILE
                        Profiler.EndSample();
                        #endif
                    }
                }
                #if PROFILE
                Profiler.EndSample();
                #endif

                shouldWrite |= memberValue.Count > kvpList.Count;
            }
        }

        private ElementMember<T> GetElement<T>(List<ElementMember<T>> elements, List<T> source, int index, int id)
        {
            if (index >= elements.Count)
            {
                var element = new ElementMember<T>(
                    @id          : id + index,
                    @attributes  : attributes,
                    @name        : string.Empty
                );
                element.Initialize(source, index, rawTarget, unityTarget);
                elements.Add(element);
                return element;
            }

            try
            {
                var e = elements[index];
                e.Initialize(source, index, rawTarget, unityTarget);
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

                    if (pairFormatPattern == null)
                    { 
                        if (pairFormatMethod == null)
                            return string.Format("[{0}, {1}]", GetObjectString(key), GetObjectString(value));
                        return pairFormatMethod(rawTarget, new object[] { pair.Key, pair.Value }) as string;
                    }

                    var result = pairFormatPattern;
                    result = Regex.Replace(result, @"\$keyType", key == null ? "null" : key.GetType().GetNiceName());
                    result = Regex.Replace(result, @"\$valueType", value == null ? "null" : value.GetType().GetNiceName());
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

        Func<TK> _getNewKey;
        Func<TK> getNewKey
        {
            get
            {
                if (_getNewKey == null)
                {
                    if (typeof(TK).IsValueType)
                    {
                        _getNewKey = () => (TK)typeof(TK).GetDefaultValue();
                    }
                    else if (typeof(TK) == typeof(string))
                    {
                        _getNewKey = () => (TK)(object)string.Empty;
                    }
                    else
                    {
                        _getNewKey = () => new TK();
                    }
                }
                return _getNewKey;
            }
        }

        private void AddNewPair()
        {
            try
            {
                var key = getNewKey();
                var value = default(TV); 
                kvpList.Insert(0, key, value, false);

                var pkey = RTHelper.CombineHashCodes(id, (kvpList.Count - 1), "entry");
                foldouts[pkey] = true;
            }
            catch (ArgumentException e)
            {
                Log(e.Message);
            }
        }
    }
}