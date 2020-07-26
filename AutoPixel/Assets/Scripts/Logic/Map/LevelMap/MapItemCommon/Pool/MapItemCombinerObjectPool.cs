using System;
using System.Collections;
using Logic.Manager.PrefabMgr;
using Logic.Map.LevelMap.MapItemCommon.Component;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon.Pool
{
    /// <summary>
    /// MapCombiner不需要预制体
    /// </summary>
    public class MapItemCombinerObjectPool : GameObjectPool
    {
        public override IEnumerator PreInit()
        {
            m_prefab = PrefabMgr.Instance.GetPrefab(PrefabPathDefine.PREFAB_PATH_MAP_ITEM_COMBINER);
            gameObject.layer = LayerMask.NameToLayer("Combiner");
            Serializable = true;
            yield return base.PreInit();
        }
    }
}