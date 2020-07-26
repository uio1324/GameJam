using System;
using System.Collections.Generic;
using ScriptableObjects.CommonDefine;
using ScriptableObjects.ScriptableObjectsAttribute;

namespace ScriptableObjects.DataTable
{
    public class MapItemDataTable : DataTableBase
    {
        [Data(typeof(MapItemData))]
        public List<MapItemData> Datas;
    }
    [Serializable]
    public class MapItemData : DataModel
    {
        [SpecifyFieldType(typeof(string))]
        public string Name;
        [SpecifyFieldType(typeof(string))]
        public string AtlasName;
        [SpecifyFieldType(typeof(string))]
        public string SpriteName;
        [SpecifyFieldType(typeof(int))]
        public int HorizontalSpeed;
        [SpecifyFieldType(typeof(int))]
        public int IsInteractive;
        [SpecifyFieldType(typeof(int))]
        public int Size;
        [SpecifyFieldType(typeof(int))]
        public int CanMoveUpward;
        [SpecifyFieldType(typeof(string))]
        public string DisplayName;
        [SpecifyFieldType(typeof(int))]
        public int EventId;
        [SpecifyFieldType(typeof(int))]
        public int CanBeBreak;
        [SpecifyFieldType(typeof(int))]
        public int VerticalSpeed;
    }
}