using System;
using System.Collections.Generic;
using UnityEngine;
using Vexe.Editor.Drawers;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Editor
{
    /// <summary>
    /// Defines a one-to-one mappying that maps a 'type' to a drawer.
    /// Type could be a member type (int, string, List etc) or an attribute type (Popup, Slider etc)
    /// The mapping could be exact, or loose. This is deterimined by the 'isForChildren' parameter.
    /// If it's false (meaning loose mapping) then the drawer would be used on any subclass of the type we're mapping the drawer to,
    /// otherwise (exact binding) the drawer would be mapped to that type only.
    /// For ex, we want our ListDrawer<> to draw any List<> so we use loose binding (for children)
    /// We want our StringDrawer to 'only' draw strings, so we use exact binding (not for children)
    /// The caller then should specify how to 'bake' (instantiate) the drawer given the input type and drawer type.
    /// For ex, all we need to instantiate a StringDrawer is just to say new StringDrawer()
    /// but to instantiate ListDrawer<> we need to insert a value for the generic parameter which happens to be the same
    /// parameter as the in our list. So the baked drawer type is: drawerType.MakeGenericType(type.GetGenericArguments()[0])
    /// Note that the order of mapping matter. That's why only 'Insert' is public and not Add. You're not meant as a user
    /// not modify the order of the built-in drawers.
    /// For instance RecursiveDrawer must always come last, as it's meant to be a 'fallback' drawer.
    ///
    /// Please see RegisterCustomDrawerExample.cs for sample usage of this system.
    /// </summary>
    public class TypeDrawerMapper
    {
        readonly List<Type> _types = new List<Type>();
        readonly List<Type> _drawerTypes = new List<Type>();
        readonly List<bool> _isForChildren = new List<bool>();
        readonly List<Func<Type, Type, Type>> _bakeDrawerType = new List<Func<Type, Type, Type>>();

        internal BaseDrawer GetDrawer(Type forType)
        {
            for (int i = 0; i < _types.Count; i++)
            {
                var type = _types[i];
                var isForChildren = _isForChildren[i];
                if ((isForChildren && forType.IsA(type) || forType.IsSubclassOrImplementerOfRawGeneric(type)) || forType == type)
                {
                    var drawerType = _drawerTypes[i];
                    var bakedType = _bakeDrawerType[i](forType, drawerType);
                    return bakedType.Instance<BaseDrawer>();
                }
            }

            Debug.LogError("No appropriate drawer found for type: " + forType);
            return null;
        }

        internal void AddBuiltinTypes()
        {
            // Basics
            {
                this.Add<string, StringDrawer>()
                    .Add<int, IntDrawer>()
                    .Add<uint, UIntDrawer>()
                    .Add<float, FloatDrawer>()
                    .Add<double, DoubleDrawer>()
                    .Add<long, LongDrawer>()
                    .Add<bool, BoolDrawer>()
                    .Add<Color, ColorDrawer>()
                    .Add<Color32, Color32Drawer>()
                    .Add<Vector2, Vector2Drawer>()
                    .Add<Vector3, Vector3Drawer>()
                    .Add<Quaternion, QuaternionDrawer>()
                    .Add<Gradient, GradientDrawer>()
                    .Add<AnimationCurve, AnimationCurveDrawer>()
                    .Add<LayerMask, LayerMaskDrawer>()
                    .Add<Rect, RectDrawer>()
                    .Add<Bounds, BoundsDrawer>()
                    .Add<PopupAttribute, PopupDrawer>()
                    .Add<Enum, EnumDrawer>(true)
                    .Add<UnityObject, UnityObjectDrawer>(true);
            }
            // Delegates
            {
                this.Add<uDelegate, uDelegateDrawer>()
                    .Add<IBaseDelegate, DelegateDrawer>(true);
            }
            // Collections
            {
                this.Add(typeof(List<>), typeof(ListDrawer<>), true, BakeTypeArgsInDrawer)
                    .Add(typeof(Array), typeof(ArrayDrawer<>), true, (a, d) => d.MakeGenericType(a.GetElementType()))
                    .Add(typeof(IDictionary<,>), typeof(IDictionaryDrawer<,>), true, (a, d) =>
                    {
                        if (a.IsAbstract)
                        {
                            Debug.LogError("Mapping error: IDictionary type {0} is abstract thus the drawer can't instantiate an instance of it. " +
                                           "Falling back to RecursiveDrawer".FormatWith(a));
                            return typeof(RecursiveDrawer);
                        }

                        if (a.GetConstructor(Type.EmptyTypes) == null)
                        {
                            Debug.LogError("Mapping error: IDictionary type {0} must have a public empty constructor in order for it to be mapped with {1}. " +
                                           "Falling back to RecursiveDrawer".FormatWith(a, d));
                            return typeof(RecursiveDrawer);
                        }

                        return BakeTypeArgsInDrawer(a, d);
                    });
            }
            // Nullable
            {
                Add(typeof(Nullable<>), typeof(NullableDrawer<>), false, BakeTypeInDrawer);
            }
            // User
            {
                // Constraints
                {
                    this.Add<iMinAttribute, iMinDrawer>()
                        .Add<fMinAttribute, fMinDrawer>()
                        .Add<iMaxAttribute, iMaxDrawer>()
                        .Add<fMaxAttribute, fMaxDrawer>()
                        .Add<iClampAttribute, iClampDrawer>()
                        .Add<fClampAttribute, fClampDrawer>();
                }
                // Regex
                {
                    Add(typeof(RegexAttribute), typeof(RegexDrawer<>), true, BakeTypeInDrawer);
                }
                // Decorates
                {
                    this.Add<CommentAttribute, CommentDrawer>()
                        .Add<WhitespaceAttribute, WhiteSpaceDrawer>();
                }
                // Enums
                {
                    Add<EnumMaskAttribute, EnumMaskDrawer>();
                }
                // Filters
                {
                    this.Add<FilterEnumAttribute, FilterEnumDrawer>()
                        .Add<FilterTagsAttribute, FilterTagsDrawer>();
                }
                // Others
                {
                    this.Add<AssignableAttribute, AssignableDrawer>()
                        .Add<DraggableAttribute, DraggableDrawer>()
                        .Add<InlineAttribute, InlineDrawer>()
                        .Add<OnChangedAttribute, OnChangedDrawer>()
                        .Add<PathAttribute, PathDrawer>()
                        .Add<ShowTypeAttribute, ShowTypeDrawer>()
                        .Add<fSliderAttribute, fSliderDrawer>()
                        .Add<iSliderAttribute, iSliderDrawer>()
                        .Add<vSliderAttribute, vSliderDrawer>()
                        .Add<ParagraphAttribute, ParagraphDrawer>()
                        .Add<ButtonAttribute, ButtonDrawer>()
                        .Add<ResourcePathAttribute, ResourcePathDrawer>();
                }
                // Popups
                {
                    this.Add<AnimVarAttribute, AnimVarDrawer>()
                        .Add<PopupAttribute, PopupDrawer>()
                        .Add<InputAxisAttribute, InputAxisDrawer>()
                        .Add<TagsAttribute, TagsDrawer>();
                }
                // Randoms
                {
                    this.Add<iRandAttribute, iRandDrawer>()
                        .Add<fRandAttribute, fRandDrawer>();
                }
                // Selections
                {
                    this.Add<SelectEnumAttribute, SelectEnumDrawer>()
                        .Add<SelectObjAttribute, SelectObjDrawer>()
                        .Add<SelectSceneAttribute, SelectSceneDrawer>();
                }
                // Vectors
                {
                    this.Add<BetterV2Attribute, BetterV2Drawer>()
                        .Add<BetterV3Attribute, BetterV3Drawer>();
                }
            }
            // Fallback
            {
                Add<object, RecursiveDrawer>(true);
            }
        }

        TypeDrawerMapper Add(Type type, Type drawerType, bool isForChildren, Func<Type, Type, Type> bakeDrawerType)
        {
            _types.Add(type);
            _drawerTypes.Add(drawerType);
            _isForChildren.Add(isForChildren);
            _bakeDrawerType.Add(bakeDrawerType);
            return this;
        }

        TypeDrawerMapper Add<TType, TDrawer>(bool isForChildren)
        {
            return Add(typeof(TType), typeof(TDrawer), isForChildren, BakeDirect);
        }

        TypeDrawerMapper Add<TType, TDrawer>()
        {
            return Add<TType, TDrawer>(false);
        }

        public TypeDrawerMapper Insert(Type type, Type drawerType, bool isForChildren, Func<Type, Type, Type> bakeDrawerType)
        {
            if (!drawerType.IsA<BaseDrawer>())
                throw new vTypeMismatch(typeof(BaseDrawer), drawerType);

            _types.Insert(type);
            _drawerTypes.Insert(drawerType);
            _isForChildren.Insert(isForChildren);
            _bakeDrawerType.Insert(bakeDrawerType);
            return this;
        }

        public TypeDrawerMapper Insert<TType, TDrawer>(bool isForChildren)
        {
            return Insert(typeof(TType), typeof(TDrawer), isForChildren, BakeDirect);
        }

        public TypeDrawerMapper Insert<TType, TDrawer>()
        {
            return Insert<TType, TDrawer>(false);
        }

        /// <summary>
        /// Bakes a generic drawer whose generic argument is 'type'
        /// </summary>
        public Type BakeTypeInDrawer(Type type, Type drawerType)
        {
            try
            {
                if (type.IsSubclassOfRawGeneric(typeof(Nullable<>)))
                    type = type.GetGenericArguments()[0];

                return drawerType.MakeGenericType(type);
            }
            catch(ArgumentException e)
            {
                Debug.LogError("Failed to bake drawer ({0}) for type ({1}): {2} at {3}".FormatWith(drawerType.GetNiceName(), type.GetNiceName(), e.Message, e.StackTrace));
                return typeof(RecursiveDrawer);
            }
        }

        /// <summary>
        /// Returns the drawer type immediately
        /// </summary>
        public Type BakeDirect(Type type, Type drawerType)
        {
            return drawerType;
        }

        /// <summary>
        /// Bakes a generic drawer whose generic arguments are the generice arguments of 'type'
        /// </summary>
        public Type BakeTypeArgsInDrawer(Type type, Type drawerType)
        {
            return drawerType.MakeGenericType(type.GetGenericArgsInThisOrAbove());
        }
    }
}
