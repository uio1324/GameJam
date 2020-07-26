using System;

namespace Logic.Map.LevelMap.MapItemCommon
{
    public class MapItemPrefabAttribute : Attribute
    {
        public string m_prefabName;

        public MapItemPrefabAttribute(string prefabName)
        {
            m_prefabName = prefabName;
        }
    }
}