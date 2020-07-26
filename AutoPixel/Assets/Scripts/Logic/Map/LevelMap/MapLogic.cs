using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Core;
using Logic.Core.Scenes;
using Logic.Manager.DataTableMgr;
using Logic.Manager.EventMgr;
using Logic.Manager.MapManager;
using Logic.Manager.PlayerManager;
using Logic.Manager.ProgressManager;
using Logic.Manager.SceneMgr;
using Logic.Manager.AudioMgr;
using Logic.Map.LevelMap.CustomItem;
using Logic.Map.LevelMap.MapItem.MapItem;
using Logic.Map.LevelMap.MapItemCommon;
using Logic.Map.LevelMap.MapItemCommon.Component;
using Logic.Map.LevelMap.MapItemCommon.Pool;
using Logic.Temp;
using ScriptableObjects.DataTable;
using UI.CommonUI;
using UI.GameSceneUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.LWRP;
using EventType = Logic.Manager.EventMgr.EventType;

namespace Logic.Map.LevelMap
{
    public class MapLogic : MonoBehaviour
    {
        public static MapLogic m_instance;
        public LevelData levelData;
        public UnityEngine.Experimental.Rendering.Universal.Light2D globalLight;
        public UnityEngine.Experimental.Rendering.Universal.Light2D maskLight;
        
        /// <summary>
        /// 结束包括死亡和过关
        /// </summary>
        public bool isOver = false;

        private Dictionary<Type, GameObjectPool> m_gameObjectPools;//用于做表现的池
        private GameObject m_colliderWithRigid;
        private GameObject m_colliderWithoutRigid;
        private TempScroller m_scroller;
        private string m_height;
        private CustomItemGenerator CustomItemGenerator;
        private IEnumerator InitLayer()
        {
            yield return AddGameObjectPool<MapDecorationPool>();
            yield return AddGameObjectPool<MapItemCombinerObjectPool>();
            yield return AddGameObjectPool<MapLadderPool>();
            yield return AddGameObjectPool<MapStonePool>();
            yield return AddGameObjectPool<MapTriangleStonePool>();
            yield return AddGameObjectPool<MapLogicPool>();
        }
        private IEnumerator AddGameObjectPool<T>() where T : GameObjectPool
        {
            var go = new GameObject();
            go.transform.parent = transform;
            var com = go.AddComponent<T>();
            go.name = com.GetType().Name;
            m_gameObjectPools.Add(com.GetType(), com);
            yield return com.PreInit();
        }
        

        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
            if (!Util.Util.IsEditorScene())
            {
                var camera = Camera.main;
                if (camera)
                {
                    m_scroller = camera.transform.gameObject.AddComponent<TempScroller>();
                    DataTableMgr.Instance.TryGetDataTableById<LevelDataTable, LevelData>(GameRoot.m_instance.m_levelId,
                        out levelData);
                    m_scroller.SetScrollSpeed(levelData.ScrollSpeed);
                }

                CustomItemGenerator = GetComponent<CustomItemGenerator>();
            }
        }

        private void OnGUI()
        {
#if UNITY_EDITOR
            var frame = 1 / Time.deltaTime;
            GUI.Label(new Rect(0, 500, 200, 500), frame.ToString());

            DebugGUI();
#endif
        }

        private void DebugGUI()
        {
            m_height = GUI.TextField(new Rect(500, 0, 200, 50), m_height);
            if (GUI.Button(new Rect(500, 50, 100, 50), "Jump"))
            {
                if (!string.IsNullOrEmpty(m_height))
                {
                    var player = PlayerManager.Instance.m_player;
                    player.position = new Vector3(player.position.x, float.Parse(m_height), 0);
                    m_scroller.transform.position = new Vector3(0, float.Parse(m_height), -10);
#if UNITY_EDITOR
                    EditorApplication.isPaused = true;
#endif
                }
            }
        }

        public IEnumerator PreInit()
        {
            if (!Util.Util.IsEditorScene())
            {
                yield return MapManager.Instance.PreInit();
            }
            yield return MapItemPool.Instance.PreInit();
            yield return InitPool();
        }

        private IEnumerator InitPool()
        {
            m_gameObjectPools = new Dictionary<Type, GameObjectPool>();

            yield return InitLayer();
        }
        
        public void InstantiateMapStone(int itemType, Vector3 pos)
        {
            MapItemPool.Instance.GetMapItem(itemType, pos).OnAppear();
        }
        public void DeleteMapItem(GameObject mapItem)
        {
            var component = mapItem.GetComponent<MapItemComponent>();
            if (component)
            {
                component.HostedItem.OnDisappear();
            }
        }

        public MapItemComponent GetBehaviorObject<T>() where T : GameObjectPool
        {
            return m_gameObjectPools[typeof(T)].Get();
        }
        
        public GameObjectPool GetGameObjectPool<T>() where T : GameObjectPool
        {
            return m_gameObjectPools[typeof(T)];
        }

        public GameObjectPool GetGameObjectPool(int layer)
        {
            foreach (var gameObjectPool in m_gameObjectPools)
            {
                if (gameObjectPool.Value.gameObject.layer == layer)
                {
                    return gameObjectPool.Value;
                }
            }

            return null;
        }

