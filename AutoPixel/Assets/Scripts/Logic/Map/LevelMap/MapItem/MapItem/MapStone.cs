using System;
using Logic.Manager.SpriteManager;
using Logic.Map.LevelMap.MapItemCommon;
using Logic.Map.LevelMap.MapItemCommon.Component;
using Logic.Map.LevelMap.MapItemCommon.Pool;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using Object = UnityEngine.Object;

namespace Logic.Map.LevelMap.MapItem.MapItem
{
    [Serializable]
    public class MapStone : MapItemBase, ICombinable
    {
        public override void Update()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public override void OnAppear()
        {
            m_owner = MapLogic.m_instance.GetBehaviorObject<MapStonePool>();
            base.OnAppear();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnDisappear()
        {
            m_owner.transform.parent = MapLogic.m_instance.GetGameObjectPool<MapStonePool>().transform;
            m_owner.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().enabled = false;
            base.OnDisappear();
        }

        public override void OnDestroy()
        {
            OnDisappear();//将mapItem还回去
        }

        public void BeCombined()
        {
            m_beCombined = true;
        }
    }
}