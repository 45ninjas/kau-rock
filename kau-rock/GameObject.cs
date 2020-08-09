using System;
using System.Collections.Generic;
using OpenTK;

namespace KauRock {
  public class GameObject {

    public String Name;

    internal bool initialised = false;
    public bool Enabled { get; private set; }
    public GameObject Parent { get; private set; }

    public readonly List<GameObject> Children = new List<GameObject>();

    public readonly HashSet<Component> Components = new HashSet<Component>();

    public readonly Transform Transform;

    public GameObject (GameObject parent, string name, bool enabled, params Component[] components) {
      Transform = new Transform();
      AddComponent( Transform );

      Name = name;
      if ( parent == null ) {
        Log.Warning( this, $"Setting parent of {name} to null." );
        SetParent( parent );
      }
      SetEnabled(enabled);
      initialised = true;

      AddComponents( components );
    }

    public void SetEnabled (bool value) {
      if ( Enabled != value ) {
        Enabled = value;

        if ( value ) {
          foreach ( var comp in Components ) {
            comp.OnEnabled();
          }
        }
        else {
          foreach ( var comp in Components ) {
            comp.OnDisabled();
          }
        }
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

    public virtual void SetParent (GameObject newParent) {
      if ( Parent != null )
        Parent.Children.Remove( this );

      Parent = newParent;
      Parent.Children.Add( this );
    }

    private void AddComponents (IEnumerable<Component> components) {
      foreach ( var component in components ) {
        AddComponent( component );
      }
    }

    public void AddComponent (Component component) {
      if ( Components.Contains( component ) ) {
        Log.Warning( this, "Unable to add component {0} because it already exists.", component );
        return;
      }

      // Add the component and enable it.
      Components.Add( component );
      component.GameObject = this;

      // Queue this component for OnStart.
      Root.NewComponents.Enqueue( component );
    }
    public void RemoveComponent (Component component) {
      if ( !Components.Contains( component ) ) {
        Log.Warning( this, "Unable to remove component {0} as it doesn't exist.", component );
        return;
      }
      component.OnDestroy();
      Components.Remove( component );
    }
  }
}