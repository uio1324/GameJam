using System;
using Logic.Manager.EventMgr;
using Logic.Temp;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon.Component
{
    public class MapLogicComponent : MapItemComponent
    {
        private Collider2D m_contactingCollider2D;
        private void OnDestroy()
        {
            HostedItem?.OnDestroy();
        }

        private void Update()
        {
            HostedItem?.Update();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HostedItem?.OnTriggerIn(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            HostedItem?.OnTriggerOut(other);
        }
    }
}