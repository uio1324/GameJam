using System;

namespace Logic.Manager.PrefabMgr
{
    public class PrefabPathAttribute : Attribute
    {
        
    }
    public static class PrefabPathDefine
    {
        public const string PREFAB_PATH_BASE = "Prefabs/";
        [PrefabPath] public const string PREFAB_PATH_MAP_DECORATION = "LevelItem/MapDecorationPrefab";
        [PrefabPath] public const string PREFAB_PATH_MAP_ITEM_COMBINER = "LevelItem/MapItemCombinerPrefab";
        [PrefabPath] public const string PREFAB_PATH_MAP_STONE = "LevelItem/MapStonePrefab";
        [PrefabPath] public const string PREFAB_PATH_MAP_TRIANGLE_STONE = "LevelItem/MapTriangleStonePrefab";
        [PrefabPath] public const string PREFAB_PATH_MAP_LADDER = "LevelItem/MapLadderPrefab";
        [PrefabPath] public const string PREFAB_PATH_MAP_LOGIC = "LevelItem/MapLogicItemPrefab";
        [PrefabPath] public const string PREFAB_PATH_UI_OPTION_PANEL = "UI/OptionPanelPrefab";
        [PrefabPath] public const string PREFAB_PATH_EFFECT_EXPLOSION = "Effect/Explosion";
    }
}