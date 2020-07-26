using System;
using Logic.Map.LevelMap.MapItemCommon.Pool;

namespace Logic.Map.LevelMap.MapItem.MapItem
{
    [Serializable]
    public class MapLadder : MapItemBase, ICombinable
    {
        public override void Update()
        {
            
        }

        public override void OnAppear()
        {
            m_owner = MapLogic.m_instance.GetBehaviorObject<MapLadderPool>();
            base.OnAppear();
        }

        public override void OnDisappear()
        {
            m_owner.transform.parent = MapLogic.m_instance.GetGameObjectPool<MapLadderPool>().transform;
            base.OnDisappear();
        }

        public override void OnDestroy()
        {
            OnDisappear();
        }

        public void BeCombined()
        {
            m_beCombined = true;
        }
    }
}