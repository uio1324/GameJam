using System.Collections;
using Logic.Map.LevelMap.MapDescriber;
using UnityEngine;
using System.Collections.Generic;
using Logic.Manager.DataTableMgr;
using ScriptableObjects.DataTable;

namespace Logic.Manager.MapManager
{
    [ManagerDefine(30, false)]
    public class MapManager : Manager<MapManager>, IManager
    {
        private const string MapDir = "Map/";
        private Dictionary<int, string> m_levelMapping;

        public override void OnAwake()
        {
            m_levelMapping = new Dictionary<int, string>();
        }

        public IEnumerator PreInit()
        {
            if(DataTableMgr.DataTableMgr.Instance.TryGetDataTable(out LevelDataTable outValue))
            {
                m_levelMapping.Clear();
                var datas = outValue.Datas;
                for(int i = 0; i < datas.Count; i++)
                {
                    var filename = datas[i].MapFileName;
                    var textAsset = Resources.Load<TextAsset>(MapDir + filename);
                    if(!textAsset)
                    {
                        Debug.LogError($"{filename} 文件未找到，加载失败");
                    }
                    else
                    {
                        m_levelMapping.Add(datas[i].Id, textAsset.text);
                    }
                }
            }
            
            yield return null;
        }

        public void LoadLevel(int levelId)
        {
            if(m_levelMapping.TryGetValue(levelId, out var outValue))
            {
                MapDescriber.Instance.Deserialize(outValue);
            }
            else
            {
                Debug.LogError($"第 {levelId} 关关卡文件加载失败");
            }
        }
    }
}