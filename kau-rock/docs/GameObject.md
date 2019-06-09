# GameObject

Essentially a gameobjet is a node in a tree data structure that can be traversed up and down the branches of the tree. Each game object also stores a list of components. These components add extra functionality to a game object, for example a Transform component to keep track of the game object's rotation and position.

If a gameobject has no parent its considered a root object. Once a gameobject has become a root object it's added to the list of Root Objects in the SceneManager.

## Properties

### Name *string*
> The name of the gameobject. Used to assist debugging and will be used by any future graphical tools.

### Active *boolean*
> This is currently not in use and has been added for future proofing.

### Parent *GameObject*
> The parent of this GameObject.

### Children *List<GameObject>*
> A list containing all game objects that have this game object as it's parent.

### Components *HashSet<Component>*
> A list containing all components for this game object.

### Transform *[Transform](components/transform.md)*
> The transform component of this game object. GameObjects will always have a transform.


## Methods

### *void* OnDestroy
    Used when the gameobject, it's children and all it's components need to be destroyed.
    Calling OnDestroy will call OnDestroy on all children followed by all OnDestroy on all components.

### *void* OnStart
    Used when the gameobject, it's children and all it's components need to be started. Either when the scene is started or when the game object is created.
    Calling OnStart will call OnStart on all children followed by all OnStart on all components.

### *void* SetParent
    Used to set the parent for this GameObject. If the parent is null the gameobject becomes a root object by adding itself to the SceneManager's list of Root Objects.

### *void* RmoveComponent