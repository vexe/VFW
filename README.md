## Vexe Framework (VFW)

### What:
  - VFW is both a runtime and editor extension that offers much more advance features than what comes out of the box with Unity. A better drawing API, a faster GUI layout system for editor drawing, a custom serialization system, tons of attributes/drawers, helper types and more. 
 
### Usage:
  - Just inherit BetterBehaviour instead of MonoBehaviour and BetterScriptableObject instead of ScriptableObject, and don't forget to add your using statement:

```
using Vexe.Runtime.Types;
using etc;

public TestBehaviour|TestObject : BetterBehaviour|BetterScriptableObject
{
    public Dictionary<int, GameObject> someDictionary;
    public uAction someAction;
    public List<int[]> someList { get; set; }
}
```

### Here's bird's eye-view for some of the features:

####1- A new drawing API that has the following features:
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

####2- An improved serialization system that has the following features:
1. Polymorphic types (interfaces and abstract/base system objects)
1. Generic types. (you can go crazy with nesting generics. i.e. Dictionary<int, Dictionary<List<ISomeInterface>>>)
1. Auto-properties (properties with side-effects are *not* serialized. Instead, you just serialize the backing field behind it)
1. static fields/auto-properties can be serialized *only* in BetterBehaviours, not in any arbitrary System.Object
1. readonly fields!
1. Structs, custom classes and Nullables!
1. Most common collection types: arrays (one dimensional), List<T>, Dictionary<TK, TV>, Stack<T>, Queue<T> and HashSet<T>
1. Delegates support with Action/Func-like equivalents: uAction and uFunc (supports up to 4 type arguments)
1. You can have your own serialization logic with your own attributes! (more on the default serialization logic/attributes shortly)


####3- Helper types:
1. The generic event system from uFAction.
1. BetterAnimator (an Animator wrapper that lets you easily modify the animator's floats, bools, ints, etc)
1. SelectionMemorizer: memorizes your objects selections. Ctrl.Shift.- to go back, Ctrl.Shift.+ to go forward.
1. BetterUndo: a simple undo system implemented via the command pattern that lets you take direct control of what is it to be done, and how it is to be undone
1. RabbitGUI: A much faster and cleaner GUI layouting system than Unity's GUILayout/EditorGUILayout with similar API and some extra features.

####4- Tons of property attributes:
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
1. **License?** 
  - [MIT](http://choosealicense.com/licenses/mit/).
1. **Does this require Unity Pro?** 
  - No
1. **Does this support AOT platforms?**
  - Yes. It has been verified to work on iOS. While not manually verified on Android, but it should work.
1. **Why free?** 
  1. Because I believe I can build a base "standard" framework that anyone could use in his own personal projects to benefit from and build upon. The current state in almost all asset developers is that they all come up with 'their' own solutions to the same problems over and over again. This makes it difficult for assets from different publishers to play nice with each other because their backbone is different. For ex say you purchased a property drawers asset that you'd like to use in say, Node Canvas. Well you can't because NC and the drawers asset use a different base for their codes thus making them incompatible. 
  2. This stuff should be available to us out-of-the-box let alone free... don't let me start uRanting. 
  3. Open source is beautiful. Everyone could contribute to the improvement of the software, fixing bugs, etc.
1. **How are my class members ordered in the inspector?**      
  - The order is: fields, properties then methods. Fields are sorted by their order of declaration, same for properties, and methods. For ex: if you had a class where you define f1, p1, f2, m1, p2 (in that order. where f:field, p:property, m:method) you would see f1, f2, p1, p2, m1 in the inspector. You can use [DisplayOrder(#)] to explicitly define the member order. Note that having all members show up in the order they're declared in is very tricky ([see](https://www.assetstore.unity3d.com/en/#!/content/31442)) (if you know of a simpler way than what was suggested, please let me know).
1. **What *doesn't* the serialization system support?**
  - The serialization system is meant to serialize types that Unity doesn't support. Serialization of Unity objects is handled by Unity itself. When the serializer comes across a UnityEngine.Object reference, it stores it in a serialized list of Unity objects, and serializes the index of where that storage took place in the list (it's basically a hack). That means, in order to properly serialize Unity objects to Stream, converters/surrogates must be written (Look forward to my FastSave which hopefully will be out soon)
1. **What are ehe exact serialization rules?**
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
1. **So should I use [Serialize], or [SerializeField]? any reason to prefer one over the other?**
  - *Short answer:* always use [Serialize]. *Long answer:* Since BetterBehaviour is a MonoBehaviour, that means all the Unity serialization rules still apply. That is, public fields (with serializable types) are serialized by default, non-public fields (with serializable types) are serialized only when annotated with [SerializeField].
That means, if you do have a public field or non-public marked with [SerializeField], it will be serialized twice! one by Unity, another by Vfw. So it's better to avoid friction with Unity's serialization system by making things only serializable once by Vfw (if you can help it) The way to achieve that for non-public fields is easy, you just don't use [SerializeField] but [Serialize] instead. For public fields, you could either use a public auto-property instead, or make the field readonly (works perfectly well if your field in public only because you want to
expose it in the inspector, and not modify it from code - my personal favorite approach)
1. **How to write converters/surrogates to customize serialization for/add support to certain types?**
  - For FullSerializer, see FS's repo for examples https://github.com/jacobdufault/fullserializer#
  Also see MethodInfoConverter.cs under Vexe/Runtime/Serialization/Serializers/FullSerializer
  After you write the converter, make sure to add it in FullSerializerBackend.cs upon initializing it
1. **How to view the serialize state of a BetterBehaviour?**
  - Just collapse the script header foldout, from there you'll see the runtime serialization data
1. **How do you serialize a BetterBehaviour to file?**
  - *Short answer:* there's no direct support to do this, so don't do it. *Long answer:* Like we mentioned before, serialization of Unity objects is done by storing the object in a serializable list (by Unity) and serializing the index of the storage location. That doesn't play well with saving things to Stream (File for ex) because then the receiving end must have the same list available for deserialization to work, which isn't very practical. This is why the serialization system is mainly meant and designed for persisting data between assembly reloads. That said, you could still try to write the serialization data of a behaviour to file, it will work nicely with non-UnityObject members such as dictionary, list etc but not with UnityObject references.



