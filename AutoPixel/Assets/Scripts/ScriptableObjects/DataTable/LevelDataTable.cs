using System.Collections.Generic;
using System;
using ScriptableObjects.CommonDefine;
using ScriptableObjects.ScriptableObjectsAttribute;

namespace ScriptableObjects.DataTable
{
    public class LevelDataTable : DataTableBase
    {
        [DataModelDesc(typeof(LevelData))]
        public List<LevelData> Datas;
    }

    [Serializable]
    public class LevelData : DataModel
    {
        public int Level;
        public string MapFileName;
        public int IsFreezeZ;
        public int ScrollSpeed;
        public int InteractiveCoolDown;
        public int MaxHeight;
        public string BackgroundName;
        public string MiddlegroundName;
        public string BornPos;
        public int BeginSpeed;
        public int EndHeight;
        public string MGOffset;
        public string MGScale;
        public string BGOffset;
        public int LightDelta;
        public int LightDepressSpeed;
        public string ItemInnerParam;
        public string ItemOuterParam;
        public string ItemIntensityParam;
        public string MgBgInnerParam;
        public string MgBgOuterParam;
        public string MgBgIntensityParam;
        public int BGMusic;
    }
}

