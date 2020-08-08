using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Excel;
using ScriptableObjects.CommonDefine;
using ScriptableObjects.ScriptableObjectsAttribute;
using UnityEditor;
using UnityEngine;

namespace Editor.DataTableConverter
{
    public static class DataTableConverter
    {
        private static readonly Dictionary<Type, Func<string, object>> S_TryParseMethodMap = new Dictionary<Type, Func<string, object>>
        {
            {typeof(int), s => int.TryParse(s, out var result) ? result : 0},
            {typeof(string), s => s},
            {typeof(double), s => double.TryParse(s, out var result) ? result : 0.0},
            {typeof(float), s => float.TryParse(s, out var result) ? result : 0.0f}
        };
        private static T GetSpecifyAttributeFromType<T>(Type type, out FieldInfo outFieldInfo,
            string fieldInfoParam = null) where T : Attribute
        {
            var fieldInfos = type.GetFields();
            foreach (var f in fieldInfos)
            {
                var fieldInfo = fieldInfoParam != null ? type.GetField(fieldInfoParam) : f;

                var attributes = fieldInfo.GetCustomAttributes(typeof(T), false);
                if (attributes.Length != 0)
                {
                    outFieldInfo = fieldInfo;
                    return (T) attributes[0];
                }
            }

            outFieldInfo = null;
            return null;
        }

        [MenuItem("Tools/DataTable/DataTableConverter")]
        public static void ConvertDataTable()
        {
            var excelFiles =
                Directory.GetFiles(Application.dataPath + "/Excels", "*.xlsx", SearchOption.AllDirectories);
            EditorUtility.ClearProgressBar();
            for (var i = 0; i < excelFiles.Length; i++) // 遍历Excel
            {
                var excelFile = excelFiles[i];
                if (!excelFile.Contains("DataTable.xlsx"))
                {
                    Debug.LogWarning($"文件： {excelFile} 并非以DataTable.xlsx为后缀，可能并非配置表");
                }

                var stream = File.Open(excelFile, FileMode.Open, FileAccess.Read);
                var excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                var result = excelDataReader.AsDataSet();
                excelDataReader.IsFirstRowAsColumnNames = true;
                var tables = result.Tables;

                for (var j = 0; j < tables.Count; j++) // 遍历Excel里的table
                {
                    var columns = tables[j].Columns;
                    var rows = tables[j].Rows;
                    var tableName = tables[j].TableName;
                    var config = (DataTableBase)ScriptableObject.CreateInstance(tableName);
                    if (config == null)
                    {
                        Debug.LogError($"未找到{tableName}类，请于Configs文件夹下手动添加类，需继承自Scriptable Object，且字段需一一对应");
                        continue;
                    }

                    var propertyNames = new List<string>();
                    var dataType = GetSpecifyAttributeFromType<DataModelDescAttribute>(config.GetType(), out var dataFieldInfo)
                        .m_dataType;
                    if (dataFieldInfo == null || dataType == null)
                    {
                        continue;
                    }

                    var listType = typeof(List<>).MakeGenericType(dataType);
                    var iList = (System.Collections.IList)Activator.CreateInstance(listType);
                    for (var k = 0; k < rows.Count; k++) //遍历行
                    {
                        EditorUtility.DisplayProgressBar("DataTable Processing", "Converting",
                            (float) ((i + 1) * (j + 1) * (k + 1)) / (excelFiles.Length * tables.Count * rows.Count));
                        if (k == 0) // 默认第1行全部是字段名
                        {
                            for (var l = 0; l < columns.Count; l++)
                            {
                                propertyNames.Add(rows[0][l].ToString());
                            }
                        }
                        else
                        {
                            var data = Activator.CreateInstance(dataType);
                            var constructResultFlag = true;
                            for (var l = 0; l < columns.Count; l++) // 遍历列
                            {
                                var value = rows[k][l].ToString();
                                var fieldInfo = data.GetType().GetField(propertyNames[l]);
                                if (!S_TryParseMethodMap.TryGetValue(fieldInfo.FieldType, out var parseResult))
                                {
                                    constructResultFlag = false;
                                    continue;
                                }
                                
                                fieldInfo.SetValue(data, parseResult(value));
                            }

                            if (constructResultFlag)
                            {
                                iList.Add(data);
                            }
                        }
                    }

                    dataFieldInfo.SetValue(config, iList);
                    var resultPath = $"{Application.dataPath}/Resources/DataTables/{tableName}.asset";
                    var tempPath = $"Assets/{tableName}.asset";
                    AssetDatabase.CreateAsset(config, tempPath);
                    if (File.Exists(resultPath))
                    {
                        File.Delete(resultPath);
                    }

                    File.Move($"{Application.dataPath}/{tableName}.asset", resultPath);
                }

                excelDataReader.Close();
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }
    }
}