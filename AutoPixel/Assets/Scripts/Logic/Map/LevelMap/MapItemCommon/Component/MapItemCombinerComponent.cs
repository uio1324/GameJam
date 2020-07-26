using Logic.Map.LevelMap.MapItem.MapItem;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon.Component
{
    public class MapItemCombinerComponent : MapItemComponent
    {
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
            if (HostedItem != null)
            {
                if (HostedItem.IsInteractive)
                {
                    HostedItem.OnTriggerIn(other);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (HostedItem != null)
            {
                if (HostedItem.IsInteractive)
                {
                    HostedItem.OnTriggerOut(other);
                }
            }
        }
    }
}