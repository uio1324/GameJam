using System.Collections;
using Logic.Manager.PrefabMgr;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon.Pool
{
    public class MapTriangleStonePool : GameObjectPool
    {
        public override IEnumerator PreInit()
        {
            m_prefab = PrefabMgr.Instance.GetPrefab(PrefabPathDefine.PREFAB_PATH_MAP_TRIANGLE_STONE);
            gameObject.layer = LayerMask.NameToLayer("Stone");
            Serializable = false;
            yield return base.PreInit();
        }
    }
}