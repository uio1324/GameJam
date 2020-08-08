using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Logic.Common.Define;
using ScriptableObjects.CommonDefine;
using ScriptableObjects.ScriptableObjectsAttribute;
using UnityEngine;

namespace Logic.Manager.DataTableMgr
{
    [ManagerDefine(0, true)]
    public sealed class DataTableMgr : Manager<DataTableMgr>
    {
        private readonly Dictionary<string, ConfigBase> m_configs;
        private readonly Dictionary<Type, DataTableBase> m_dataTables;

        public DataTableMgr()
        {
            m_configs = new Dictionary<string, ConfigBase>();
            m_dataTables = new Dictionary<Type, DataTableBase>();
        }
        /// <summary>
        /// 从指定属性修饰的实例和属性中萃取数据
        /// </summary>
        /// <param name="o">指定属性修饰的实例</param>
        /// <param name="outObject">输出数据</param>
        /// <typeparam name="T">指定的属性</typeparam>
        /// <returns>返回属性，用于获取属性中的字段</returns>
        private T ExtractDataByAttribute<T>(object o, out object outObject) where T : DataModelDescAttribute
        {
            var fieldInfos = o.GetType().GetFields();
            foreach (var fieldInfo in fieldInfos)
            {
                var attributes = fieldInfo.GetCustomAttributes(typeof(T), true);
                if (attributes.Length > 0)
                {
                    outObject = fieldInfo.GetValue(o);
                    return attributes[0] as T;
                }
            }

            outObject = null;
            return null;
        }
        /// <summary>
        /// 通过指定参数从类型元数据中萃取方法，这里的布尔值在后面将此接口扩展为通用接口时可用bitVector优化
        /// </summary>
        /// <param name="type">指定类型</param>
        /// <param name="methodName">方法名</param>
        /// <param name="isGeneric">方法是否为泛型方法</param>
        /// <param name="isStatic">是否为静态方法</param>
        /// <returns></returns>
        private MethodInfo ExtractMethodInfo(Type type, string methodName, bool isGeneric, bool isStatic)
        {
            var methods = type.GetMethods();
            MethodInfo method = null;
            foreach (var methodInfo in methods)
            {
                if (isGeneric == methodInfo.IsGenericMethod && isStatic == methodInfo.IsStatic && methodName == methodInfo.Name)
                {
                    method = methodInfo;
                }
            }

            return method;
        }

        private void LoadConfig()
        {
            var configs = Resources.LoadAll<ConfigBase>(Application.dataPath);
            foreach (var config in configs)
            {
                if (m_configs.ContainsKey(config.name))
                {
                    m_configs.Clear();
                    Debug.LogError($"配置表{config.name}表格重复，请检查Configs文件夹下是否有同名配置表");
                    continue;
                }

                m_configs[config.name] = config;
            }
        }
        
        private void LoadDataTable()
        {
            
            var dataTables = Resources.LoadAll<DataTableBase>(PathDefine.DATA_TABLE_PATH);
            foreach (var dataTable in dataTables)
            {
                var dataTableType = dataTable.GetType();
                var dataTableBase = dataTable;
                if (m_dataTables.ContainsKey(dataTableType))
                {
                    Debug.LogError($"数据表{dataTableType}表格重复，请检查DataTables文件夹下是否有类型相同的不同名配置表");
                }

                var attribute = ExtractDataByAttribute<DataModelDescAttribute>(dataTable, out var outValue);
                if (outValue != null)
                {
                    var subType = attribute.m_dataType;
                    var dataTableMethod = ExtractMethodInfo(dataTableType, "ConstructDataTable", true, false);
                    dataTableMethod.MakeGenericMethod(subType).Invoke(dataTable, new []{outValue});
                    m_dataTables.Add(dataTableType, dataTableBase);
                }
            }
        }

        public TConfigBase GetConfig<TConfigBase>(string variant = "") where TConfigBase : ConfigBase
        {
            var key = $"{typeof(TConfigBase).Name}{variant}";
            if (m_configs.TryGetValue(key, out var config))
            {
                return config as TConfigBase;
            }

            Debug.LogError($"配置表 : {key} 未找到");
            return null;
        }

        private TDataTableBase GetDataTable<TDataTableBase>() where TDataTableBase : DataTableBase
        {
            if (m_dataTables.TryGetValue(typeof(TDataTableBase), out var dataTableBase))
            {
                return dataTableBase as TDataTableBase;
            }
            
            Debug.LogError($"数据表 : {typeof(TDataTableBase)} 未找到，检查configs文件夹下是否有该类或者是否搞混了Config和DataTable");
            return null;
        }

        /// <summary>
        /// 直接获取整张数据表
        /// </summary>
        /// <param name="outValue"></param>
        /// <typeparam name="T">泛型为数据表的子类</typeparam>
        /// <returns></returns>
        public bool TryGetDataTable<T>(out T outValue) where T : DataTableBase
        {
            outValue = GetDataTable<T>();
            return outValue;
        }

        /// <summary>
        /// 通过给定的id和类型获取数据行
        /// </summary>
        /// <param name="id">指定id</param>
        /// <param name="outValue"></param>
        /// <typeparam name="TDataTableBase">数据表的类型</typeparam>
        /// <typeparam name="TDataModel">数据行的类型</typeparam>
        /// <returns></returns>
        public bool TryGetDataTableById<TDataTableBase, TDataModel>(int id, out TDataModel outValue)
            where TDataTableBase : DataTableBase where TDataModel : DataModel
        {
            if (TryGetDataTable(out TDataTableBase dataTable))
            {
                outValue = dataTable.GetDataModelById<TDataModel>(id);
                return outValue != null;
            }

            outValue = null;
            return false;
        }

        public override IEnumerator PreInit()
        {
            LoadConfig();
            LoadDataTable();
            yield return null;
        }
    }
}