using System;
using UnityEngine;

namespace ScriptableObjects.ScriptableObjectsAttribute
{
    /// <summary>
    /// 指定字段类型属性存在的意义在于显式的提醒制表者注意类型，表格中的类型暂时只支持到整型和字符串
    /// </summary>
    public class SpecifyFieldTypeAttribute : PropertyAttribute
    {
        public Type m_fieldType;

        public SpecifyFieldTypeAttribute(Type fieldType)
        {
            if (fieldType == typeof(int) || fieldType == typeof(string))
            {
                m_fieldType = fieldType;
            }
            else
            {
                throw new Exception("暂时不支持整型和字符串以外的格式");
            }
        }
    }
}