        public List<MapItemBase> GetOtherMapItems()
        {
            var components = new List<MapItemBase>();
            foreach (var gameObjectPool in m_gameObjectPools)
            {
                if (gameObjectPool.Value.Serializable && gameObjectPool.Value.gameObject.layer != LayerMask.NameToLayer("Combiner"))
                {
                    var itemComponents = gameObjectPool.Value.GetComponentsInChildren<MapItemComponent>();
                    foreach (var itemComponent in itemComponents)
                    {
                        if (itemComponent.gameObject.layer == gameObjectPool.Value.gameObject.layer)
                        {
                            components.Add(itemComponent.HostedItem);
                        }
                    }
                }
            }

            return components;
        }

        public List<MapItemCombiner> GetAllCombiners()
        {
            var components = GetGameObjectPool<MapItemCombinerObjectPool>().GetComponentsInChildren<MapItemCombinerComponent>();
            var list = new List<MapItemCombiner>();
            foreach (var component in components)
            {
                if (component.isActiveAndEnabled)
                {
                    list.Add((MapItemCombiner)component.HostedItem);
                }
            }

            return list;
        }

        public Vector2 GetUpperAndLower()
        {
            return new Vector2(m_scroller.Lower, m_scroller.Upper);
        }

        public Vector3 GetCameraPos()
        {
            return m_scroller.transform.position;
        }

        public void SetCameraPos(Vector3 cameraPos)
        {
            m_scroller.transform.position = cameraPos;
        }

        public float GetScrollSpeed()
        {
            return m_scroller.ScrollSpeed;
        }

        public void SetAllowScroll(bool canScroll)
        {
            m_scroller.AllowScroll = canScroll;
        }

        public bool GetAllowScroll()
        {
            return m_scroller.AllowScroll;
        }

        public void ClearGameObject()
        {
            var gameObjectPools = transform.GetComponentsInChildren<GameObjectPool>();
            foreach (var gameObjectPool in gameObjectPools)
            {
                var mapItemComponents = gameObjectPool.transform.GetComponentsInChildren<MapItemComponent>(true);
                foreach (var mapItemComponent in mapItemComponents)
                {
                    mapItemComponent.HostedItem?.OnDisappear();
                }
            }
        }

        public void Reset()
        {
            ClearGameObject();
            MapDescriber.MapDescriber.Instance.Reset();
            CustomItemGenerator.Reset();
            MapManager.Instance.LoadLevel(GameRoot.m_instance.m_levelId);
            m_scroller.Reset();
            isOver = false;
        }

        private void Update()
        {
            if (!Util.Util.IsEditorScene())
            {
                if (m_scroller.AllowScroll)
                {
                    MapDescriber.MapDescriber.Instance.Update(m_scroller.Lower, m_scroller.Upper + m_scroller.GetCameraOrthographicSize());
                    CustomItemGenerator.UpdateHeight(m_scroller.Upper);
                }

                if (levelData.MaxHeight < PlayerManager.Instance.m_player.position.y && !isOver)
                {
                    GameRoot.m_instance.StartCoroutine(OnFinishedLevel());
                }
            }
        }

        private IEnumerator OnFinishedLevel()
        {
            isOver = true;
            var curLevel = GameRoot.m_instance.m_levelId - DataTableMgr.DataTableMgrDefine.LEVEL_ID_BASE;
            var progress = -1;
            if(ProgressMgr.Instance.achievedTeachingLevel)
            {
                progress = PlayerPrefs.GetInt("progress");
            }
            //var maskLightBeginIntensity = maskLight.intensity;
            //var globalLightBeginIntensity = globalLight.intensity;
            EventMgr.Instance.Dispatch(EventType.FinishLevel);
            PlayerPrefs.SetInt("progress", Mathf.Max(progress, curLevel));
            PlayerPrefs.Save();
            ProgressMgr.Instance.achievedTeachingLevel = true;

            var t = 0f;
            var gameScene = (GameSceneMenu)SceneUiBase.Instance;
            var button = gameScene.goOnButton;
            var text = button.transform.Find("Text");
            var image = button.GetComponent<Image>();
            
            button.gameObject.SetActive(true);
            text.gameObject.SetActive(false);
            button.interactable = false;
            

            while (t < 1f)
            {
                image.color = new Color(0, 0, 0, t);

                t += Time.deltaTime / 2;
                AudioMgr.Instance.SetBgVolume(1 - t);

                //maskLight.intensity = Mathf.Lerp(maskLightBeginIntensity, 0, t / 1f);
                //globalLight.intensity = Mathf.Lerp(globalLightBeginIntensity, 0, t / 1f);


                yield return null;
            }
            image.color = new Color(0, 0, 0, 1);
            AudioMgr.Instance.SetBgVolume(0);

            yield return null;
            Handheld.PlayFullScreenMovie("level" + curLevel + ".mp4", Color.black, FullScreenMovieControlMode.Hidden);

            yield return null;

            text.gameObject.SetActive(true);
            button.interactable = true;


            while (button.interactable)
            {
                yield return null;
            }

            if (curLevel < 3)
            {
                curLevel++;
                GameRoot.m_instance.m_levelId = curLevel + DataTableMgr.DataTableMgrDefine.LEVEL_ID_BASE;
                yield return SceneMgr.Instance.SwitchScene(typeof(GameScene));
            }
            else
            {
                yield return SceneMgr.Instance.SwitchScene(typeof(MainScene));
            }
        }

        private void OnDestroy()
        {
            MapDescriber.MapDescriber.Instance.OnDestroy();
        }
    }
}
