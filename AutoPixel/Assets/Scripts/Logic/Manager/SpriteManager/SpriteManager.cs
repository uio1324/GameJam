using System.Collections;
using System.Collections.Generic;
using ScriptableObjects.DataTable;
using UnityEngine;
using UnityEngine.U2D;

namespace Logic.Manager.SpriteManager
{
    [ManagerDefine(1, true)]
    public sealed class SpriteManager : Manager<SpriteManager>, IManager
    {
        private Dictionary<string, SpriteAtlas> m_atlases;
        private Dictionary<string, Sprite> m_backgrounds;
        private Dictionary<string, Sprite> m_middlegrounds;
        private const string m_filePath = "Atlas/";
        private const string m_backgroundFilePath = "Textures/Background";
        private const string m_middlegroundFilePath = "Textures/Middleground";
        public IEnumerator PreInit() // 在这里加载图集
        {
            m_atlases = new Dictionary<string, SpriteAtlas>();
            m_backgrounds = new Dictionary<string, Sprite>();
            m_middlegrounds = new Dictionary<string, Sprite>();
            var atlas = Resources.LoadAll<SpriteAtlas>(m_filePath);
            foreach (var spriteAtlas in atlas)
            {
                m_atlases.Add(spriteAtlas.name, spriteAtlas);
            }

            if (DataTableMgr.DataTableMgr.Instance.TryGetDataTable(out LevelDataTable outValue))
            {
                foreach (var levelData in outValue.Datas)
                {
                    var sprites = Resources.LoadAll<Sprite>(m_backgroundFilePath + "/" + levelData.Level + "/");
                    foreach (var sprite in sprites)
                    {
                        m_backgrounds.Add(levelData.Level + "/" + sprite.name, sprite);
                    }

                    sprites = Resources.LoadAll<Sprite>(m_middlegroundFilePath + "/" + levelData.Level + "/");
                    foreach (var sprite in sprites)
                    {
                        m_middlegrounds.Add(levelData.Level + "/" + sprite.name, sprite);
                    }
                }
            }
            yield return null;
        }

        

        public Sprite GetSprite(string atlasName, string spriteName)
        {
            if (atlasName == "background")
            {
                if (m_backgrounds.TryGetValue(spriteName, out var sprite))
                {
                    return sprite;
                }
                return null;
            }

            if (atlasName == "middleground")
            {
                if (m_middlegrounds.TryGetValue(spriteName, out var sprite))
                {
                    return sprite;
                }

                return null;
            }
            if (m_atlases.TryGetValue(atlasName, out var outValue))
            {
                return outValue.GetSprite(spriteName);
            }

            return null;
        }

        public Texture GetTexture(string TextureName)
        {
            return null;
        }

        
    }
}