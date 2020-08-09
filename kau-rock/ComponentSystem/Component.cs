namespace KauRock {
  public class Component {
    public GameObject GameObject;

    private bool enabled = true;
    public bool Enabled => enabled;

    public virtual void OnDisabled() {
      Log.Debug(this, "WTF IS GOING ON?");
    }
    public virtual void OnEnabled() {
      Log.Debug(this, "HOW THE FUCK?");
    }

    public virtual void OnStart() {
      // The first frame that has ran on this gameObject.
    }
    public virtual void OnDestroy() {
      // The gameobject is being destroyed.
    }
  }
}