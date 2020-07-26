using System;

namespace Logic.Manager.EventMgr
{
    public class EventTypeAttribute : Attribute
    {
        public int m_eventType;

        public EventTypeAttribute(int eventType)
        {
            m_eventType = eventType;
        }
    }
}