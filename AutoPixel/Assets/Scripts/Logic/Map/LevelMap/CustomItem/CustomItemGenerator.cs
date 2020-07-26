using System;
using System.Collections.Generic;
using Logic.Core;
using UnityEngine;

namespace Logic.Map.LevelMap.CustomItem
{
    public class CustomItemGenerator : MonoBehaviour
    {
        public List<CustomItem> CustomItems;
        private int m_curIndex;
        private List<GameObject> m_usingObjects;

        private void Start()
        {
            foreach (var customItem in CustomItems)
            {
                customItem.GenerateKey();
            }
            CustomItems.Sort((a, b) =>(a.GetKey() - b.GetKey()));
            m_usingObjects = new List<GameObject>();
            m_curIndex = 0;
        }

        public void Reset()
        {
            m_curIndex = 0;
            foreach (var usingObject in m_usingObjects)
            {
                Destroy(usingObject);
            }
            m_usingObjects.Clear();
        }

        public void UpdateHeight(float upper)
        {
            if (m_curIndex < CustomItems.Count && upper > CustomItems[m_curIndex].Key)
            {
                if (CustomItems[m_curIndex].LevelName != MapLogic.m_instance.levelData.MapFileName)
                {
                    m_curIndex++;
                    return;
                }
                m_usingObjects.Add(CustomItems[m_curIndex].Generate());
                m_curIndex++;
            }
        }
    }
}