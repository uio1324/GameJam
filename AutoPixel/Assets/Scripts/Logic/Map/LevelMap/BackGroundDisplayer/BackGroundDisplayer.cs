using System;
using System.Collections.Generic;
using Logic.Manager.SpriteManager;
using ScriptableObjects.DataTable;
using UnityEngine;

namespace Logic.Map.LevelMap.BackGroundDisplayer
{
    public class BackGroundDisplayer : MonoBehaviour
    {
        public GameObject Template;
        private LevelData m_levelData;
        private List<Sprite> m_subBackgrounds;
        private Transform m_needSmaller;
        private void Start()
        {
            m_levelData = MapLogic.m_instance.levelData;
            m_subBackgrounds = new List<Sprite>();

            var bgOffsetStrings = m_levelData.BGOffset.Split(',');
            var bgOffset = new Vector3(float.Parse(bgOffsetStrings[0]) * 0.001f, float.Parse(bgOffsetStrings[1]) * 0.001f, 0);
            transform.localPosition += bgOffset;
            var curHeight = 0f;
            for (int i = 0; ; i++)
            {
                var sprite = SpriteManager.Instance.GetSprite("background", m_levelData.BackgroundName + "/" + i);
                if (sprite)
                {
                    m_subBackgrounds.Add(sprite);
                    var spriteRenderer = Instantiate(Template).GetComponent<SpriteRenderer>();
                    spriteRenderer.transform.SetParent(transform);
                    spriteRenderer.sprite = sprite;
                    spriteRenderer.gameObject.SetActive(true);
                    var halfHeight = sprite.rect.height * 0.01f * 0.5f;
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
            if (m_needSmaller)
            {
                
            }
        }
    }
}