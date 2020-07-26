using System.Collections;
using Logic.Manager.PrefabMgr;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon.Pool
{
    public class MapLogicPool : GameObjectPool
    {
        public override IEnumerator PreInit()
        {
            m_prefab = PrefabMgr.Instance.GetPrefab(PrefabPathDefine.PREFAB_PATH_MAP_LOGIC);
            gameObject.layer = LayerMask.NameToLayer("Trigger");
            Serializable = true;
            yield return base.PreInit();
        }
    }
}