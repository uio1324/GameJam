using System;
using Logic.Map.LevelMap.MapItem.MapItem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Logic.Manager.DataTableMgr;
using Logic.Manager.PrefabMgr;
using Logic.Map.LevelMap;
using Logic.Map.LevelMap.MapDescriber;
using Logic.Map.LevelMap.MapItemCommon;
#if UNITY_EDITOR
using Logic.Manager.SpriteManager;
using Logic.Map.LevelMap.MapItemCommon.Component;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace MapEditor
{
    public class MapEditor : MonoBehaviour
    {
        public static MapEditor m_instance;
        public Button LoadButton;
        public Button SaveButton;
        public Button DeleteButton;
        public Button InstantiateButton;
        public Button CombineButton;
        public Button DecombineButton;
        public Dropdown TypeList;
        public Material Material;
        public float DebugLineLength;

        private int m_curType;
        private bool m_isMultiSelect;
        private bool m_isPlacing;
        private Camera m_camera;

        public List<GameObject> CurSelected;
#if UNITY_EDITOR

        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
            
            CurSelected = new List<GameObject>();
            m_camera = gameObject.GetComponent<Camera>();
            m_curType = DataTableMgr.DataTableMgrDefine.MAP_ITEM_ID_BASE;
            m_isPlacing = false;
            
            MgrPreInit();
        }

        private void MgrPreInit()
        {
            StartCoroutine(PreInit());
        }

        private IEnumerator PreInit()
        {
            yield return DataTableMgr.Instance.PreInit();
            yield return SpriteManager.Instance.PreInit();
            yield return PrefabMgr.Instance.PreInit();
            yield return MapLogic.m_instance.PreInit();
            InitUi();
        }

        private void Update()
        {
            m_isMultiSelect = Input.GetKey(KeyCode.LeftControl);
            
            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = Input.mousePosition;

                if (m_isPlacing) // 处于放置阶段
                {
                    PlaceMapItem(mousePos);
                }
                else // 点选
                {
                    var cameraRay = m_camera.ScreenPointToRay(mousePos);
                    var hit2d = Physics2D.Raycast(new Vector2(cameraRay.origin.x, cameraRay.origin.y),
                        new Vector2(cameraRay.direction.x, cameraRay.direction.y), float.PositiveInfinity,
                        LayerMask.GetMask("Stone") | LayerMask.GetMask("Decoration") |
                        LayerMask.GetMask("Ladder") | LayerMask.GetMask("Trigger"));
                    if (hit2d.collider)
                    {
                        OnClickGameObject(hit2d.collider);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (m_isPlacing)
                {
                    ExitPlacing();
                }
                else
                {
                    ExitSelecting();
                }
                
            }
        }

        private void PlaceMapItem(Vector3 pos)
        {
            pos = m_camera.ScreenToWorldPoint(pos);
            pos.z = 0;
            MapLogic.m_instance.InstantiateMapStone(m_curType, pos);
        }

        private void OnClickGameObject(Collider2D targetCollider)
        {
            var o = targetCollider.gameObject;
            if (o.transform.parent.gameObject.layer == LayerMask.NameToLayer("Combiner"))
            {
                o = o.transform.parent.gameObject;
            }
                    
            if (CurSelected.Contains(o))
            {
                CurSelected.Remove(o);
            }
            else
            {
                if (m_isMultiSelect)
                {
                    CurSelected.Add(o);
                    Selection.activeGameObject = null;
                }
                else
                {
                    CurSelected.Clear();
                    CurSelected.Add(o);
                    Selection.activeGameObject = o;
                }
            }
        }

        private void ExitSelecting()
        {
            CurSelected.Clear();
            Selection.activeGameObject = null;
        }

        private void ExitPlacing()
        {
            m_isPlacing = false;
        }

        private void OnGUI()
        {
            if (m_isPlacing)
            {
                // 这里其实不用每次都拿到所有字符串，不过我懒得写了，Windows编辑器下应该不缺这点性能
                GUI.Label(new Rect(0, 0, 220, 20),
                    "正在放置 ： " + MapItemPool.Instance.GetMapItemNames()[
                        m_curType - DataTableMgr.DataTableMgrDefine.MAP_ITEM_ID_BASE].DisplayName);
            }
            else
            {
                for (var i = 0; i < CurSelected.Count; i++)
                {
                    GUI.Label(new Rect(0, 20 * i, 220, 20), CurSelected[i].name);
                }
            }
            
        }

        private void InitUi()
        {
            LoadButton.onClick.AddListener(OnLoadButtonClick);
            SaveButton.onClick.AddListener(OnSaveButtonClick);
            TypeList.onValueChanged.AddListener(OnDropDownValueChange);
            InstantiateButton.onClick.AddListener(OnInstantiateButtonClick);
            DeleteButton.onClick.AddListener(OnDeleteButtonClick);
            CombineButton.onClick.AddListener(OnCombineButtonClick);
            DecombineButton.onClick.AddListener(OnDecombineButtonClick);

            var dataList = new List<Dropdown.OptionData>();
            var list = MapItemPool.Instance.GetMapItemNames();
            foreach (var itemData in list)
            {
                dataList.Insert(itemData.Id - DataTableMgr.DataTableMgrDefine.MAP_ITEM_ID_BASE,
                        new Dropdown.OptionData {text = itemData.DisplayName});
            }

            TypeList.options = dataList;
        }

        /// <summary>
        /// 检查list中的物体的layer，如果list中存在一个layer不与layers中任意一个layer相等的物体，返回false
        /// </summary>
        /// <param name="list"></param>
        /// <param name="layers"></param>
        /// <returns></returns>
        private static bool CheckLayer(List<GameObject> list, int[] layers)
        {
            foreach (var o in list)
            {
                bool badLayer = false;
                for (int i = 0; i < layers.Length; i++)
                {
                    if (o.layer == layers[i])
                    {
                        badLayer = true;
                    }
                }

                if (!badLayer)
                {
                    return false;
                }
            }

            return true;
        }

        
        private void OnLoadButtonClick()
        {
            var path = EditorUtility.OpenFilePanel("选择文件", Application.dataPath + "/Resources/Map", "json");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            MapLogic.m_instance.ClearGameObject();
            var stream = File.OpenRead(path);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            MapDescriber.Instance.Deserialize(Encoding.UTF8.GetString(bytes));
            MapDescriber.Instance.ShowAllItem();
            stream.Dispose();
            stream.Close();
        }

        private void OnSaveButtonClick()
        {
            var path = EditorUtility.SaveFilePanel("保存数据", Application.dataPath + "/Resources/Map", "map_Describer", "json");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            var jsonBytes = Encoding.UTF8.GetBytes(MapDescriber.Instance.Serialize());
            var stream = File.Open(path, FileMode.Create);
            stream.Write(jsonBytes, 0, jsonBytes.Length);
            stream.Dispose();
            stream.Close();
            AssetDatabase.Refresh();
        }

        private void OnDropDownValueChange(int value)
        {
            m_curType = value + DataTableMgr.DataTableMgrDefine.MAP_ITEM_ID_BASE;
        }

        private void OnDeleteButtonClick()
        {
            foreach (var o in CurSelected)
            {
                MapLogic.m_instance.DeleteMapItem(o);
            }
            CurSelected.Clear();
            Selection.activeGameObject = null;
        }

        private void OnInstantiateButtonClick()
        {
            m_isPlacing = true;
        }

        private void OnCombineButtonClick()//这一块后面要重构一下
        {
            if (CurSelected.Count > 0)
            {
                if (!CheckLayer(CurSelected,
                    new[]
                    {
                        LayerMask.NameToLayer("Stone"), LayerMask.NameToLayer("Ladder"),
                        LayerMask.NameToLayer("Trigger"), LayerMask.NameToLayer("Decoration")
                    }))
                {
                    Debug.LogError("所选物体中有Layer错误");
                    return;
                }
                var mapItem = (MapItemCombiner)MapItemPool.Instance.GetMapItem(20000002);//mapItemCombiner
                mapItem.Combine(CurSelected);
                CurSelected.Clear();
            }
        }

        private void OnDecombineButtonClick()
        {
            if (CurSelected.Count > 0 && CurSelected[0].layer == LayerMask.NameToLayer("Combiner"))
            {
                var combinerGo = CurSelected[0];
                var children = combinerGo.GetComponentsInChildren<MapItemComponent>();
                foreach (var mapItemComponent in children)
                {
                    var parent = MapLogic.m_instance.GetGameObjectPool(mapItemComponent.gameObject.layer);
                    if (!parent)
                    {
                        Debug.LogError("未找到layer为" + LayerMask.LayerToName(mapItemComponent.gameObject.layer) + "的池");
                        continue;
                    }
                    
                    mapItemComponent.transform.SetParent(parent.transform);
                }

                var combiner = combinerGo.GetComponent<MapItemCombinerComponent>().HostedItem as MapItemCombiner;
                if (combiner != null)
                {
                    combiner.MapItems.Clear();
                    combiner.OnDisappear();
                }

                CurSelected.Clear();
            }
        }
#endif
    }
}