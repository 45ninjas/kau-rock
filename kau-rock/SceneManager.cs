using System.Collections.Generic;

namespace KauRock {
    public static class SceneManager {
        public static List<GameObject> RootObjects = new List<GameObject>();
        public static void Start() {
            Log.Debug(typeof(SceneManager), $"Started with {RootObjects.Count} RootObject(s).");

            foreach (var gameObject in RootObjects)
            {
                gameObject.OnStart();
            }
        }
        public static void DestroyAll() {
            foreach (var gameObject in RootObjects)
            {
                gameObject.OnDestroy();
            }
        }
    }
}