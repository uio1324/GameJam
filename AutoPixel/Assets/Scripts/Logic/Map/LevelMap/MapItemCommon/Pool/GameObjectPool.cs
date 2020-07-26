using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Map.LevelMap.MapItemCommon.Component;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon.Pool
{
    interface IGameObjectPool
    {
        IEnumerator PreInit();
        MapItemComponent Get();
        MapItemComponent Alloc();
    }
    public class GameObjectPool : MonoBehaviour, IGameObjectPool
    {
        protected GameObject m_prefab;
        private Stack<MapItemComponent> m_pool;
        private int m_preInitNums = 6;
        private static Dictionary<MapItemComponent, GameObjectPool> s_returnCache;

        public bool Serializable
        {
            get;
            protected set;
        }
        public virtual IEnumerator PreInit()
        {
            if (s_returnCache == null)
            {
                s_returnCache = new Dictionary<MapItemComponent, GameObjectPool>();
            }
            
            m_pool = new Stack<MapItemComponent>();

            for (var i = 0; i < m_preInitNums; i++)
            {
                m_pool.Push(Alloc());
            }

            yield return null;
        }
        
        public virtual MapItemComponent Get()
        {
            if (m_pool.Count > 0)
            {
                var o = m_pool.Pop();
                SetMapItemAppear(o);
                return o;
            }
            else
            {
                var o = Alloc();
                SetMapItemAppear(o);
                return o;
            }
        }

        private void SetMapItemAppear(MapItemComponent component)
        {
            component.gameObject.SetActive(true);
            s_returnCache.Add(component, this);
        }
        private void SetMapItemDisappear(MapItemComponent component)
        {
            ResetMapItem(component);
            s_returnCache.Remove(component);
        }

        private void ResetMapItem(MapItemComponent component)
        {
            component.gameObject.SetActive(false);
            var comTrans = component.transform;
            comTrans.position = Vector3.zero;
            comTrans.rotation = Quaternion.identity;
        }

        public static void Return(MapItemComponent o)
        {
            if (s_returnCache.TryGetValue(o, out var outValue))
            {
                outValue.SetMapItemDisappear(o);
                outValue.m_pool.Push(o);
            }
            else
            {
                Debug.LogError("returnCache异常");
            }
        }
        
        /// <summary>
        /// alloc出来会将对象置为inactive
        /// </summary>
        /// <returns></returns>
        public virtual MapItemComponent Alloc()
        {
            if (m_prefab)
            {
                var o = Instantiate(m_prefab, transform);
                o.SetActive(false);
                return o.GetComponent<MapItemComponent>();
            }
            else
            {
                var o = new GameObject();
                o.SetActive(false);
                return o.GetComponent<MapItemComponent>();
            }
        }
    }
}