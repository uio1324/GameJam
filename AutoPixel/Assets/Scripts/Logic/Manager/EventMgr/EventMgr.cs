using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Manager.EventMgr
{
    [ManagerDefine(50, false)]
    public sealed class EventMgr : Manager<EventMgr> , IManager
    {
        private Dictionary<int, HashSet<Action>> m_callbacks;
        public override void OnAwake()
        {
            m_callbacks = new Dictionary<int, HashSet<Action>>();
        }

        public IEnumerator PreInit()
        {
            yield return null;
        }

        public void Register(int eventType, Action callback)
        {
            if (m_callbacks.TryGetValue(eventType, out var actions))
            {
                if (!actions.Contains(callback))
                {
                    actions.Add(callback);
                }
                else
                {
                    Debug.LogError("你试图添加一个已存在的回调");
                }
            }
            else
            {
                HashSet<Action> hashSet = new HashSet<Action> {callback};
                m_callbacks.Add(eventType, hashSet);
            }
        }

        public void UnRegister(int eventType, Action callback)
        {
            if (m_callbacks.TryGetValue(eventType, out var actions))
            {
                if (actions.Contains(callback))
                {
                    actions.Remove(callback);
                }
                else
                {
                    Debug.LogError("你试图移除一个未注册的回调");
                }
            }
        }

        public void Dispatch(int eventType)
        {
            var actions = m_callbacks[eventType];
            foreach (var action in actions)
            {
                action.Invoke();
            }
        }
    }
}