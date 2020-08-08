using System;
namespace KauRock.Loaders {
  public interface ILoader<T> : System.IDisposable {
    T Load (string path);
  }
}