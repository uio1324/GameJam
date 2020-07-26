using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic.Common.Singleton;
using Logic.Map.LevelMap.MapItem;
using Logic.Map.LevelMap.MapItem.MapItem;
using Logic.Map.LevelMap.MapItemCommon;
using Logic.Map.LevelMap.MapItemCommon.Component;
using UnityEngine;

namespace Logic.Map.LevelMap.MapDescriber
{
    [Serializable]
    public class MapJson
    {
        public MapItemCombiner[] Combiners;
        public MapItemBase[] TrivialMapItem;
    }
    /// <summary>
    /// 主要是用来做序列化和反序列化，是一个工具类
    /// </summary>
    public class MapDescriber : Singleton<MapDescriber>
    {
        private int m_combinerIndex;
        private int m_decorationIndex;
        private MapJson m_mapJson;
        private int m_curLevelId;

        public MapDescriber()
        {
            m_combinerIndex = 0;
            m_decorationIndex = 0;
        }

        public void Reset()
        {
            m_combinerIndex = 0;
            m_decorationIndex = 0;
        }
        /// <summary>
        /// 将描述者的物体序列化为json
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            var combiners = new List<MapItemCombiner>();
            var allStones = MapLogic.m_instance.GetAllCombiners();
            foreach (var stoneBase in allStones)
            {
                stoneBase.UpdateDatas();
                combiners.Add(stoneBase);
            }
            combiners.Sort((a, b) => (int) (a.Pos.y - b.Pos.y));
            
            var trivialMapItems = new List<MapItemBase>();
            var otherMapItems = MapLogic.m_instance.GetOtherMapItems();
            foreach (var mapItemBase in otherMapItems)
            {
                mapItemBase.UpdateDatas();
                trivialMapItems.Add(mapItemBase);
            }
            trivialMapItems.Sort((a, b) => (int) (a.Pos.y - b.Pos.y));
            var mapJson = new MapJson {Combiners = combiners.ToArray(), TrivialMapItem = trivialMapItems.ToArray()};
            return JsonUtility.ToJson(mapJson);
        }

        public void Deserialize(string jsonStr)
        {
            m_mapJson = JsonUtility.FromJson<MapJson>(jsonStr);
        }

        public void ShowAllItem()
        {
            if (m_mapJson.Combiners != null)
            {
                for (int i = 0; i < m_mapJson.Combiners.Length; i++)
                {
                    m_mapJson.Combiners[i].OnAppear();
                }
            }

            if (m_mapJson.TrivialMapItem != null)
            {
                for (int i = 0; i < m_mapJson.TrivialMapItem.Length; i++)
                {
                    var oldItem = m_mapJson.TrivialMapItem[i];
                    if (oldItem.GetType().Name == "MapItemBase")
                    {
                        var newItem = MapItemBase.ReGenerate(oldItem);
                        newItem.OnAppear();
                        m_mapJson.TrivialMapItem[i] = newItem;
                    }
                    else
                    {
                        oldItem.OnAppear();
                    }
                }
            }
        }

        /// <summary>
        /// 更新地图描述者，有需要显示的东西让他显示出来
        /// </summary>
        /// <param name="screenDisappearLower"></param>
        /// <param name="screenAppearUpper"></param>
        public void Update(float screenDisappearLower, float screenAppearUpper)
        {
            if (m_mapJson != null)
            {
                if (m_mapJson.Combiners != null)
                {
                    for (; m_combinerIndex < m_mapJson.Combiners.Length; )
                    {
                        if (m_mapJson.Combiners[m_combinerIndex].Pos.y < screenAppearUpper)
                        {
                            if (m_mapJson.Combiners[m_combinerIndex].Pos.y < screenDisappearLower)
                            {
                                m_combinerIndex++;
                                continue;
                            }
                            m_mapJson.Combiners[m_combinerIndex].OnAppear();
                            m_combinerIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (m_mapJson.TrivialMapItem != null)
                {
                    for (; m_decorationIndex < m_mapJson.TrivialMapItem.Length; )
                    {
                        if (m_mapJson.TrivialMapItem[m_decorationIndex].Pos.y < screenAppearUpper)
                        {
                            if (m_mapJson.TrivialMapItem[m_decorationIndex].Pos.y < screenDisappearLower)
                            {
                                m_decorationIndex++;
                                continue;
                            }
                            var oldItem = m_mapJson.TrivialMapItem[m_decorationIndex];
                            if (oldItem.GetType().Name == "MapItemBase")
                            {
                                var newItem = MapItemBase.ReGenerate(oldItem);
                                newItem.OnAppear();
                                m_mapJson.TrivialMapItem[m_decorationIndex] = newItem;
                            }
                            else
                            {
                                oldItem.OnAppear();
                            }
                            m_decorationIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            m_combinerIndex = 0;
            m_decorationIndex = 0;
        }
    }
}