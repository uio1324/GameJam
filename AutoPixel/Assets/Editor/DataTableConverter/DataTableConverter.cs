using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Excel;
using ScriptableObjects.ScriptableObjectsAttribute;
using UnityEditor;
using UnityEngine;

namespace Editor.DataTableConverter
{
    public static class DataTableConverter
    {
        private static T GetSpecifyAttributeFromType<T>(Type type, out FieldInfo outFieldInfo,
            string fieldInfoParam = null) where T : PropertyAttribute
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
                var fileType = "";
                if (excelFile.Contains("DataTable.xlsx"))
                {
                    fileType = "DataTables";
                }

                if (excelFile.Contains("Config.xlsx"))
                {
                    fileType = "Configs";
                }

                if ("".Equals(fileType))
                {
                    Debug.Log($"文件 ：{excelFile}命名有误，应以Config或DataTable结尾，或非xlsx后缀文件");
                    continue;
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
                    var config = ScriptableObject.CreateInstance(tableName);
                    if (config == null)
                    {
                        Debug.Log($"未找到{tableName}类，请于Configs文件夹下手动添加类，需继承自Scriptable Object，且字段需一一对应");
                        continue;
                    }

                    var propertyNames = new List<string>();
                    var dataType = GetSpecifyAttributeFromType<DataAttribute>(config.GetType(), out var dataFieldInfo)
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
                            for (var l = 0; l < columns.Count; l++) // 遍历列
                            {
                                var value = rows[k][l].ToString();
                                var fieldType =
                                    GetSpecifyAttributeFromType<SpecifyFieldTypeAttribute>(dataType, out var fieldInfo,
                                        propertyNames[l]).m_fieldType;
                                if (fieldInfo == null || fieldType == null)
                                {
                                    continue;
                                }

                                if (fieldType == typeof(int))
                                {
                                    fieldInfo.SetValue(data, int.TryParse(value, out var outValue) ? outValue : 0);
                                }
                                else if (fieldType == typeof(string))
                                {
                                    fieldInfo.SetValue(data, value);
                                }
                                else if (fieldType == typeof(float))
                                {
                                    fieldInfo.SetValue(data, float.TryParse(value, out var outValue) ? outValue : 0.0);
                                }
                            }

                            iList.Add(data);
                        }
                    }

                    dataFieldInfo.SetValue(config, iList);
                    var resultPath = $"{Application.dataPath + "/Resources/" + fileType}/{tableName}.asset";
                    var tempPath = $"Assets/{tableName}.asset";
                    AssetDatabase.CreateAsset(config, tempPath);
                    if (File.Exists(resultPath))
                    {
                        File.Delete(resultPath);
                    }

                    File.Move(tempPath, resultPath);
                }

                excelDataReader.Close();
            }

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }
    }
}