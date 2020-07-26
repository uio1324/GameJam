using System.Collections;
using Logic.Common.Singleton;

namespace Logic.Manager
{
    public interface IManager : ISingleton
    {
        IEnumerator PreInit();
    }
    public abstract class Manager<T> : Singleton<T> where T :Singleton<T>, IManager, new()
    {
        
    }
}