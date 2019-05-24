# GameObject

Game objects are objects that store components. GameObjects also store a list of children and a reference to it's parent (if one exists).

Essentially a gameobjet is a node in a tree data structure that can be traversed up and down the branches of the tree.

If a gameobject has no parent its considered a root object. Currently kau-rock supports multiple root gameobjects however this is subject to change.

## Properties

### Name *string*
    The name of the gameobject. Used to assist debugging and will be used by any future graphical tools.

### Active *boolean*
    This is currently not in use and has been added for future proofing.

### Parent *GameObject*
    The parent of this GameObject.

### Children *List<GameObject>*
    A list containing all game objects that have this game object as it's parent.

### Components *HashSet<Component>*
    A list containing all components for this game object.

### Transform *[Transform](components/transform.md)*
    The transform component of this game object. GameObjects will always have a transform.


## Methods

### *void* OnDestroy
    Used when the gameobject, it's children and all it's components need to be destroyed.
    Calling OnDestroy will call OnDestroy on all children followed by all OnDestroy on all components.

### *void* OnStart
    Used when the gameobject, it's children and all it's components need to be started. Either when the scene is started or when the game object is created.
    Calling OnStart will call OnStart on all children followed by all OnStart on all components.

### *void* SetParent
    Used to set the parent for this GameObject. If the parent is null the gameobject becomes a root object.