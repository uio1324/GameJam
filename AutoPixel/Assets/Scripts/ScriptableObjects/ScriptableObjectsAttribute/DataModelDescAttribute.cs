using System;
using ScriptableObjects.CommonDefine;
using UnityEngine;

namespace ScriptableObjects.ScriptableObjectsAttribute
{
	/// <summary>
	/// 数据属性用于告诉转表工具哪些字段是需要序列化的，数据属性需要传入一个类型让属性知道他所修饰的字段的泛型类型
	/// 并且会对该类型进行检查，因为该类型必须要继承自dataModel
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class DataModelDescAttribute : Attribute
	{
		public Type m_dataType;

		public DataModelDescAttribute(Type dataType)
		{
			if (!dataType.IsSubclassOf(typeof(DataModel)))
			{
				throw new Exception("该类型必须为DataModel的子类");
			}
			m_dataType = dataType;
		}
	}
}
