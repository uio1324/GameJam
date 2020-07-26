using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Common.Singleton;
using UnityEngine;
using Logic.Core;

namespace Logic.Manager.InputManager
{
    [ManagerDefine(90, false)]
    public sealed class InputManager : Manager<InputManager>, IManager
    {
        public Vector2 Axis
        {
            get
            {
                Vector2 finalAxis = m_buttonAxis + m_keyAxis;
                finalAxis.x = Mathf.Clamp(finalAxis.x, -1.0f, +1.0f);
                finalAxis.y = Mathf.Clamp(finalAxis.y, -1.0f, +1.0f);
                return finalAxis;
            }
        }
        public Vector2 ButtonAxis
        {
            set
            {
                value.x = Mathf.Clamp(value.x, -1, 1);
                value.y = Mathf.Clamp(value.y, -1, 1);
                m_buttonAxis = value;
            }
        }
        public Vector2 KeyAxis
        {
            set
            {
                value.x = Mathf.Clamp(value.x, -1, 1);
                value.y = Mathf.Clamp(value.y, -1, 1);
                m_keyAxis = value;
            }
        }
        private Vector2 m_keyAxis;
        private Vector2 m_buttonAxis;
        private Dictionary<int, HashSet<Action>> m_inputActions;

        public override void OnAwake()
        {
            m_inputActions = new Dictionary<int, HashSet<Action>>();
            if(GameRoot.m_instance)
            {
                GameRoot.m_instance.AddManager(this);
            }
        }

        public IEnumerator PreInit()
        {
            yield return null;
        }

        void ISingleton.Update()
        {
            this.KeyAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxis("Vertical"));

            var keys = m_inputActions.Keys;
            foreach (var key in keys)
            {
                if (Input.GetKeyDown((KeyCode)key))
                {
                    if (m_inputActions.TryGetValue(key, out var actions))
                    {
                        foreach (var action in actions)
                        {
                            action.Invoke();
                        }
                    }
                }
            }
        }

        public void AddCallback(KeyCode key, Action action)
        {
            if (m_inputActions.TryGetValue((int)key, out var actions))
            {
                if (!actions.Contains(action))
                {
                    actions.Add(action);
                }
                else
                {
                    Debug.LogError("你试图添加一个已存在的回调");
                }
            }
            else
            {
                HashSet<Action> hashSet = new HashSet<Action> { action };
                m_inputActions.Add((int)key, hashSet);
            }
        }

        public void RemoveCallback(KeyCode key, Action action)
        {
            var actions = m_inputActions[(int)key];
            if (actions.Contains(action))
            {
                actions.Remove(action);
            }
            else
            {
                Debug.LogError("你试图移除一个未注册的回调");
            }
        }
    }
}
