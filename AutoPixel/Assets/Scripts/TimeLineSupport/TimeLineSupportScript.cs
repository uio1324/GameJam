using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Logic.Map.LevelMap.MapDescriber;
using Logic.Map.LevelMap.MapItem.MapItem;
using Logic.Map.LevelMap.MapItemCommon;
using Logic.Map.LevelMap.MapItemCommon.Component;
using Logic.Manager.TimelineMangager;
using ScriptableObjects.DataTable;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace TimeLineSupport
{
    public class TimeLineSupportScript : MonoBehaviour
    {
        public Transform Root;
        public string MapFileName;
        public MapItemDataTable mapItemDataTable;
        public Dictionary<string, GameObject> prefabPool;
        private string mapFileContent;
        private MapJson m_mapJson;
        private bool m_hasDeserialized;
        public static Dictionary<int, MapItemComponent> m_mapItemComponents;
        
        public void Deserialize()
        {
            LoadAsset();
            m_mapJson = JsonUtility.FromJson<MapJson>(mapFileContent);

            foreach (var combiner in m_mapJson.Combiners)
            {
                var id = combiner.Id;
                var name = GetNameFromDataTable(id);

                // Add an animator component to combiner so timeline edit works.
                GameObject newGameObject = Instantiate(prefabPool[name]);
                newGameObject.AddComponent<Animator>();

                MapItemComponent com = newGameObject.GetComponent<MapItemComponent>();
                com.transform.parent = transform;
                com.HostedItem = MapItemPool.InstantiateMapItemBase(name);
                com.HostedItem.HashCode = combiner.HashCode;
                com.transform.position = combiner.Pos;

                foreach (var subItem in combiner.MapItems)
                {
                    var subId = subItem.Id;
                    var subName = GetNameFromDataTable(subId);
                    var subCom = Instantiate(prefabPool[subName]).GetComponent<MapItemComponent>();
                    subCom.HostedItem = MapItemPool.InstantiateMapItemBase(subName);
                    subCom.HostedItem.HashCode = subItem.HashCode;
                    subCom.transform.position = subItem.Pos;
                    subCom.transform.SetParent(com.transform, true);
                }
            }
            ConstructMapping();
        }

        public void ImportTimeline()
        {
            PlayableDirector playableDirectorComp = Root.GetComponent<PlayableDirector>();

            if (TimelineManager.Instance.LoadTimeline(playableDirectorComp, playableDirectorComp.playableAsset.name))
            {
                Debug.Log("Timeline imported.");
            }
        }

        public void ExportTimeline()
        {
            PlayableDirector playableDirectorComp = Root.GetComponent<PlayableDirector>();

            if (TimelineManager.Instance.SaveTimeline(playableDirectorComp, playableDirectorComp.playableAsset.name))
            {
                Debug.Log("Timeline exported.");
            }
        }

        private void LoadAsset()
        {
            if (prefabPool == null)
            {
                prefabPool = new Dictionary<string, GameObject>();
            }
            else
            {
                prefabPool.Clear();
            }
            mapItemDataTable = Resources.Load<MapItemDataTable>("DataTables/MapItemDataTable");
            var prefabs = Resources.LoadAll<GameObject>("Prefabs/LevelItem");
            foreach (var prefab in prefabs)
            {
                prefabPool.Add(prefab.name.Replace("Prefab", ""), prefab);
            }
            var content = Resources.Load<TextAsset>("Map/" + MapFileName);
            if (content == null)
            {
                Debug.Log("检查文件名是否有误，未找到文件");
            }
            else
            {
                mapFileContent = content.text;
            }
        }

        private string GetNameFromDataTable(int id)
        {
            foreach (var mapItemData in this.mapItemDataTable.Datas)
            {
                if (mapItemData.Id == id)
                {
                    return mapItemData.Name;
                }
            }
            return "";
        }

        private void ConstructMapping()
        {
            var root = GameObject.Find("Root");
            var components = root.GetComponentsInChildren<MapItemCombinerComponent>();
            if (m_mapItemComponents == null)
            {
                m_mapItemComponents = new Dictionary<int, MapItemComponent>();
            }
            else
            {
                m_mapItemComponents.Clear();
            }
            
            foreach (var mapItemComponent in components)
            {
                m_mapItemComponents.Add(mapItemComponent.HostedItem.HashCode, mapItemComponent);
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnSceneRun()
        {
			if (SceneManager.GetActiveScene().name != "TimelineScene")
			{
				return;
            }

            var root = GameObject.Find("Root");
            var components = root.GetComponentsInChildren<MapItemCombinerComponent>();
            if (m_mapItemComponents == null)
            {
                m_mapItemComponents = new Dictionary<int, MapItemComponent>();
            }
            else
            {
                m_mapItemComponents.Clear();
            }
            
            foreach (var mapItemComponent in components)
            {
                m_mapItemComponents.Add(mapItemComponent.HostedItem.HashCode, mapItemComponent);
            }
        }
    }
}