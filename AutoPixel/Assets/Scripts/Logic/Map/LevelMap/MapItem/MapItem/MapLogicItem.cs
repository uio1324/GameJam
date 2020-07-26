using System;
using Logic.Manager.EventMgr;
using Logic.Manager.ProgressManager;
using Logic.Manager.SpriteManager;
using Logic.Manager.PlayerManager;
using Logic.Map.LevelMap.MapItemCommon.Component;
using Logic.Map.LevelMap.MapItemCommon.Pool;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using EventType = Logic.Manager.EventMgr.EventType;
using Object = UnityEngine.Object;

namespace Logic.Map.LevelMap.MapItem.MapItem
{
    [Serializable]
    public class MapLogicItem : MapItemBase, ICombinable
    {
        private static readonly int Transition = Shader.PropertyToID("_Transition");
        private static readonly int NotSaveTex = Shader.PropertyToID("_NotSaveTex");
        private static readonly int SavedTex = Shader.PropertyToID("_SavedTex");
        private static readonly float m_distanceThreshold = 2f;
        public override void OnTriggerIn(Collider2D collider2D)
        {
            if (EventId != 0)
            {
                MapLogicSharedDelegate.Instance.Invoke(EventId, this, collider2D);
            }
        }

        public override void Update()
        {
            if(m_owner.SpriteRenderer)
            {
                if(m_spriteName.Contains("Sentence"))
                {
                    //var cameraPos = MapLogic.m_instance.GetCameraPos();
                    var playerPos = PlayerManager.Instance.m_player.position.y;
                    //var distance = Vector3.Distance(m_owner.transform.position, new Vector3(cameraPos.x, cameraPos.y, 0));
                    var distance = Math.Abs(m_owner.transform.position.y - playerPos);

                    m_owner.SpriteRenderer.color = new Color(1, 1, 1, 1 - Mathf.Clamp01(distance / m_distanceThreshold));
                }
                else
                {
                    m_owner.SpriteRenderer.color = Color.white;
                }
            }
        }

        public override void OnAppear()
        {
            m_owner = MapLogic.m_instance.GetBehaviorObject<MapLogicPool>();
            if (EventId == EventType.FillLight)
            {
                m_owner.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().enabled = true;
            }
            base.OnAppear();
            
            var litMate = Resources.Load<Material>("Material/Sprite-Lit-Default");
            m_owner.SpriteRenderer.material = litMate;

            if (EventId == EventType.SavePoint)
            {
                var spriteRenderer = m_owner.SpriteRenderer;
                var name = spriteRenderer.sprite.name.Replace("(Clone)", "");
                var notSaveTex = Resources.Load<Texture>("Textures/StaticTexture/" + name);
                spriteRenderer.sprite = Resources.Load<Sprite>("Textures/StaticTexture/" + name);
                var savedTex = Resources.Load<Texture>("Textures/StaticTexture/" + name.Replace("NotSave", "Saved"));
                var mate = Object.Instantiate(Resources.Load<Material>("Material/SavePoint"));
                mate.SetTexture(NotSaveTex, notSaveTex);
                mate.SetTexture(SavedTex, savedTex);

                if (!Util.Util.IsEditorScene())
                {
                    if (ProgressMgr.Instance.IsArrived(HashCode))
                    {
                        mate.SetFloat(Transition, 1f);
                    }
                    else
                    {
                        mate.SetFloat(Transition, 0f);
                    }
                }
                
                spriteRenderer.material = mate;
            }
        }

        public override void OnDisappear()
        {
            if (m_owner)
            {
                m_owner.transform.parent = MapLogic.m_instance.GetGameObjectPool<MapLogicPool>().transform;
                if (EventId == EventType.FillLight)
                {
                    m_owner.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().enabled = false;
                }
                base.OnDisappear();
            }
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