_________________
*** Fast.Save ***
-----------------

* What is it?
- It's a simple and fast runtime saving/loading (80%) solution for Unity Components, GameObjects and other System.Objects as well.
- You could write/read to/from any Stream (File, Memory, etc)
- Could be used to build other systems (persist changes in playmode, clone Components at runtime etc)
- It has a dependency on VFW (runtime extensions/helpers and editor codes)
- Should work on AOT platforms, not tested though

* What is it NOT?
- A 100% solution
- A generic saving/loading soultion that works *out of the box* for *any game*
- A tool to help artists

* License?
- MIT

* Components support?
- Transform,
- Rigidbody, Rigidbody2D,
- MeshFilter,
- Renderers (MeshRenderer, SkinnedMeshRenderer, TrailRenderer, ParticleRenderer)
- Animator, ParticleAnimator,
- Camera,
- CharacterController,
- AudioSource,
- 3D Colliders (BoxCollider, CapsuleCollider, MeshCollider, SphereCollider)
- 2D Colliders (BoxCollider2D, CircleCollider2D, PolygonCollider2D, EdgeCollider2D)
- 2D Effectors (AreaEffector2D, PlatformEffector2D, PointEffector2D, SurfaceEffector2D)
- 2D Joints    (DistanceJoint2D, HingeJoint2D, SliderJoint2D, SpringJoint2D, WheelJoint2D)
- ConstantForce2D
- Support to other components is as easy as typing what fields to serialize (by name) in that component.

* Assets?
- Mesh, AudioClip, Material, PhysicMaterial, PhysicsMaterial2D, Flare,
- GUIStyle, Texture, RuntimeAnimatorController, AnimationClip,
- and pretty much any other type of Asset (TextAsset etc)

* System.Objects?
- Arrays (1D), Lists, Stacks, Queues, Dictionaries are supported.

* Polymorphic serialization, Cycle support, serialize by reference, string caching?
- The serializer has two modes: Standard (1) and Minimal (0). By default it's set to 1.
  Which means all of these features are enabled. Set the mode to 0 and you'd only get string caching.
- Polymorphic component serialization is supported by default.
  To disable: ReflectiveComponentSerializer.SetRTI(false); // don't store runtime type info

* What's the underlying serializer?
- A home-cooked reflection-based binary serializer: BinaryX 2.0
- Uses Fast.Reflection (dynamic IL emission to generate getters/setters for fields/properties)
  for standalone builds and regular reflection otherwise.

* Why a home-cooked serializer?
- Excellent learning experience (this is my second serializer :p)
- I like to have total control over my toys. Know all the ins and outs. If there's ever a bug
  I can fix it my self, if I ever wanted a new feature I can implement it myself instead of having
  to wait for somebody else's reply/permission.
- Most existing serializers don't meet my requirements: Speed, Simplicity and high Customizability.
  Take BinaryFormatter for example, it's slow, stores tons of metadata and not customizable (requires Serializable attribute on types)
  Take protobuf-net, it's fast but has some limitations I don't like. e.g. serializing nested
  generic lists. Not very customizable, requires you to clutter your code with annotations
  everywhere, you can't change what attributes to annotate with. You can't change the serializer logic etc.
  Other serializers (JSON, etc) are just painfully slow.

* 2.0? what happened to 1.0?
- That's the genius part, we skip it. Who needs 1.0 if there's 2.0?

* Usage?
- The public API is split into a 'Save' and 'Load' classes
- You can Save/Load GameObjects, Components, Hierarchy or Marked (a bit on that later)
  to/from Stream, Memory and File. So you can say:
    Save.[GameObject|Component|Hierarchy|Marked]To[Stream|Memory|File] and
    Load.[GameObject|Component|Hierarchy|Marked]From[Stream|Memory|File]

- When saving, you provide an instance to save, when loading you provide an instance to load 'into'
- Example:
    byte[] bytes = Save.GameObjectToMemory(target);
    string str   = bytes.GetString();
    PlayerPrefs.SetString("Target", str);
    // ...
    string str   = PlayerPrefs.GetString("Target");
    byte[] bytes = str.GetBytes();
    Load.GameObjectFromStream(bytes, target); // load/deserialize 'into' target

    Notes:
    1- GetString and GetBytes are extension methods in System
    2- You can also use the extension methods in FSExtensions.cs so you can say:
       byte[] bytes = myGo.SaveToMemory(memoryStream);
    3- You can also save/load system objects, via: Save.ObjectToX and Load.ObjectFromX

