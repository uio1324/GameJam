using System;
using System.Collections;
using Logic.Manager.PrefabMgr;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon.Pool
{
    public class MapDecorationPool : GameObjectPool
    {
        public override IEnumerator PreInit()
        {
            m_prefab = PrefabMgr.Instance.GetPrefab(PrefabPathDefine.PREFAB_PATH_MAP_DECORATION);
            gameObject.layer = LayerMask.NameToLayer("Decoration");
            Serializable = true;
            yield return base.PreInit();
        }
    }
}