using System;
using System.Collections;
using Logic.Manager.PrefabMgr;
using Logic.Map.LevelMap.MapItemCommon.Component;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon.Pool
{
    public class MapLadderPool : GameObjectPool
    {
        public override IEnumerator PreInit()
        {
            m_prefab = PrefabMgr.Instance.GetPrefab(PrefabPathDefine.PREFAB_PATH_MAP_LADDER);
            gameObject.layer = LayerMask.NameToLayer("Ladder");
            Serializable = false;
            yield return base.PreInit();
        }
    }
}