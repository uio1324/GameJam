using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon.Component
{
    public class MapTriangleStoneComponent : MapItemComponent
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
            if (HostedItem.IsInteractive)
            {
                HostedItem.OnTriggerIn(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (HostedItem.IsInteractive)
            {
                HostedItem.OnTriggerOut(other);
            }
        }
    }
}