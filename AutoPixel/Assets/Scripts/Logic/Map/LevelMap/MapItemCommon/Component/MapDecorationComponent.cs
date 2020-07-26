using System;
using Logic.Map.LevelMap.MapItem.MapItem;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon.Component
{
    public class MapDecorationComponent : MapItemComponent
    {
        private void OnDestroy()
        {
            HostedItem?.OnDestroy();
        }

        private void Update()
        {
            HostedItem?.Update();
        }
    }
}