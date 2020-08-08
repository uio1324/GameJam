using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.CommonDefine
{
    public abstract class DataTableBase: ScriptableObject
    {
        [NonSerialized]
        private Dictionary<int, DataModel> m_dataModels;
        public T GetDataModelById<T>(int id) where T : DataModel
        {
            if (m_dataModels.TryGetValue(id, out var dataModel))
            {
                return (T) dataModel;
            }

            return null;
        }

        public virtual void ConstructDataTable<T>(List<T> dataModels) where T : DataModel
        {
            if (dataModels == null)
            {
                return;
            }
            if (m_dataModels == null)
            {
                m_dataModels = new Dictionary<int, DataModel>();
            }
            else
            {
                m_dataModels.Clear();
            }

            foreach (var dataModel in dataModels)
            {
                var id = dataModel.Id;
                if (m_dataModels.ContainsKey(id))
                {
                    Debug.LogError($"{GetType()}表中行键值重复 ： {id}");
                }
                m_dataModels.Add(id, dataModel);
            }
        }
    }
}