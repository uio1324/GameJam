using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Logic.Common.Singleton;
using Logic.Core;
using Logic.Manager.AudioMgr;
using Logic.Manager.PrefabMgr;
using Logic.Map.LevelMap;
using Logic.Manager.ProgressManager;
using Logic.Manager.TimelineMangager;
using Logic.Map.LevelMap.MapItem.MapItem;
using Logic.Map.LevelMap.MapItemCommon.Component;
using Logic.Temp;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Logic.Manager.EventMgr
{
    public class MapLogicSharedDelegate : Singleton<MapLogicSharedDelegate>
    {
        private delegate void MapLogicDelegate(MapLogicItem logicItem, Collider2D other);

        private Dictionary<int, MapLogicDelegate> m_sharedDelegates;
        private static readonly int Transition = Shader.PropertyToID("_Transition");
        private static readonly int NotSaveTex = Shader.PropertyToID("_NotSaveTex");
        private static readonly int SavedTex = Shader.PropertyToID("_SavedTex");

        public void Invoke(int eventType, MapLogicItem mapLogicItem, Collider2D collider2D)
        {
            if (m_sharedDelegates.TryGetValue(eventType, out var logicDelegate))
            {
                logicDelegate.Invoke(mapLogicItem, collider2D);
            }
        }
        public override void OnAwake()
        {
            m_sharedDelegates = new Dictionary<int, MapLogicDelegate>();

            var type = typeof(MapLogicSharedDelegate);
            var methodInfos = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var methodInfo in methodInfos)
            {
                var attributes = methodInfo.GetCustomAttributes(typeof(EventTypeAttribute), false);
                foreach (var attribute in attributes)
                {
                    var eventTypeAttribute = (EventTypeAttribute) attribute;
                    var method = (MapLogicDelegate)Delegate.CreateDelegate(typeof(MapLogicDelegate),this, methodInfo);
                    m_sharedDelegates.Add(eventTypeAttribute.m_eventType, method);
                }
            }
        }

        [EventType(EventType.SavePoint)]
        private void OnSavePoint(MapLogicItem logicItem, Collider2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                return;
            }

            if (ProgressMgr.Instance.IsArrived(logicItem.HashCode))
            {
                return;
            }

            ProgressMgr.Instance.SaveProgress(logicItem.HashCode);
            
            var spriteRenderer = logicItem.m_owner.SpriteRenderer;
            var mate = spriteRenderer.material;
            
            PlayerManager.PlayerManager.Instance.GetPlayerScript().AddLight(50);
            AudioMgr.AudioMgr.Instance.Play(AudioDefine.FillLight);
            
            GameRoot.m_instance.StartCoroutine(TransitionSavePoint(mate));
        }

        private IEnumerator TransitionSavePoint(Material material)
        {
            float time = 0;
            while (time < 1)
            {
                time += Time.deltaTime;
                material.SetFloat(Transition, time);
                yield return null;
            }
        }

        [EventType(EventType.BreakStone)]
        private void OnStoneBreak(MapLogicItem mapLogicItem, Collider2D other)
        {
            if (other.attachedRigidbody)
            {
                var gameObject = other.attachedRigidbody.gameObject;
                if (gameObject.layer == LayerMask.NameToLayer("Combiner"))
                {
                    var mapItemCombiner = gameObject.GetComponent<MapItemCombinerComponent>().HostedItem;
                    if (mapItemCombiner.m_canBeBreak)
                    {
                        var prefab =
                            PrefabMgr.PrefabMgr.Instance.GetPrefab(PrefabPathDefine.PREFAB_PATH_EFFECT_EXPLOSION);
                        var explosion = Object.Instantiate(prefab);
                        explosion.transform.position = mapItemCombiner.m_owner.transform.position + Vector3.up;
                        Object.Destroy(explosion, 0.5f);
                        AudioMgr.AudioMgr.Instance.Play(AudioDefine.StoneBreak);

                        mapItemCombiner?.OnDisappear();
                    }
                    else
                    {
                        return;
                    }
                }
                else if (gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    PlayerManager.PlayerManager.Instance.m_player.GetComponent<TempPlayerController>().OnDeathTriggerEnter(other);
                }
                else
                {
                    return;
                }

                
                if (mapLogicItem.m_beCombined)
                {
                    var parent = mapLogicItem.m_owner.transform.parent.GetComponent<MapItemCombinerComponent>();
                    if (parent)
                    {
                        var selfParent = (MapItemCombiner)parent.HostedItem;
                        selfParent?.HideAllSameLogicItem(mapLogicItem.EventId);
                    }
                }
                else
                {
                    mapLogicItem.OnDisappear();
                }
            }
        }

        [EventType(EventType.TimelineTrigger)]
        private void OnLogicTrigger(MapLogicItem logicItem, Collider2D other)
        {
            PlayableDirector playableDirector = MapLogic.m_instance.GetComponent<PlayableDirector>();

            if (other.gameObject == PlayerManager.PlayerManager.Instance.m_player.gameObject)
            {
                if (string.IsNullOrEmpty(logicItem.TimeLineName) == false)
                {
                    TimelineManager.Instance.LoadAndPlayTimeline(playableDirector, logicItem.TimeLineName);
                }
            }
        }

        [EventType(EventType.FillLight)]
        private void OnFillLight(MapLogicItem logicItem, Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                EventMgr.Instance.Dispatch(EventType.FillLight);
                AudioMgr.AudioMgr.Instance.Play(AudioDefine.FillLight);
                if (logicItem.m_beCombined)
                {
                    var parent = logicItem.m_owner.transform.parent.GetComponent<MapItemCombinerComponent>();
                    if (parent)
                    {
                        var selfParent = (MapItemCombiner)parent.HostedItem;
                        selfParent?.HideSingleLogicItem(logicItem.EventId, logicItem.HashCode);
                    }
                }
                else
                {
                    logicItem.OnDisappear();
                }
            }
        }
    }
}