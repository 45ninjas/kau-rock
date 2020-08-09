using System;
using System.Collections.Generic;
using OpenTK;

namespace KauRock {
  public class Root : GameObject {

    internal static Queue<Component> NewComponents = new Queue<Component>();

    public Root () : base( null, "Root", false) {
      Events.UpdateFirst += OnUpdateFirst;
    }

    // Invoke Start for the first frame of any new components.
    private static void OnUpdateFirst () {
      while(NewComponents.TryDequeue(out var newComponent)) {
        newComponent.OnStart();
      }
    }

    public override void SetParent (GameObject newParent) { }
  }
}