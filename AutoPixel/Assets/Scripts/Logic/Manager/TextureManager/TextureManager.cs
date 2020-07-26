using System.Collections;
using System.Collections.Generic;
using Logic.Manager.AssetsManager;
using ScriptableObjects.DataTable;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

namespace Logic.Manager.TextureManager
{
    [ManagerDefine(1, true)]
    public sealed class TextureManager : Manager<TextureManager>, IManager
    {
        private Dictionary<string, AssetReference> m_Assets;
        
        public IEnumerator PreInit() // 在这里加载图集
        {
            m_Assets = new Dictionary<string, AssetReference>();
            yield return null;
        }
        public Texture GetTexture(string TextureName)
        {
            return null;
        }
        
    }
}