using System.Collections.Generic;
using System;
using ScriptableObjects.CommonDefine;
using ScriptableObjects.ScriptableObjectsAttribute;

namespace ScriptableObjects.DataTable
{
    public class LevelDataTable : DataTableBase
    {
        [Data(typeof(LevelData))]
        public List<LevelData> Datas;
    }

    [Serializable]
    public class LevelData : DataModel
    {
        [SpecifyFieldType(typeof(int))]
        public int Level;
        [SpecifyFieldType(typeof(string))]
        public string MapFileName;
        [SpecifyFieldType(typeof(int))] 
        public int IsFreezeZ;
        [SpecifyFieldType(typeof(int))] 
        public int ScrollSpeed;
        [SpecifyFieldType(typeof(int))] 
        public int InteractiveCoolDown;
        [SpecifyFieldType(typeof(int))]
        public int MaxHeight;
        [SpecifyFieldType(typeof(string))]
        public string BackgroundName;
        [SpecifyFieldType(typeof(string))] 
        public string MiddlegroundName;
        [SpecifyFieldType(typeof(string))]
        public string BornPos;
        [SpecifyFieldType(typeof(int))]
        public int BeginSpeed;
        [SpecifyFieldType(typeof(int))]
        public int EndHeight;
        [SpecifyFieldType(typeof(string))]
        public string MGOffset;
        [SpecifyFieldType(typeof(string))]
        public string MGScale;
        [SpecifyFieldType(typeof(string))]
        public string BGOffset;
        [SpecifyFieldType(typeof(int))]
        public int LightDelta;
        [SpecifyFieldType(typeof(int))]
        public int LightDepressSpeed;
        [SpecifyFieldType(typeof(string))]
        public string ItemInnerParam;
        [SpecifyFieldType(typeof(string))]
        public string ItemOuterParam;
        [SpecifyFieldType(typeof(string))]
        public string ItemIntensityParam;
        [SpecifyFieldType(typeof(string))]
        public string MgBgInnerParam;
        [SpecifyFieldType(typeof(string))]
        public string MgBgOuterParam;
        [SpecifyFieldType(typeof(string))]
        public string MgBgIntensityParam;
        [SpecifyFieldType(typeof(int))]
        public int BGMusic;
    }
}

