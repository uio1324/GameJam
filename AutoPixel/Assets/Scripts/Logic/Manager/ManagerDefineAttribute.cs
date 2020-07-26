using System;

namespace Logic.Manager
{
    public class ManagerDefineAttribute : Attribute
    {
        public uint m_priority;
        public bool m_needPreInit;

        public ManagerDefineAttribute(uint priority, bool needPreInit)
        {
            m_priority = priority;
            m_needPreInit = needPreInit;
        }
    }
}