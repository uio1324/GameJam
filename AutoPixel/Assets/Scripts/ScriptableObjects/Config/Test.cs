using System;
using System.Collections.Generic;
using ScriptableObjects.CommonDefine;
using ScriptableObjects.ScriptableObjectsAttribute;

namespace ScriptableObjects.Config
{
	public class Test : ConfigBase
	{
		[Data(typeof(TestData))]
		public List<TestData> Datas;
	
		[Serializable]
		public class TestData : DataModel
		{
			[SpecifyFieldType(typeof(int))]
			public int Test1;
			[SpecifyFieldType(typeof(string))]
			public string Test2;
			[SpecifyFieldType(typeof(string))]
			public string Test3;
		}
	}
}
