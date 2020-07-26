using System;
using Logic.Map.LevelMap.MapItemCommon.Pool;
using ScriptableObjects.DataTable;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItem.MapItem
{
    public class MapDecoration : MapItemBase
    {
        private LevelData m_levelData;
        private float disappearHeight = 10000f;
        public override void OnAppear()
        {
            m_levelData = MapLogic.m_instance.levelData;
            m_owner = MapLogic.m_instance.GetBehaviorObject<MapDecorationPool>();
            base.OnAppear();
            var sprite = m_owner.SpriteRenderer.sprite;
            if (sprite)
            {
                disappearHeight = sprite.rect.height;
            }
        }

        public override void OnDestroy()
        {
            OnDisappear();
        }

        public override void Update()
        {
            if (!Util.Util.IsEditorScene())
            {
                var cameraHeight = MapLogic.m_instance.GetCameraPos().y;
                var curSpeed = Mathf.Lerp(m_levelData.BeginSpeed, 0, cameraHeight / m_levelData.EndHeight) * 0.001f;
                var tran = m_owner.transform;
                tran.position += Time.deltaTime * curSpeed * Vector3.down;
                var bounds = MapLogic.m_instance.GetUpperAndLower();
                if (tran.position.y + disappearHeight < bounds.x)
                {
                    OnDisappear();
                }
            }
        }
    }
}