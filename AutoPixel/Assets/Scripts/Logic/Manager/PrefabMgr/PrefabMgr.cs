using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Logic.Manager.PrefabMgr
{
    [ManagerDefine(9, true)]
    public sealed class PrefabMgr : Manager<PrefabMgr>, IManager
    {
        private Dictionary<string, GameObject> m_prefabs;

        public PrefabMgr()
        {
            m_prefabs = new Dictionary<string, GameObject>();
        }
        
        public List<string> GetMarkedPath<T>() where T : PrefabPathAttribute
        {
            var strs = new List<string>();
            var fields = typeof(PrefabPathDefine).GetFields();
            foreach (var fieldInfo in fields)
            {
				
                if (fieldInfo.GetCustomAttributes(typeof(T), false).Any(a => a is T))
                {
                    strs.Add(fieldInfo.GetValue(null) as string);
                }
            }

            return strs;
        }

        public GameObject GetPrefab(string key)
        {
            if (m_prefabs.TryGetValue(key, out var outValue))
            {
                return outValue;
            }

            return null;
        }

        public IEnumerator PreInit()
        {
            var paths = GetMarkedPath<PrefabPathAttribute>();
            foreach (var path in paths)
            {
                var prefab = Resources.Load<GameObject>(PrefabPathDefine.PREFAB_PATH_BASE + path);
                if (prefab)
                {
                    m_prefabs.Add(path, prefab);
                }
                else
                {
                    Debug.LogError($"未找到prefab： {path} 。");
                }
            }
            yield return null;
        }
    }
}