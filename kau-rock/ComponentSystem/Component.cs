namespace KauRock {
  public class Component {
    public GameObject GameObject;

    private bool enabled = true;
    public bool Enabled {
      get => enabled;
      set {
        if(value != enabled) {
          if(value)
            OnEnabled();
          else
            OnDisabled();
        }
        enabled = value;
      }
    }

    public virtual void OnDisabled() {
    }
    public virtual void OnEnabled() {
    }

    public virtual void OnStart() {
      // The first frame that has ran on this gameObject.
    }
    public virtual void OnDestroy() {
      // The gameobject is being destroyed.
    }
  }
}