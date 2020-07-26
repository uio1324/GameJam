using System.Collections.Generic;

namespace DataStructure.Queue
{
    public class PriorityQueue<T>
    {
        private IComparer<T> m_comparer;
        private T[] m_heap;

        public int Count
        {
            get;
            private set;
        }
    }
}