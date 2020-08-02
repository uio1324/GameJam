using System.Collections;
using Logic.Common.Singleton;

namespace Logic.Manager
{
    public interface IManager : ISingleton
    {
        IEnumerator PreInit();
        void Update();
    }
    public abstract class Manager<T> : Singleton<T>, IManager where T :Manager<T>, new()
    {
        public virtual IEnumerator PreInit()
        {
            yield return null;
        }

        public virtual void Update()
        {
            
        }
    }
}