(FSMarker)
- You can selectively mark GameObjects to save/load by adding a FSMarker to them.
-- From the marker, you choose what components to save/load within those GameObjects.
-- Then you can:
      // finds all marked GameObject and save whatever marked components in them
      byte[] bytes = Save.MarkedToMemory();
      // loads the saved data into the same marked objects.
      // If one of the previously saved marked objects is destroyed,
      // its data gets ignored when loading
      Load.MarkedFromMemory(bytes);
-- There's callbacks for when saving/loading marked objects begins/finishes.
    Save.OnBeganSavingMarked, Save.OnFinishedSavingMarked,
    Load.OnBeganLoadingMarked and Load.OnFinishedLoadingMarked.

[Save], [DontSave]
- You can customize the saving/loading of your script fields:
-- Public fields/auto-properties in scripts are saved by default.
-- Non-public ones are saved only if marked with [Save]
-- You can explicitly marked a member not to be saved via [DontSave]
-- The reason that we request non-public fields to be marked with [Save],
   is because there are instances where you have a field that you care about
   saving, but not necessarily 'serialize' for persistency (via SerializeField)
   Think of a bomb timer, the timer starts, you save the game in the middle of the timer,
   when loading you want the timer to resume where it left off.
   In this case you might not be interested in persisting the timer via [SerializeField],
   but only in Saving it via [Save]

(FSReference)
- If you have a script that references an in-scene Component or GameObject
  and you want to save that reference, you have to attach a FSReference
  to the GameObject your referenced component is attached to
  or to the GameObject you're referencing. e.g.

  public class MyScript : MonoBehaviour
  {
      public GameObject  someGo;
      public BoxCollider someCollider;
  }

  If you wanted to save those references, an FSReference component has to be attached to
  the GameObject 'someGo' is referencing, and another on the GameObject 'someCollider'
  is attached to.

  Note: You don't have to explicitly add it yourself. When the serializer serializes
        a reference that is not a prefab or a component attached to a prefab,
        it will automatically add a FSReference if it's not already there.

(PrefabStorage)
- If you have a script that references a prefab (or a Component that's attached to a prefab)
  and you want to save that reference, you have to create a PrefabStorage asset under a Resources
  folder and populate it with your prefabs. When you import Fast.Save there should already be
  a PrefabStorage under FastSave/Resources. To populate it just click the "Populate" button.
  This will look for all the prefabs in your project and add them to the store.

  Now when the serializer comes across a prefab reference, it just looks up its id (index)
  from the store and serializes it. When loading, we read the id and get the prefab from
  the store at that id number.

(AssetStorage)
- The point of this system is to let you save/load your game in the most effecient and fastest way
  regardless of how it's done. For that reason, Assets (textures, meshes, audio clips etc) are not
  serialized in the sense that we get their bytes data. I figured they're already living in the project,
  we already have them, so why bother getting their bytes? why not just reference them?
  This is that idea of AssetStorage. Similar to PrefabStorage, it stores references to assets.
  You don't need to do anything for it to work. Just make sure there's a AssetStorage asset under
  a Resources folder. There's already one created for you under FastSave/Resources.

  Notes:
  1- Your store *MUST* be populated before you build your game. Otherwise when you play your bulid
     assets not included in the store won't be loaded properly.
     To populate the store with the assets you want to save, you just simply play and save the game
     from the editor.
  2- The system curretnly uses a single storage for all assets. This is not a problem unless you're
     saving *huge* amounts of assets. You probably don't want assets from the first level of your game
     to be still loaded at the last one. You can simply work around that by using multiple storages,
     maybe each storage stores assets referenced in a single scene, when you're done with that scene,
     you clear out its storage.
     Multiple-storage solution is not included out of the box in the system as of yet.
     Because the most common things to save are position of things, AI states, player data etc.
     Even if you had a giant game with tons of assets, do you really want to save all of them?

* Current limitations:
- Assets (textures, audio clips etc) referenced by components you're saving *must* have unique names
- Components you're saving in GameObjects (via Save.GameObjectToX or Save.MarkedToX or Save.HierarchyToX)
  *must* be of unique types i.e. you can't have 2 Rigidbodies on a GameObject you want to save,
  or say, X and Y, where both X and Y 'is a' (inherits) Z.
- No interface with other serializers (JSON etc). Just a home-cooked Binary serializer for now.
- No way to extend the system/add support for new component types without modifying existing code
- No versioning/backward type compatibility from the serializer
- No error/status codes returned from the saving/loading methods (will eventually be added but not
  sure if it's really necessary for all methods and if it is what is the right approach for it)

Hope you find it useful! Cheers!
- vexe
