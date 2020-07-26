using System;
using System.Collections.Generic;
using Logic.Manager.SpriteManager;
using ScriptableObjects.DataTable;
using UnityEngine;

namespace Logic.Map.LevelMap.BackGroundDisplayer
{
    public class MiddleGroundDisplayer : MonoBehaviour
    {
        public GameObject Template;
        public string side;
        private LevelData m_levelData;
        private List<Sprite> m_subMiddleground;
        private Transform m_needSmaller;

        private void Start()
        {
            m_levelData = MapLogic.m_instance.levelData;
            m_subMiddleground = new List<Sprite>();
            
            var mgScaleStrings = m_levelData.MGScale.Split(',');
            var mgScale = new Vector3(float.Parse(mgScaleStrings[0]) * 0.001f, float.Parse(mgScaleStrings[1]) * 0.001f, 0);
            var mgOffsetStrings = m_levelData.MGOffset.Split(',');
            Vector3 mgOffset;
            if (side == "left")
            {
                mgOffset = new Vector3(float.Parse(mgOffsetStrings[0]) * 0.001f, float.Parse(mgOffsetStrings[1]) * 0.001f, 0);
            }
            else
            {
                mgOffset = new Vector3(float.Parse(mgOffsetStrings[2]) * 0.001f, float.Parse(mgOffsetStrings[3]) * 0.001f, 0);
            }
            
            transform.localPosition += mgOffset;

            var curHeight = 0f;
            for (int i = 0; ; i++)
            {
                var sprite = SpriteManager.Instance.GetSprite("middleground", m_levelData.MiddlegroundName + "/" + i + side);
                if (sprite)
                {
                    
                    m_subMiddleground.Add(sprite);
                    var spriteRenderer = Instantiate(Template).GetComponent<SpriteRenderer>();
                    spriteRenderer.transform.SetParent(transform);
                    spriteRenderer.transform.localScale = mgScale;
                    spriteRenderer.sprite = sprite;
                    spriteRenderer.gameObject.SetActive(true);
                    var halfHeight = sprite.rect.height * 0.01f * 0.5f * mgScale.y;
                    curHeight += halfHeight;
                    spriteRenderer.transform.localPosition = new Vector3(0, curHeight, 0);
                    curHeight += halfHeight;
                }
                else
                {
                    break;
                }
            }
        }

        private void Update()
        {
            if (MapLogic.m_instance.GetAllowScroll())
            {
                var cameraHeight = MapLogic.m_instance.GetCameraPos().y;
                var curSpeed = Mathf.Lerp(m_levelData.BeginSpeed, 0, cameraHeight / m_levelData.EndHeight) * 0.001f;
                transform.position += Time.deltaTime * curSpeed * Vector3.down;
            }
        }
    }
}