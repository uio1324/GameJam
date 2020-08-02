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
        private const string m_filePath = "Atlas/";
        public override IEnumerator PreInit() // 在这里加载图集
        {
            m_atlases = new Dictionary<string, SpriteAtlas>();
            var atlas = Resources.LoadAll<SpriteAtlas>(m_filePath);
            foreach (var spriteAtlas in atlas)
            {
                m_atlases.Add(spriteAtlas.name, spriteAtlas);
            }
            yield return null;
        }

        

        public Sprite GetSprite(string atlasName, string spriteName)
        {
            if (m_atlases.TryGetValue(atlasName, out var outValue))
            {
                return outValue.GetSprite(spriteName);
            }

            return null;
        }
    }
}