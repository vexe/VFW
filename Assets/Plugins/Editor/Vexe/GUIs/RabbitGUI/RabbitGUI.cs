//#define dbg_level_1
//#define dbg_level_2
#define dbg_controls

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vexe.Editor.Helpers;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor.GUIs
{
    public class RabbitGUI : BaseGUI, IDisposable
    {
        public Action OnFinishedLayoutReserve, OnRepaint;

        public float Height { private set; get; }

        public float Width  { private set; get; }

        public override Rect LastRect
        {
            get
            {
                if (!_allocatedMemory)
                    return kDummyRect;

                if (_nextControlIdx == 0)
                    throw new InvalidOperationException("Can't get last rect - there are no previous controls to get the last rect from");

                if (_nextControlIdx - 1 >= _controls.Count)
                {
                    #if dbg_level_1
                    Debug.Log("Last rect out of range. Returning dummy rect. If that's causing problems, maybe request a seek instead. nextControlIdx {0}. controls.Count {1}".FormatWith(_nextControlIdx, _controls.Count));
                    #endif
                    return kDummyRect; // or maybe request reset?
                }

                return _controls[_nextControlIdx - 1].rect;
            }
        }

        private enum GUIPhase { Layout, Draw }
        private GUIPhase _currentPhase;
        private List<GUIControl> _controls;
        private List<GUIBlock> _blocks;
        private Stack<GUIBlock> _blockStack;
        private Rect _startRect;
        private Rect? _validRect;
        private int _nextControlIdx;
        private int _nextBlockIdx;
        private float _prevInspectorWidth;
        private bool _pendingLayout;
        private bool _pendingRepaint;
        private bool _allocatedMemory;
        private bool _storedValidRect;
        private int _id;
        private float _widthCorrection = 0f;

        static MethodCaller<object, string> _scrollableTextArea;
        static MethodCaller<object, Gradient> _gradientField;

        #if dbg_level_1
        private int dbgMaxDepth;
        public static void LogCallStack()
        {
            string stack = RuntimeHelper.GetCallStack();
            Debug.Log("Call stack: " + stack);
        }
        #endif


#if dbg_level_2
        private bool m_pendingReset;
        private bool _pendingReset
        {
            get { return m_pendingReset; }
            set
            {
                if (value)
                    Debug.Log("Setting Reset Request to True! Came from: " + RuntimeHelper.GetCallStack());
                m_pendingReset = value;
            }
        }
#else
        private bool _pendingReset;
#endif

        public RabbitGUI(EditorRecord prefs)
        {
            _currentPhase = GUIPhase.Layout;
            _controls     = new List<GUIControl>();
            _blocks       = new List<GUIBlock>();
            _blockStack   = new Stack<GUIBlock>();
            this.prefs    = prefs;

            #if dbg_level_1
                Debug.Log("Instantiated Rabbit");
            #endif
        }

        static RabbitGUI()
        {
            var editorGUIType = typeof(EditorGUI);

            // ScrollabeTextArea
            {
                var method = editorGUIType.GetMethod("ScrollableTextAreaInternal",
                    new Type[] { typeof(Rect), typeof(string), typeof(Vector2).MakeByRefType(), typeof(GUIStyle) },
                    Flags.StaticAnyVisibility);

                _scrollableTextArea = method.DelegateForCall<object, string>();
            }
            // GradientField
            {
                var method = editorGUIType.GetMethod("GradientField",
                    new Type[] { typeof(GUIContent), typeof(Rect), typeof(Gradient) },
                    Flags.StaticAnyVisibility);

                _gradientField = method.DelegateForCall<object, Gradient>();
            }
        }

        void OnEditorUpdate()
        {
            if (!EditorApplication.isCompiling)
                return;

            StoreValidRect();
        }

        void OnPlaymodeChanged()
        {
            StoreValidRect();
        }

        void StoreValidRect()
        {
            if (!_storedValidRect && _validRect.HasValue)
            {
                int key = RuntimeHelper.CombineHashCodes(_id, "rabbit_coords");
                int keyX = RuntimeHelper.CombineHashCodes(key, "x");
                int keyY = RuntimeHelper.CombineHashCodes(key, "y");
                prefs[keyX] = _validRect.Value.x;
                prefs[keyY] = _validRect.Value.y;
                _storedValidRect = true;
            }
        }

        public override void OnGUI(Action guiCode, Vector4 padding, int targetId)
        {
            _widthCorrection = padding.y;
            _id = targetId;

            if (!_validRect.HasValue)
            {
                int key = RuntimeHelper.CombineHashCodes(_id, "rabbit_coords");
                int keyX = RuntimeHelper.CombineHashCodes(key, "x");
                int keyY = RuntimeHelper.CombineHashCodes(key, "y");
                RecordValue prevX, prevY;
                if (prefs.TryGetValue(keyX, out prevX))
                {
                    //Log("Seems we changed play modes and rabbit doesn't have a coord. but we have in store a prev coord from a previous editor session that should work");
                    prevY = prefs[keyY];
                    var tmp = new Rect();
                    tmp.x = prevX;
                    tmp.y = prevY;
                    _validRect = tmp;
                }
            }

            var unityRect = GUILayoutUtility.GetRect(0f, 0f);

            if (Event.current.type == EventType.Repaint)
            {
                if (!_validRect.HasValue || _validRect.Value.y != unityRect.y)
                {
                    _validRect = unityRect;
                    _pendingLayout = true;
                }
            }

            if (_validRect.HasValue)
            {
                
                var start = new Rect(_validRect.Value.x + padding.x, _validRect.Value.y + padding.z,
                    EditorGUIUtility.currentViewWidth - padding.y, _validRect.Value.height);

                using (Begin(start))
                    guiCode();
            }

            GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth - 35f, Height - padding.w);

            OnFinishedLayoutReserve.SafeInvoke();
        }

        public RabbitGUI Begin(Rect start)
        {
            if (_currentPhase == GUIPhase.Layout)
            {
                #if dbg_level_1
                    Debug.Log("Layout phase. Was pending layout: {0}. Was pending reset: {1}".FormatWith(_pendingLayout, _pendingReset));
                #endif
                Width = start.width;
                Height = 0f;
                _startRect = start;
                _pendingLayout = false;
                _pendingReset = false;
            }

            _nextControlIdx = 0;
            _nextBlockIdx   = 0;
            BeginVertical(GUIStyles.None);

            return this;
        }

        public override void OnEnable()
        {
            EditorApplication.playmodeStateChanged += OnPlaymodeChanged;
            EditorApplication.update += OnEditorUpdate;
        }

        public override void OnDisable()
        {
            EditorApplication.playmodeStateChanged -= OnPlaymodeChanged;
            EditorApplication.update -= OnEditorUpdate;
        }

        private void End()
        {
            _allocatedMemory = true;
            var main = _blocks[0];
            main.Dispose();

            if (_currentPhase == GUIPhase.Layout)
            {
                main.ResetDimensions();
                main.Layout(_startRect);
                Height = main.height.Value;
                _currentPhase = GUIPhase.Draw;

                if (_pendingRepaint)
                {
                    EditorHelper.RepaintAllInspectors();
                    OnRepaint.SafeInvoke();
                    _pendingRepaint = false;
                }

                #if dbg_level_1
                    Debug.Log("Done layout. Deepest Block depth: {0}. Total number of blocks created: {1}. Total number of controls {2}"
                                .FormatWith(dbgMaxDepth, _blocks.Count, _controls.Count));
                #endif
            }
            else
            {
                if (_pendingReset || _nextControlIdx != _controls.Count || _nextBlockIdx != _blocks.Count)
                {
                    #if dbg_level_1
                    if (_pendingReset)
                        Debug.Log("Resetting - Theres a reset request pending");
                    else Debug.Log("Resetting -  The number of controls/blocks drawn doesn't match the total number of controls/blocks");
                    #endif
                    _controls.Clear();
                    _blocks.Clear();
                    _allocatedMemory = false;
                    _pendingRepaint = true;
                    _currentPhase = GUIPhase.Layout;
                }
                else if (_pendingLayout)
                {
                    #if dbg_level_1
                        Debug.Log("Pending layout request. Doing layout in next phase");
                    #endif
                    _currentPhase = GUIPhase.Layout;
                    EditorHelper.RepaintAllInspectors();
                }
                else {
                    bool resized = _prevInspectorWidth != EditorGUIUtility.currentViewWidth + _widthCorrection;
                    if (resized)
                    {
                        #if dbg_level_1
                            Debug.Log("Resized inspector. Doing layout in next phase");
                        #endif
                        _prevInspectorWidth = EditorGUIUtility.currentViewWidth + _widthCorrection;
                        _currentPhase = GUIPhase.Layout;
                    }
                }
            }
        }

        private T BeginBlock<T>(GUIStyle style) where T : GUIBlock, new()
        {
            if (_pendingReset)
            {
                #if dbg_level_1
                    Debug.Log("Pending reset. Can't begin block of type: " + typeof(T).Name);
                #endif
                return null;
            }

            if (_allocatedMemory && _nextBlockIdx >= _blocks.Count)
            {
                #if dbg_level_1
                    Debug.Log("Requesting Reset. Can't begin block {0}. We seem to have created controls yet nextBlockIdx {1} > blocks.Count {2}"
                                .FormatWith(typeof(T).Name, _nextBlockIdx, _blocks.Count));
                #endif
                _pendingReset = true;
                return null;
            }

            T result;
            if (!_allocatedMemory)
            {
                _blocks.Add(result = new T
                {
                    onDisposed = EndBlock,
                    data = new ControlData
                    {
                        type = typeof(T).Name.ParseEnum<ControlType>(),
                        style = style,
                    }
                });

                if (_blockStack.Count > 0)
                {
                    var owner = _blockStack.Peek();
                    owner.AddBlock(result);
                }

                #if dbg_level_1
                    Debug.Log("Created new block of type {0}. Blocks count {1}. Is pending reset? {2}".FormatWith(typeof(T).Name, _blocks.Count, _pendingReset));
                #endif
            }
            else
            {
                result = _blocks[_nextBlockIdx] as T;

                if (result != null)
                {
                    GUI.Box(result.rect, string.Empty, result.data.style = style);
                }
                else
                {
                    var requestedType = typeof(T);
                    var resultType = _blocks[_nextBlockIdx].GetType();
                    if (requestedType != resultType)
                    {
                        #if dbg_level_1
                            Debug.Log("Requested block result is null. " +
                                         "The type of block requested {0} doesn't match the block type {1} at index {2}. " +
                                         "This is probably due to the occurance of new blocks revealed by a foldout for ex. " +
                                         "Requesting Reset".FormatWith(requestedType.Name, resultType.Name, _nextBlockIdx));
                        #endif
                        _pendingReset = true;
                        return null;
                    }

                    #if dbg_level_1
                        Debug.Log("Result block is null. Count {0}, Idx {1}, Request type {2}".FormatWith(_blocks.Count, _nextBlockIdx, typeof(T).Name));
                        for (int i = 0; i < _blocks.Count; i++)
                            Debug.Log("Block {0} at {1} has {2} controls".FormatWith(_blocks[i].data.type.ToString(), i, _blocks[i].controls.Count));

                        Debug.Log("Block Stack count " + _blockStack.Count);
                        var array = _blockStack.ToArray();
                        for (int i = 0; i < array.Length; i++)
                            Debug.Log("Block {0} at {1} has {2} controls".FormatWith(array[i].data.type.ToString(), i, array[i].controls.Count));
                    #endif

                    throw new NullReferenceException("result");
                }
            }

            _nextBlockIdx++;
            _blockStack.Push(result);

            #if dbg_level_2
                Debug.Log("Pushed {0}. Stack {1}. Total {2}. Next {3}".FormatWith(result.GetType().Name, _blockStack.Count, _blocks.Count, _nextBlockIdx));
            #endif

            #if dbg_level_1
            if (_blockStack.Count > dbgMaxDepth)
                dbgMaxDepth = _blockStack.Count;
            #endif

            return result;
        }

        private void EndBlock()
        {
            if (!_pendingReset)
                _blockStack.Pop();
        }

        private bool CanDrawControl(out Rect position, ControlData data)
        {
            position = kDummyRect;

            if (_pendingReset)
            {
                #if dbg_level_1
                    Debug.Log("Can't draw control of type " + data.type + " There's a Reset pending.");
                #endif
                return false;
            }

            if (_nextControlIdx >= _controls.Count && _currentPhase == GUIPhase.Draw)
            {
                #if dbg_level_1
                    Debug.Log("Can't draw control of type {0} nextControlIdx {1} is >= controls.Count {2}. Requesting reset".FormatWith(data.type, _nextControlIdx, _controls.Count));
                    LogCallStack();
                #endif
                _pendingReset = true;
                return false;
            }

            if (!_allocatedMemory)
            {
                NewControl(data);
                return false;
            }

            position = _controls[_nextControlIdx++].rect;
            return true;
        }

        private GUIControl NewControl(ControlData data)
        {
            var parent  = _blockStack.Peek();
            var control = new GUIControl(data);
            parent.controls.Add(control);
            _controls.Add(control);
            #if dbg_level_2
                Debug.Log("Created control {0}. Count {1}".FormatWith(data.type, _controls.Count));
            #endif
            return control;
        }

        public void RequestReset()
        {
            _pendingReset = true;
        }

        public void RequestLayout()
        {
            _pendingLayout = true;
        }

        void IDisposable.Dispose()
        {
            End();
        }

        public override IDisposable If(bool condition, IDisposable body)
        {
            if (condition)
                return body;
            body.Dispose();
            return null;
        }

        public override Bounds BoundsField(GUIContent content, Bounds value, Layout option)
        {
            var bounds = new ControlData(content, GUIStyles.None, option, ControlType.Bounds);

            Rect position;
            if (CanDrawControl(out position, bounds))
            {
                return EditorGUI.BoundsField(position, content, value);
            }

            return value;
        }

        public override Rect Rect(GUIContent content, Rect value, Layout option)
        {
            var data = new ControlData(content, GUIStyles.None, option, ControlType.RectField);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.RectField(position, content, value);
            }

            return value;
        }

		public override AnimationCurve Curve (GUIContent content, AnimationCurve value, Layout option)
		{
			var data = new ControlData (content, GUIStyles.None, option, ControlType.CurveField);

			Rect position;
			if (CanDrawControl (out position, data))
			{
				if(value == null)
					value = new AnimationCurve();
				return EditorGUI.CurveField(position, content, value);
			}

			return value;
		}

        public override Gradient GradientField(GUIContent content, Gradient value, Layout option)
        {
            var data = new ControlData(content, GUIStyles.None, option, ControlType.GradientField);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                if (value == null)
                    value = new Gradient();
                return _gradientField(null, new object[] { content, position, value });
            }

            return value;
        }

        public override void Box(GUIContent content, GUIStyle style, Layout option)
        {
            var data = new ControlData(content, style, option, ControlType.Box);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                GUI.Box(position, content, style);
            }
        }

        public override void HelpBox(string message, MessageType type)
        {
            var content = GetContent(message);
            var height  = GUIStyles.HelpBox.CalcHeight(content, Width);
            var layout  = Layout.sHeight(height);
            var data    = new ControlData(content, GUIStyles.HelpBox, layout, ControlType.HelpBox);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                EditorGUI.HelpBox(position, message, type);
            }
        }

        public override bool Button(GUIContent content, GUIStyle style, Layout option, ControlType buttonType)
        {
            var data = new ControlData(content, style, option, buttonType);

            Rect position;
            if (!CanDrawControl(out position, data))
                return false;

#if dbg_controls
            // due to the inability of unity's debugger to successfully break inside generic classes
            // I'm forced to write things this way so I could break when I hit buttons in my drawers,
            // since I can't break inside them cause they're generics... Unity...
            var pressed = GUI.Button(position, content, style);
            if (pressed)
                return true;
            return false;
#else
            return GUI.Button(position, content, style);
#endif
        }

        public override Color Color(GUIContent content, Color value, Layout option)
        {
            var data = new ControlData(content, GUIStyles.ColorField, option, ControlType.ColorField);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.ColorField(position, content, value);
            }

            return value;
        }

        public override Enum EnumPopup(GUIContent content, Enum selected, GUIStyle style, Layout option)
        {
            var data = new ControlData(content, style, option, ControlType.EnumPopup);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.EnumPopup(position, content, selected, style);
            }

            return selected;
        }

        public override float Float(GUIContent content, float value, Layout option)
        {
            var data = new ControlData(content, GUIStyles.NumberField, option, ControlType.Float);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.FloatField(position, content, value);
            }

            return value;
        }

        public override bool Foldout(GUIContent content, bool value, GUIStyle style, Layout option)
        {
            var data = new ControlData(content, style, option, ControlType.Foldout);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.Foldout(position, value, content, true, style);
            }

            return value;
        }

        public override int Int(GUIContent content, int value, Layout option)
        {
            var data = new ControlData(content, GUIStyles.NumberField, option, ControlType.IntField);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.IntField(position, content, value);
            }

            return value;
        }

        public override void Label(GUIContent content, GUIStyle style, Layout option)
        {
            var label = new ControlData(content, style, option, ControlType.Label);

            Rect position;
            if (CanDrawControl(out position, label))
            {
                EditorGUI.LabelField(position, content, style);
            }
        }

        public override int MaskField(GUIContent content, int mask, string[] displayedOptions, GUIStyle style, Layout option)
        {
            var data = new ControlData(content, style, option, ControlType.MaskField);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.MaskField(position, content, mask, displayedOptions, style);
            }

            return mask;
        }

        public override UnityObject Object(GUIContent content, UnityObject value, Type type, bool allowSceneObjects, Layout option)
        {
            var data = new ControlData(content, GUIStyles.ObjectField, option, ControlType.ObjectField);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.ObjectField(position, content, value, type, allowSceneObjects);
            }

            return value;
        }

        public override int Popup(string text, int selectedIndex, string[] displayedOptions, GUIStyle style, Layout option)
        {
            var content = GetContent(text);
            var popup = new ControlData(content, style, option, ControlType.Popup);

            Rect position;
            if (CanDrawControl(out position, popup))
            {
                return EditorGUI.Popup(position, content.text, selectedIndex, displayedOptions, style);
            }

            return selectedIndex;
        }

        protected override void BeginScrollView(ref Vector2 pos, bool alwaysShowHorizontal, bool alwaysShowVertical, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background, Layout option)
        {
            throw new NotImplementedException("I need to implement ExpandWidth and ExpandHeight first, sorry");
        }

        protected override void EndScrollView()
        {
            throw new NotImplementedException();
        }

        public override float FloatSlider(GUIContent content, float value, float leftValue, float rightValue, Layout option)
        {
            var data = new ControlData(content, GUIStyles.HorizontalSlider, option, ControlType.Slider);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.Slider(position, content, value, leftValue, rightValue);
            }

            return value;
        }

        public override void Space(float pixels)
        {
            if (!_allocatedMemory)
            {
                var parent = _blockStack.Peek();
                var option = parent.Space(pixels);
                var space  = new ControlData(GUIContent.none, GUIStyle.none, option, ControlType.Space);
                NewControl(space);
            }
            else
            {
                _nextControlIdx++;
            }
        }

        public override void FlexibleSpace()
        {
            if (!_allocatedMemory)
            {
                var flexible = new ControlData(GUIContent.none, GUIStyle.none, null, ControlType.FlexibleSpace);
                NewControl(flexible);
            }
            else
            {
                _nextControlIdx++;
            }
        }

        public override string Text(GUIContent content, string value, GUIStyle style, Layout option)
        {
            var data = new ControlData(content, style, option, ControlType.TextField);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.TextField(position, content, value, style);
            }

            return value;
        }

        public override string ToolbarSearch(string value, Layout option)
        {
            var data = new ControlData(GetContent(value), GUIStyles.TextField, option, ControlType.TextField);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                int searchMode = 0;
                return GUIHelper.ToolbarSearchField(null, new object[] { position, null, searchMode, value }) as string;
            }

            return value;
        }

        public override bool ToggleLeft(GUIContent content, bool value, GUIStyle labelStyle, Layout option)
        {
            var data = new ControlData(content, labelStyle, option, ControlType.Toggle);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.ToggleLeft(position, content, value, labelStyle);
            }

            return value;
        }

        public override bool Toggle(GUIContent content, bool value, GUIStyle style, Layout option)
        {
            var data = new ControlData(content, style, option, ControlType.Toggle);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.Toggle(position, content, value, style);
            }

            return value;
        }

        protected override HorizontalBlock BeginHorizontal(GUIStyle style)
        {
            return BeginBlock<HorizontalBlock>(style);
        }

        protected override VerticalBlock BeginVertical(GUIStyle style)
        {
            return BeginBlock<VerticalBlock>(style);
        }

        protected override void EndHorizontal()
        {
            EndBlock();
        }

        protected override void EndVertical()
        {
            EndBlock();
        }

        public override string TextArea(string value, Layout option)
        {
            if (option == null)
                option = new Layout();

            if (!option.height.HasValue)
                option.height = 50f;

            var data = new ControlData(GetContent(value), GUIStyles.TextArea, option, ControlType.TextArea);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.TextArea(position, value);
            }

            return value;
        }

        public override bool InspectorTitlebar(bool foldout, UnityObject target)
        {
            var data = new ControlData(GUIContent.none, GUIStyles.None, null, ControlType.Foldout);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.InspectorTitlebar(position, foldout, target, true);
            }

            return foldout;
        }

        public override string Tag(GUIContent content, string tag, GUIStyle style, Layout layout)
        {
            var data = new ControlData(content, style, layout, ControlType.Popup);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.TagField(position, content, tag, style);
            }

            return tag;
        }

        public override int LayerField(GUIContent content, int layer, GUIStyle style, Layout layout)
        {
            var data = new ControlData(content, style, layout, ControlType.Popup);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                return EditorGUI.LayerField(position, content, layer, style);
            }

            return layer;
        }

        public override void Prefix(string label)
        {
            if (string.IsNullOrEmpty(label)) return;
            var content = GetContent(label);
            var style = EditorStyles.label;
            var data = new ControlData(content, style, Layout.sWidth(EditorGUIUtility.labelWidth), ControlType.PrefixLabel);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                EditorGUI.HandlePrefixLabel(position, position, content, 0, style);
            }
        }

        public override string ScrollableTextArea(string value, ref Vector2 scrollPos, GUIStyle style, Layout option)
        {
            if (option == null)
                option = Layout.None;

            if (!option.height.HasValue)
                option.height = 50f;

            var content = GetContent(value);
            var data = new ControlData(content, style, option, ControlType.TextArea);

            Rect position;
            if (CanDrawControl(out position, data))
            {
                var args = new object[] { position, value, scrollPos, style };
                var newValue = _scrollableTextArea.Invoke(null, args);
                scrollPos = (Vector2)args[2];
                return newValue;
            }

            return value;
        }

        static bool hoveringOnPopup;
        static readonly string[] emptyStringArray = new string[0];

        public override string TextFieldDropDown(GUIContent label, string value, string[] dropDownElements, Layout option)
        {
            var data = new ControlData(label, GUIStyles.TextFieldDropDown, option, ControlType.TextFieldDropDown);

            Rect totalRect;
            if (CanDrawControl(out totalRect, data))
            {
                Rect textRect = new Rect(totalRect.x, totalRect.y, totalRect.width - GUIStyles.TextFieldDropDown.fixedWidth, totalRect.height);
                Rect popupRect = new Rect(textRect.xMax, textRect.y, GUIStyles.TextFieldDropDown.fixedWidth, totalRect.height);

                value = EditorGUI.TextField(textRect, "", value, GUIStyles.TextFieldDropDownText);

                string[] displayedOptions;
                if (dropDownElements.Length > 0)
                    displayedOptions = dropDownElements;
                else
                    (displayedOptions = new string[1])[0] = "--empty--";

                if (popupRect.Contains(Event.current.mousePosition))
                    hoveringOnPopup = true;

                // if there were a lot of options to be displayed, we don't need to always invoke
                // Popup cause Unity does a lot of allocation inside and it would have a huge negative impact
                // on editor performance so we only display the options when we're hoving over it
                if (!hoveringOnPopup)
                    EditorGUI.Popup(popupRect, string.Empty, -1, emptyStringArray, GUIStyles.TextFieldDropDown);
                else
                {
                    EditorGUI.BeginChangeCheck();
                    int selection = EditorGUI.Popup(popupRect, string.Empty, -1, displayedOptions, GUIStyles.TextFieldDropDown);
                    if (EditorGUI.EndChangeCheck() && displayedOptions.Length > 0)
                    {
                        hoveringOnPopup = false;
                        value = displayedOptions[selection];
                    }
                }
            }
            return value;
        }

        public override double Double(GUIContent content, double value, Layout option)
        {
            var data = new ControlData(content, GUIStyles.NumberField, option, ControlType.Double);

            Rect position;
            if (CanDrawControl(out position, data))
                return EditorGUI.DoubleField(position, content, value);
            return value;
        }

        public override long Long(GUIContent content, long value, Layout option)
        {
            var data = new ControlData(content, GUIStyles.NumberField, option, ControlType.Long);

            Rect position;
            if (CanDrawControl(out position, data))
                return EditorGUI.LongField(position, content, value);
            return value;
        }

        public override void MinMaxSlider(GUIContent label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, Layout option)
        {
            var data = new ControlData(label, GUIStyles.MinMaxSlider, option, ControlType.Slider);

            Rect position;
            if (CanDrawControl(out position, data))
                EditorGUI.MinMaxSlider(label, position, ref minValue, ref maxValue, minLimit, maxLimit);
        }
    }
}
