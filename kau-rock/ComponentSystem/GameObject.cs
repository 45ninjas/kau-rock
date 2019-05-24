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

        public GameObject(string name) {
            Transform = new Transform(this);
            Name = name;

        }
        
        public GameObject(GameObject parent, string name) {
            Name = name;
            Transform = new Transform(this);
            SetParent(parent);
        }

        public void OnStart() {
            // Start the children first.
            foreach(var child in Children) {
                child.OnStart();
            }

            // Then start the components.
            foreach(var component in Components) {
                component.OnStart();
            }
        }

        public void OnDestroy() {
            // Destroy the children first.
            foreach(var child in Children) {
                child.OnDestroy();
            }

            // Then destroy the components.
            foreach(var component in Components) {
                component.OnDestroy();
            }
        }
        
        public void SetParent(GameObject parent) {
            // Remove ourselves from our existing parent.
            if(Parent != null) {
                Parent.Children.Remove(this);
            }

            // Set the parent to the new
            Parent = parent;

            // Add ourselves to the parents list of children.
            if(Parent != null) {
                Parent.Children.Add(parent);
            }
        }

        public void AddComponent(Component component) {
            if(Components.Contains(component))
            {
                Log.Warning(this, "Unable to add component {0} because it already exists.", component);
                return;
            }
            Components.Add(component);
        }
        public void RemoveComponent(Component component) {
            if(!Components.Contains(component)) {
                Log.Warning(this, "Unable to remove component {0} as it doesn't exist.", component);
                return;
            }
            Components.Remove(component);
        }
    }
}