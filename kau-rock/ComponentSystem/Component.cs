namespace KauRock {
  public class Component {
    public GameObject GameObject;

    public virtual void OnStart() {
      // The gameobject has been started.
    }
    public virtual void OnDestroy() {
      // The gameobject is beging destoyed.
    }
  }
}