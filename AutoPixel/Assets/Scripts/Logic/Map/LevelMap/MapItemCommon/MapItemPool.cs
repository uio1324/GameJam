using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Logic.Common.Singleton;
using Logic.Manager.DataTableMgr;
using Logic.Map.LevelMap.MapItem.MapItem;
using ScriptableObjects.DataTable;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon
{
    /// <summary>
    /// 由于实例化MapItem需要用到反射，为了减少运行时消耗这里进行池化，复用已经生成的MapItem
    /// </summary>
    public class MapItemPool : Singleton<MapItemPool>
    {
        private Dictionary<int, Stack<MapItemBase>> m_pools;
        private Dictionary<int, MapItemData> m_mapItemDatas;
        private int m_preInitNums = 6;
        private Assembly m_assembly;
        private string m_fullName;
        
        private void ResetItem(MapItemBase mapItemBase, MapItemData data)
        {
            mapItemBase.Id = data.Id;
            mapItemBase.m_name = data.Name;
            mapItemBase.m_atlasName = data.AtlasName;
            mapItemBase.m_spriteName = data.SpriteName;
            mapItemBase.EventId = data.EventId;
            mapItemBase.m_size = data.Size;
            mapItemBase.m_canBeBreak = data.CanBeBreak == 1;
            mapItemBase.m_horizontalSpeed = data.HorizontalSpeed;
            mapItemBase.m_verticalSpeed = data.VerticalSpeed;
            mapItemBase.IsInteractive = data.IsInteractive == 1;
            mapItemBase.m_canMoveUpward = data.CanMoveUpward == 1;
        }
        
        public MapItemBase GetMapItem(int mapItemId)
        {
            if (!DataTableMgr.Instance.TryGetDataTableById<MapItemDataTable, MapItemData>(mapItemId, out var outValue))
            {
                return null;
            }

            var stack = m_pools[outValue.Id];
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
            
            return Alloc(outValue);
        }

        public MapItemBase GetMapItem(int mapItemId, Vector3 pos)
        {
            var mapItem = GetMapItem(mapItemId);
            mapItem.Pos = pos;
            return mapItem;
        }

        public void ReturnMapItem(MapItemBase mapItemBase)
        {
            ResetItem(mapItemBase, m_mapItemDatas[mapItemBase.Id]);
            m_pools[mapItemBase.Id].Push(mapItemBase);
        }
        
        /// <summary>
        /// alloc一个mapitem需要提供一个数据
        /// </summary>
        /// <param name="mapItemData"></param>
        /// <returns></returns>
        private MapItemBase Alloc(MapItemData mapItemData)
        {
            var mapItem = (MapItemBase) Activator.CreateInstance(
                m_assembly.GetType(m_fullName.Replace("MapItemBase", mapItemData.Name)));
            ResetItem(mapItem, mapItemData);
            return mapItem;
        }
        
        /// <summary>
        /// 在这里将需要的数据全部拿到
        /// </summary>
        /// <returns></returns>
        public IEnumerator PreInit()
        {
            if (!DataTableMgr.Instance.TryGetDataTable(out MapItemDataTable mapItemDataTable))
            {
                throw new Exception("未找到该数据表 : MapItemDataTable");
            }

            m_assembly = Assembly.GetAssembly(typeof(MapItemBase));
            m_fullName = typeof(MapItemBase).FullName;
            if (string.IsNullOrEmpty(m_fullName))
            {
                throw new Exception("MapItemBase全限定名获取失败");
            }
            
            m_pools = new Dictionary<int, Stack<MapItemBase>>();
            m_mapItemDatas = new Dictionary<int, MapItemData>();

            foreach (var data in mapItemDataTable.Datas)
            {
                var stack = new Stack<MapItemBase>();

                for (var i = 0; i < m_preInitNums; i++)
                {
                    stack.Push(Alloc(data));
                }
                m_pools.Add(data.Id, stack);
                m_mapItemDatas.Add(data.Id, data);
                //todo 初始化完成后还要将sprite等加载进来，但是得在atlas管理器完成后才能继续写这里的代码
            }
            
            yield return null;
        }

        public static MapItemBase InstantiateMapItemBase(string name)
        {
            var asm = Assembly.GetAssembly(typeof(MapItemBase));
            var fullName = typeof(MapItemBase).FullName;
            return (MapItemBase) Activator.CreateInstance(asm.GetType(fullName.Replace("MapItemBase", name)));
        }

        public List<MapItemData> GetMapItemNames()
        {
            var ret = new List<MapItemData>();

            foreach (var data in m_mapItemDatas.Values)
            {
                ret.Add(data);
            }

            return ret;
        }

        public override void OnDestroy()
        {
            foreach (var pool in m_pools)
            {
                pool.Value.Clear();
            }
            m_pools.Clear();
        }
    }
}