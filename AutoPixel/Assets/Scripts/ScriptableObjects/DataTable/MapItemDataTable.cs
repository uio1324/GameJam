using System;
using System.Collections.Generic;
using ScriptableObjects.CommonDefine;
using ScriptableObjects.ScriptableObjectsAttribute;

namespace ScriptableObjects.DataTable
{
    public class MapItemDataTable : DataTableBase
    {
        [DataModelDesc(typeof(MapItemData))]
        public List<MapItemData> Datas;
    }
    [Serializable]
    public class MapItemData : DataModel
    {
        public string Name;
        public string AtlasName;
        public string SpriteName;
        public int HorizontalSpeed;
        public int IsInteractive;
        public int Size;
        public int CanMoveUpward;
        public string DisplayName;
        public int EventId;
        public int CanBeBreak;
        public int VerticalSpeed;
    }
}