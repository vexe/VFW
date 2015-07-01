## Vexe Framework (VFW)

### Contents

- [What](#what)
- [Usage](#usage)
- [Features](#heres-a-birds-eye-view-for-some-of-the-features)
  - [A new drawing API](#feature-drawing-api)
  - [An improved serialization system](#feature-improved-serialization)
  - [Helper types](#feature-helper-types)
  - [Tons of property attributes](#feature-property-attributes)
- [Screenshots](#screenshots)
- [Tutorials/Videos](#tutorialsvideos-read-the-video-description-to-see-what-it-covers)
- [Known Issues](#known-issues)
- [Acknowledgements](#acknowledgments)
- [FAQ](#faq)
  - [License](#faq-license)
  - [Does this require Unity Pro?](#faq-needs-pro)
  - [Does this support AOT platforms?](#faq-support-aot)
  - [Why free?](#faq-free)
  - [How does the drawing system work and how do I write custom drawers?](#faq-drawing-system)
  - [How are my class members ordered in the inspector?](#faq-member-order)
  - [What *doesn't* the serialization system support?](#faq-doesnt-serialization)
  - [What are the exact serialization rules?](#faq-exact-serialization)
  - [So should I use [Serialize], or [SerializeField]? any reason to prefer one over the other?](#faq-serialize-or-serialize-field)
  - [How to write converters/surrogates to customize serialization for/add support to certain types?](#faq-converters)
  - [How to view the serialize state of a BetterBehaviour?](#faq-view-state)
  - [How do you serialize a BetterBehaviour to file?](#faq-how-to-serialize)

### What:
  - VFW is both a runtime and editor extension that offers much more advance features than what comes out of the box with Unity. A better drawing API, a faster GUI layout system for editor drawing, a custom serialization system, tons of attributes/drawers, helper types and more. 
 
### Usage:
  - Import the VFW unitypackage into your project.
  - Inherit BetterBehaviour instead of MonoBehaviour and BetterScriptableObject instead of ScriptableObject, and don't forget to add your using statement:

```csharp
using Vexe.Runtime.Types; // for BetterBehaviour, BetterScriptableObject and uAction
using System.Collections.Generic; // for List
using UnityEngine; // for GameObject

public class TestBehaviour : BetterBehaviour
{
    public Dictionary<int, GameObject> someDictionary;
    public uAction someAction;
    public List<int[]> someList { get; set; }
}

public class TestScriptableObject : BetterScriptableObject
{
    public Dictionary<int, GameObject> someDictionary;
    public uAction someAction;
    public List<int[]> someList { get; set; }
}
```


### Here's a bird's eye-view for some of the features:

####1- <a name="feature-drawing-api">A new drawing API that has the following features</a>:
1. You can write drawers for pretty much 'any' System.Type!
1. Can be applied on any 'member' (method, property or field) You can write a single drawer and apply it on fields or properties! DRY!
1. The API is strongly typed (uses generics). You get a strong reference to attribute you're writing your drawer for
1. Fields/properties are dealt with equally the same. This is possible via a 'DataMember' wrapper around them. That wrapper is strongly-typed, there's also a weak-typed version of it. No more dealing with SerializedProperties!
1. You can expose 'any' member even if it wasn't serializable! (inspector exposure is independent of serialization).
1. Fields, properties, methods, generics, interfaces, abstract objects, dictionaries, arrays, lists, etc. All are exposed and serializable.
1. Attribute composition works very well, as long as you compose correctly (just like Unity's 'order' attribute, you get an 'id' that you could use to sort your composite attributes).
1. You can re-use your drawers everywhere! Drawers target a System.Object as opposed to Unity's targeting UnityEngine.Object. So you could easily use your drawers in an EditorWindow for instance.
1. The drawing API is so flexible, that you can use drawers on members that don't have any attributes on them! pseudo: MemberField(someMember, someAttributes);
1. When dealing with collections, you have the option whether you want to apply attributes on the collection itself, or its individual elements.
1. You can use both GUILayout and GUI!
1. You can have your composite drawers target specific areas of your target field. So you get a OnLeftGUI, OnRightGUI, OnBottomGUI and OnUpperGUI (for the left, right, bottom, and upper sides of your field) - Composition level: Korean!
1. Custom/shorter attributes to show/hide things: [Show], [Hide]
1. Focus on writing your drawers' logic without having to write any type-checks/validation to make sure your attributes are applied on the right field/property type. Apply a Popup on a GameObject field, it just gets ignored.

####2- <a name="feature-improved-serialization">An improved serialization system that has the following features</a>:
1. Polymorphic types (interfaces and abstract/base system objects)
1. Generic types. (you can go crazy with nesting generics. i.e. Dictionary<int, Dictionary<List<ISomeInterface>>>)
1. Auto-properties (properties with side-effects are *not* serialized. Instead, you just serialize the backing field behind it)
1. static fields/auto-properties can be serialized *only* in BetterBehaviours, not in any arbitrary System.Object
1. readonly fields!
1. Structs, custom classes and Nullables!
1. Most common collection types: arrays (one dimensional), List<T>, Dictionary<TK, TV>, Stack<T>, Queue<T> and HashSet<T>
1. Delegates support with Action/Func-like equivalents: uAction and uFunc (supports up to 4 type arguments)
1. You can have your own serialization logic with your own attributes! (more on the default serialization logic/attributes shortly)


####3- <a name="feature-helper-types">Helper types</a>:
1. The generic event system from uFAction.
1. BetterAnimator (an Animator wrapper that lets you easily modify the animator's floats, bools, ints, etc)
1. SelectionMemorizer: memorizes your objects selections. Ctrl.Shift.- to go back, Ctrl.Shift.+ to go forward.
1. BetterUndo: a simple undo system implemented via the command pattern that lets you take direct control of what is it to be done, and how it is to be undone
1. RabbitGUI: A much faster and cleaner GUI layouting system than Unity's GUILayout/EditorGUILayout with similar API and some extra features.

####4- <a name="feature-property-attributes">Tons of property attributes</a>:
1. **Constraints**: *Min*, *Max*, *Regex*, *IP*, *NumericClamp*, *StringClamp*
1. **Decorates**: *Comment*, *WhiteSpace*
1. **Enums**: *EnumMask* (one that works! - credits to Bunny83), *SelectEnum* (gives you the ability to right-click an enum to show the values in a selection window - useful for enums with large values)
1. **Popups**: *AnimVar* (shows all the variables available in the attached Animator component), *Popup* (your usual popup - could take a method name to generate the values at runtime), *Tags* (displays all available tags in a popup), *InputAxis* (gives you all the available input axes like Horizontal etc)
1. **Randoms**: Rand (sets a float/int to a random value with a randomize button)
1. **Requirements**: *Required*, *RequiredFromThis*, *RequiredFromChildren*, *RequiredFromParents* (Signifies that the annotated field is required for things to work properly. Extremely useful, makes assigning variables/configuring less of a pain by assigning fields automatically for you, just tell it where to look - you could tell it to add the component if not found)
1. **Vectors**: *BetterVector* (gives you the ability to zero, normalize and copy/paste vector values)
1. **Others**: *Draggable* (allows you to drag/drop the field itself), *Path* (allows you to drag/drop objects to a string field to take their string value - useful if you have a path field for a folder/file instead of manually writing its path you just drag-drop it, works on GameObjects too). *Readonly* (makes your field unmodifiable), *ShowType* (displays a popup for all the runtime types children for a given System.Type). *OnChanged* (my fav! lets you choose a callback method or property/field to set when your field value changes!), *Assignable* (lets you assign your fields/properties from a source object)
1. **Selections**: *SelectObj*, *SelectEnum*, *SelectScene*: give you a selection window to select objects (from children, parents, etc), enums and scenes in that order.
1. **Filters**: *FilterEnum*, *FilterTags*: give you a small text field to quickly filter out values to quickly assign your enums/tags
1. **Categories**: *DefineCategory*: defines a member category and lets you categorize your behavior members according to custom rules and filters. *Category*: annotate members with this, to include them in a certain category. IgnoreCategories: Ignore (hide) previously defined categories. *BasicView*: displays your behavior in an uncategorized style. *MinimalView*: even more basic than the previous.
1. **Collections**: *Seq*: lets you customize the way your sequence (array/list) looks. *PerItem*: indicated that you want your composite attributes to be drawn on each element of a sequence (array/list). *PerKey/PerValue*: indicates that you want to apply your custom attributes on each key/value of a dictionary.

### Screenshots:
- [UnityTypes](http://i.imgur.com/O5klA26.png)
- [ShowType](http://i.imgur.com/WlLhnXn.png)
- [Sequences](http://i.imgur.com/vtPe4SN.png)
- [SelectObj](http://i.imgur.com/eOHFBw3.png)
- [SelectEnum](http://i.imgur.com/srCJepM.png)
- [Popups](http://i.imgur.com/HNeuUSB.png)
- [InputAxis](http://i.imgur.com/UN7qf2d.png)
- [Nullables](http://i.imgur.com/7q9Lx99.png)
- [ShowMethod](http://i.imgur.com/O5n2gCv.png)
- [Abstracts](http://i.imgur.com/f71qvqQ.png)
- [Interfaces](http://i.imgur.com/ODwc6hT.png)
- [Inline](http://i.imgur.com/vHZORHn.png)
- [Generics/Nesting](http://i.imgur.com/8ZdCdXf.png)
- [Delegates](http://i.imgur.com/ALGlmux.png)
- [AnimVar](http://i.imgur.com/q0lINLZ.png)
- [Filters](http://i.imgur.com/J0F5Msb.png)
- [Attributes in EditorWindow](http://i.imgur.com/2b8YOJu.png)
- [Assignable](http://i.imgur.com/MzIPxVD.png)

### Tutorials/Videos (read the video description to see what it covers):
1. [Intro / API Overview](https://www.youtube.com/watch?v=9AnTNGKjAK0)
1. [Attributes 1](https://www.youtube.com/watch?v=jsNXBY2VJ6g)
1. [Attributes 2](https://www.youtube.com/watch?v=y3XJKnOCrcU)
1. [Serialization](https://www.youtube.com/watch?v=CmbOEYQoV7Q)
1. [Member Categories](https://www.youtube.com/watch?v=c9PDvJOZXGU)
1. [Delegates 1](https://www.youtube.com/watch?v=NBSMIURbfmY)
1. [Delegates 2](https://www.youtube.com/watch?v=iPhY08o-X2g)
1. [Misc Features, and some minor issues](https://www.youtube.com/watch?v=rlGqavcPNuE)
1. [Full Serializer Integration](https://www.youtube.com/watch?v=7V4vIBBq03g)
1. [EventSystem & BetterAnimator](https://www.youtube.com/watch?v=c3SJ0vWSIjs)

### Known Issues:
1. Undo isn't 100% perfect still, (although it has been improved in 1.3)
1. Not everything is documented yet
1. No multi-object editing support yet

### Acknowledgments: 
1. Please understand that this is a one-man job, I try my best to make everything work, but I can't make everything perfect. I try to do my best isolating my code and writing unit tests for it to make everything's working OK, but a bug or two could easily slip in... If you want to write a program without bugs, then don't write it.
1. Apologies in advance if you come across a frustrating bug or any inconvenience due to using this framework, if you do, make sure you let met know.
1. For help, improvements/suggestions, bug reports: you could post here, pm me or email me at askvexe@gmail.com. I will try to reply ASAP and help as best as I can.
1. I always use the latest version of Unity for development. Before you report an issue, please make sure you're using the latest version too. If the issue is still there, report it.

### FAQ: 
1. <a name="faq-license">**License?**</a>
  - [MIT](http://choosealicense.com/licenses/mit/).
1. <a name="faq-needs-pro">**Does this require Unity Pro?** 
  - No
1. <a name="faq-support-aot">**Does this support AOT platforms?**</a>
  - Yes. It has been verified to work on iOS. While not manually verified on Android, but it should work.
1. <a name="faq-free">**Why free?**</a>
  1. Because I believe I can build a base "standard" framework that anyone could use in his own personal projects to benefit from and build upon. The current state in almost all asset developers is that they all come up with 'their' own solutions to the same problems over and over again. This makes it difficult for assets from different publishers to play nice with each other because their backbone is different. For ex say you purchased a property drawers asset that you'd like to use in say, Node Canvas. Well you can't because NC and the drawers asset use a different base for their codes thus making them incompatible. 
  2. This stuff should be available to us out-of-the-box let alone free... don't let me start uRanting. 
  3. Open source is beautiful. Everyone could contribute to the improvement of the software, fixing bugs, etc.
1. <a name="faq-drawing-system">**How does the drawing system work? And how do I write custom drawers?**</a>
  - There are 3 types of drawers: ObjectDrawer<T>, AttributeDrawer<T, A> and CompositeDrawer<T, A>. 
  1. ObjectDrawers define how a member looks like in the inspector. Use an ObjectDrawer when you want your drawer to be applied on your type wherever it occurs. Examples of that are IntDrawer, StringDrawer, ListDrawer<T>, ArrayDrawer<T> etc. So whenever an int, string, List<T>, T[] occur, the corresponding drawer will be used to draw them. In your ObjectDrawer you override OnGUI and do all your gui stuff. You will get a strongly-typed 'memberValue' that you could use to modify the value of the member the drawer is targetting. Use the 'gui' property to do all your drawings. You will also get a strongly typed property 'attribute' referencing the attribute your drawer targets.
  2. AttributeDrawer<T, A> where A is a DrawnAttribute. Just like ObjectDrawers, AttributeDrawers also define how a member looks like. Use an AttributeDrawer<T, A> if you want your drawer to only be used when you annotate your members with a certain Attribute. Examples of that are PopupDrawer, AnimVarDrawer and ShowTypeDrawer. PoupDrawer is only used on string if they're annotated with PopupAttribute. So PopupDrawer is a AttributeDrawer<string, PopupAttribute>. Again, you get OnGUI and a memberValue property. It does not make sense to annotate with multiple AttributeDrawers, ex annotate a string with both AnimVar and Popup as there will be a conflict and only one of them will be used.
  3. CompsoiteDrawer<T, A> where A : CompsoiteAttribute. CompositeDrawers are a bit different. They're not used to define a how a member should look like, instead they 'decorate' areas around the member. You don't get OnGUI, but OnLeftGUI, OnRightGUI, OnUpperGUI, OnLowerGUI and OnMemberDrawn(Rect) callbacks. The last callback gets called when a member is drawn, the rectangle of where the member is drawn gets passed. The rest of the callbacks are called in this order: OnUpperGUI, OnLeftGUI, OnRightGUI, OnBottomGUI. You could override any of those callbacks to decorate certain areas of your member. See WhiteSpaceDrawer as an example, it uses those callbacks to add space to the left/right/top/bottom of a member. See PathDrawer as an example using OnMemberDrawn callback. Just like AttributeDrawer, you also get 'memberValue', 'attribute' and 'gui'. Examples of Composites are CommentDrawer, WhiteSpaceDrawer, InlineDrawer, PathDrawer and many more. Unlike AttributeDrawer, you can apply multiple composite drawers on a member. You can define the order of which those drawers are applied/drawn via the 'id' property in CompositeAttribute. Lower comes first.
  4. Applying an attribute on a member that it can't handle will output an error, and then fallback to whatever the appropriate drawer is for that member. For example, applying Popup on a Transform, this doesn't make sense so you will get an error saying that Popup can't handle Trasnform, and your member will be drawn using UnityObjectDrawer.
  5. After you write your drawer, you must register/map it using the TypeDrawerMapper. See RegisterCustomDrawerExample.cs to see how it's done.
1. <a name="faq-member-order">**How are my class members ordered in the inspector?**</a>
  - The order is: fields, properties then methods. Fields are sorted by their order of declaration, same for properties, and methods. For ex: if you had a class where you define f1, p1, f2, m1, p2 (in that order. where f:field, p:property, m:method) you would see f1, f2, p1, p2, m1 in the inspector. You can use [DisplayOrder(#)] to explicitly define the member order. Note that having all members show up in the order they're declared in is very tricky ([see](https://www.assetstore.unity3d.com/en/#!/content/31442)) (if you know of a simpler way than what was suggested, please let me know).
1. <a name="faq-doesnt-serialization">**What *doesn't* the serialization system support?**</a>
  - The serialization system is meant to serialize types that Unity doesn't support. Serialization of Unity objects is handled by Unity itself. When the serializer comes across a UnityEngine.Object reference, it stores it in a serialized list of Unity objects, and serializes the index of where that storage took place in the list (it's basically a hack). That means, in order to properly serialize Unity objects to Stream, converters/surrogates must be written (Look forward to my FastSave which hopefully will be out soon)
1. <a name="faq-exact-serialization">**What are the exact serialization rules?**</a>
  1. Public fields are serialized by default
  1. Auto-properties with at least a public getter or setter are serialized by default
  1. Properties with side-effects (have bodies for their getter/setter) are never serialized
  1. Any non-public field or auto-property marked with [SerializeField] or [Serialize] are serialized
  1. Public fields/auto-properties are not serialized if marked with [DontSerialize] or [NonSerialized]. Note it's better to use [DontSerialize] because it can be used on Properties unlike [NonSerialized]
  1. All the previous applies on readonly fields
  1. For static fields/auto-properties, all the previous is applied if they're in a BetterBehaviour.
  They might not be serialized in System.Objects depending on the serializer of use. For ex,
  FullSerializer doesn't support serialization of static fields/auto-properties, but FastSerializer does (will be released soon)
  1. No need to mark types with any attribute to make them [Serializable] (in fact, it's recommended that you don't mark types with [Serializable], see note below)
1. <a name="faq-serialize-or-serialize-field">**So should I use [Serialize], or [SerializeField]? any reason to prefer one over the other?**</a>
  - *Short answer:* always use [Serialize]. *Long answer:* Since BetterBehaviour is a MonoBehaviour, that means all the Unity serialization rules still apply. That is, public fields (with serializable types) are serialized by default, non-public fields (with serializable types) are serialized only when annotated with [SerializeField].
That means, if you do have a public field or non-public marked with [SerializeField], it will be serialized twice! one by Unity, another by Vfw. So it's better to avoid friction with Unity's serialization system by making things only serializable once by Vfw (if you can help it) The way to achieve that for non-public fields is easy, you just don't use [SerializeField] but [Serialize] instead. For public fields, you could either use a public auto-property instead, or make the field readonly (works perfectly well if your field in public only because you want to
expose it in the inspector, and not modify it from code - my personal favorite approach). ***BUT*** if you do let only Vfw serialize your members, then any field initializer values will be ignored! So when you click "Reset" on a behaviour, members won't revert to their initialized values, but instead to their default ones. You could use the DefaultAttribute as a work around.
1. <a name="faq-converters">**How to write converters/surrogates to customize serialization for/add support to certain types?**</a>
  - For FullSerializer, see FS's repo for examples https://github.com/jacobdufault/fullserializer#
  Also see MethodInfoConverter.cs under Vexe/Runtime/Serialization/Serializers/FullSerializer
  After you write the converter, make sure to add it in FullSerializerBackend.cs upon initializing it
1. <a name="faq-view-state">**How to view the serialize state of a BetterBehaviour?**</a>
  - Just expand the script header foldout, from there you'll see the runtime serialization data
1. <a name="faq-how-to-serialize">**How do you serialize a BetterBehaviour to file?**</a>
  - *Short answer:* there's no direct support to do this, so don't do it. *Long answer:* Like we mentioned before, serialization of Unity objects is done by storing the object in a serializable list (by Unity) and serializing the index of the storage location. That doesn't play well with saving things to Stream (File for ex) because then the receiving end must have the same list available for deserialization to work, which isn't very practical. This is why the serialization system is mainly meant and designed for persisting data between assembly reloads. That said, you could still try to write the serialization data of a behaviour to file, it will work nicely with non-UnityObject members such as dictionary, list etc but not with UnityObject references.
