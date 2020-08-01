using System.Collections.Generic;
using JetBrains.Annotations;

namespace Logic.Common.Pool
{
    public class Pool<T> where T : class, new()
    {
        protected Queue<T> m_objects;
        protected HashSet<T> m_usedObjects;

        public Pool()
        {
            m_objects = new Queue<T>();
            m_usedObjects = new HashSet<T>();
        }

        public void Push([NotNull]T obj)
        {
            m_objects.Enqueue(obj);
        }

        public T Get()
        {
            if (m_objects.Count > 0)
            {
                return m_objects.Dequeue();
            }
            var obj = new T();
            m_usedObjects.Add(obj);
            return obj;
        }

        public void Return([NotNull]T obj)
        {
            m_usedObjects.Remove(obj);
            m_objects.Enqueue(obj);
        }

        public void PreInit(int nums)
        {
            if (m_objects.Count > 0)
            {
                return;
            }
            for (var i = 0; i < nums; i++)
            {
                Push(new T());
            }
        }
    }
}