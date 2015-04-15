# Vexe Framework
The Vexe Framework (VFW) is both a runtime and editor extension that offers much more advance features than what comes out of the box with Unity. A better drawing API, a faster GUI layout system for editor drawing, a custom serialization system, tons of attributes/drawers, helper types and more. Here's an overview.

###1- A new drawing API that has the following features:
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

###2- An improved serialization system that has the following features:
1. Polymorphic serialization (interfaces and abstract/base system objects)
1. Generic types support. You can go crazy with nesting generics, you can nest types 'infinitely' and things will serialize
1. Only auto-properties are serialized (properties with side-effects are not serialized - instead, you just serialize the backing field behind it instead. This is done for good reasons - because you could be accessing Unity's API from your property, and that is not allowed when using ISerializationCallback - which I what I used to register my [de]serialization calls).
1. static fields/auto-properties serialization!
1. readonly fields serialization!
1. Structs and Nullables serialization.
1. Most common collections are supported: arrays (only one dimensional), lists (you could nest lists, depending on your serializer), dictionaries, stacks, queues, etc. (I didn't write a drawer for stack/queue yet... maybe I will make a tutorial of it).
1. Delegates are not directly serialized, but I wrote a new delegate system designed with simplicity, robustness and predictability in mind. Instead of saying Action<T>/Func<T>, just prefix that with a 'u', so uAction<T>/uFunc<T>
1. You have custom, shorter attributes for serialization: [Serialize], [DontSerialize]
1. ScriptableObjects support - Just like you have a BetterBehaviour, you have a BetterScriptableObject. Dealt with pretty much the same (same serialization features, same attributes, etc).
1. Serialization can be applied to any object, not just Mono/BetterBehaviours.

###3- Helper types:
1. The generic event system from uFAction.
1. BetterAnimator (an Animator wrapper that lets you easily modify the animator's floats, bools, ints, etc)
1. SelectionMemorizer: memorizes your objects selections. Ctrl.Shift.- to go back, Ctrl.Shift.+ to go forward.
1. BetterUndo: a simple undo system implemented via the command pattern that lets you take direct control of what is it to be done, and how it is to be undone
1. RabbitGUI: A much faster and cleaner GUI layouting system than Unity's GUILayout/EditorGUILayout with similar API and some extra features.

###4- Most the attributes from previous ShowEmAll, with some improvements/bug-fixes and a bunch of new ones:
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

###6- Screenshots:
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

### 7- Tutorials/Videos (read the video description to see what it covers):
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

###8- Known Issues:
1. Undo isn't 100% perfect still, (although it has been improved in 1.3)
1. Not everything is documented yet
1. No multi-object editing support yet

###9- Acknowledgments: 
1. Please understand that this is a one-man job, I try my best to make everything work, but I can't make everything perfect. I try to do my best isolating my code and writing unit tests for it to make everything's working OK, but a bug or two could easily slip in... If you want to write a program without bugs, then don't write it.
1. Apologies in advance if you come across a frustrating bug or any inconvenience due to using this framework, if you do, make sure you let met know.
1. For help, improvements/suggestions, bug reports: you could post here, pm me or email me at askvexe@gmail.com. I will try to reply ASAP and help as best as I can.
1. I always use the latest version of Unity for development. Before you report an issue, please make sure you're using the latest version too. If the issue is still there, report it.

###10- FAQ: 
1. **Q**: License? **A**: [MIT](http://choosealicense.com/licenses/mit/).
1. **Q**: Does this require Unity Pro? **A**: No.
1. **Q**: Does this support AOT platforms? **A**: Yes. It has been verified to work on iOS. While not manually verified on Android, but it should work.
1. **Q**: Why free? **A**: 1- Because I believe I can build a base "standard" framework that anyone could use in his own personal projects to benefit from and build upon. The current state in almost all asset developers is that they all come up with 'their' own solutions to the same problems over and over again. This makes it difficult for assets from different publishers to play nice with each other because their backbone is different. For ex say you purchased a property drawers asset that you'd like to use in say, Node Canvas. Well you can't because NC and the drawers asset use a different base for their codes thus making them incompatible. 2- This stuff should be available to us out-of-the-box let alone free... don't let me start uRanting. 3- Open source is beautiful. Everyone could contribute to the improvement of the software, fixing bugs, etc.
1. **Q**: How are my class members ordered in the inspector? **A**: **(from 1.2.9c onwards)** FieldsThenPropertiesThenMethods. Fields are sorted by their order of declaration, same for properties, and methods. For ex: if you had a class where you define f1, p1, f2, m1, p2 (in that order. where f:field, p:property, m:method) you would see f1, f2, p1, p2, m1 in the inspector. You can use [DisplayOrder(#)] to explicitly define the member order. Note that having all members show up in the order they're declared in is very tricky ([see](https://www.assetstore.unity3d.com/en/#!/content/31442)) (if you know of a simpler way than what was suggested, please let me know).

### 12- Latest version release notes (history file is included in the package)

**-- 1.3 --**

**--Source Control:**

- VFW is now on [github](https://github.com/vexe/VFW)!

**--Editor Fixes:**
- **(Finally, duh!)** Fixed a RabbitGUI layout issue where the editor disappears when changing playmode. Things should now look persistent, playmode state transitions should be a lot smoother.

**--Delegates:**
- handler target could now be any System.Object! (still no support for lambdas with captures)
- handler method is now MethodInfo instead of just a string. which means methods overloads are
now supported in the editor

**--Serialization:**
- Updated FullSerializer and included its source code. Which in turn seemed to address most the
serializaiton issues a lot of users started having after Unity 4.6.3
- Renamed BetterSerializer to SerializerBackend with the FullSerializer related stuff
implemented in FullSerializeBackend (as there will soon be a 'FastSerializerBackend' ;)

**--New:**
- Dynamic reflection system! (included within vRuntime.dll) - you can support me and buy the source code if you're interested (see)
- Inline support for AudioSource
- DoubleDrawer to handle drawing System.Double
- VFWSettings asset for global settings that get applied to newly instantiated behaviours.
found under Plugins/Editor/Vexe/ScriptableAssets or hit the menu Tools/Vexe/VFWSettings
- More customization options on how your behaviour looks:
a) more options in the 'Display' popup when collapsing the script header
b) a spacing field to change members' indentation
- MethodInfoConverter, for serializing MethodInfos

**--Removed:**
- SaveAttribute and DontSaveAttribute. Since they will be used later in FastSave
- Basic/Full/MinimalViewAttributes. Things are always categorized now, everything is customized
the in display options now!
- IgnoreCategoriesAttribute
- GUICode method in BaseEditor and replaced it with OnGUI. That way you could totally override
OnGUI. If you want to call 'base', you can just call it...
- Fasterflect (no need for it, since all I was using it for was its delegate API, which is what my DynamicReflection is for)
- colorValue from DefaultAttribute
- vEditor.dll (it was only a couple of files which are now under Plugins/Editor/Vexe/Helpers)
- Dbg category string constant from Base behaviour classes. Want one? good, let it in your own scripts
- The plural stuff (Lists of lists) from BetterPrefs

**--Improvements/Modifications:**
- DictionaryDrawer now works with any IDictionary
- Seperated visibility attributes/logic from serialization attributes/logic
- Improved overall undo support. Ctrl-z cover more cases. Also brought back BetterUndo so if
Ctrl-z doesn't wory, try Alt-Ctrl-u
