namespace Logic.Common.Singleton
{
    public interface ISingleton
    {
        void OnAwake();
        void OnDestroy();
    }

    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        private static T s_instance;
        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    CreateInstance();
                }

                return s_instance;
            }
        }

        private static void CreateInstance()
        {
            if (s_instance == null)
            {
                s_instance = new T();
                s_instance.OnAwake();
            }
        }

        public virtual void OnAwake()
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }
    }
    
}