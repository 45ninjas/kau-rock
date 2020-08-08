using System;
using System.Collections.Generic;
using OpenTK;

namespace KauRock {
  public class GameObject {

    public String Name;

    public bool Active = true;
    public GameObject Parent { get; private set; }

    public readonly List<GameObject> Children = new List<GameObject>();

    public readonly HashSet<Component> Components = new HashSet<Component>();

    public readonly Transform Transform;

    private bool hasStarted = false;

    public GameObject (string name) {
      Transform = new Transform( this );
      Name = name;
      SetParent( null );
    }

    public GameObject (GameObject parent, string name) {
      Name = name;
      Transform = new Transform( this );
      SetParent( parent );
    }

    public void OnStart () {
      hasStarted = true;
      // Start the children first.
      foreach ( var child in Children ) {
        child.OnStart();
      }

      // Then start the components.
      foreach ( var component in Components ) {
        component.OnStart();
      }
    }

    public void OnDestroy () {
      // Call this function on each child.
      foreach ( var child in Children ) {
        child.OnDestroy();
      }

      // Clear all children.
      Children.Clear();

      // Call OnDestroy on each component so they have time to do things.
      foreach ( var component in Components ) {
        component.OnDestroy();
      }

      // Clear the components.
      Components.Clear();
    }

    public void SetParent (GameObject newParent) {
      // Remove ourselves from our existing parent.
      if ( Parent != null )
        Parent.Children.Remove( this );
      // Or remove ourselves from the list of root objects only if we have a new parent.
      else if ( newParent != null )
        SceneManager.RootObjects.Remove( this );

      // Set the parent to the new
      Parent = newParent;

      // Add ourselves to the children list of our new parent.
      if ( Parent != null )
        Parent.Children.Add( newParent );
      // Or add ourselves to the list of root objects.
      else
        SceneManager.RootObjects.Add( this );
    }

    public void AddComponent (Component component) {
      if ( Components.Contains( component ) ) {
        Log.Warning( this, "Unable to add component {0} because it already exists.", component );
        return;
      }

      // Add the component.
      Components.Add( component );

      // Call OnStart if the game object has already started.
      if ( hasStarted ) {
        Log.Info( this, "Gameobject has already started, Starting component" );
        component.OnStart();
      }
    }
    public void RemoveComponent (Component component) {
      if ( !Components.Contains( component ) ) {
        Log.Warning( this, "Unable to remove component {0} as it doesn't exist.", component );
        return;
      }
      Components.Remove( component );
    }
  }
}