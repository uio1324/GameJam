using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.CommonDefine
{
    public class ConfigBase : ScriptableObject
    {
        [NonSerialized]
        private Dictionary<int, DataModel> m_dataModels;
        public T GetConfigById<T>(int id) where T : DataModel
        {
            if (m_dataModels.TryGetValue(id, out var dataModel))
            {
                return (T) dataModel;
            }

            return null;
        }
    }
}