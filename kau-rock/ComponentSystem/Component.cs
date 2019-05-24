namespace KauRock {
    public class Component {
        public bool Active = true;
        public readonly GameObject GameObject;
        public Component (GameObject gameObject) {
            // Add this component to the game object.
            gameObject.AddComponent(this);
            // Set the GameObject of this component too.
            GameObject = gameObject;
        }

        public virtual void OnStart() {
            // The gameobject has been started.
        }
        public virtual void OnDestroy() {
            // The gameobject is beging destoyed.
            GameObject.RemoveComponent(this);
            Active = false;
        }
    }
}