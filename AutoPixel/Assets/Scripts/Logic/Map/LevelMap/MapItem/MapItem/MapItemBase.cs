using System;
using System.Collections.Generic;
using Logic.Manager.SpriteManager;
using Logic.Map.LevelMap.MapItemCommon;
using Logic.Map.LevelMap.MapItemCommon.Component;
using Logic.Map.LevelMap.MapItemCommon.Pool;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItem.MapItem
{
    public enum AdditionalDecoration
    {
        None = 0,
        Snow1 = 1,
        Grass = 2,
        Rune = 3,
        Snow2 = 4,
        RuneSnow = 5
    }
    /// <summary>
    /// 这里本应该是抽象基类，但是抽象基类引用的物体是无法被unity序列化的，因此有些地方可能会new此类，要注意
    /// 不实现的方法默认会跑这里的空方法
    /// </summary>
    [Serializable]
    public class MapItemBase
    {
        public Vector3 Pos; // 位置
        public int Id; // mapItem类型索引
        public int RotationZ; // 旋转角度
        public int RotationY;
        public int RotationX;
        public bool IsInteractive; // 是否可交互，用于触发器判断
        public int HashCode; // 用于辨别单独的item
        public int EventId;
        public string TimeLineName;
        public AdditionalDecoration WithDecoration = AdditionalDecoration.None;
        public Vector3 Scale = Vector3.one;

        [NonSerialized] public string m_spriteName;
        [NonSerialized] public string m_name;
        [NonSerialized] public string m_atlasName;
        [NonSerialized] public int m_size;
        [NonSerialized] public int m_verticalSpeed;
        [NonSerialized] public int m_horizontalSpeed;
        [NonSerialized] public MapItemComponent m_owner;
        [NonSerialized] public bool m_beCombined;
        [NonSerialized] public bool m_canMoveUpward;
        [NonSerialized] public bool m_canBeBreak;

        private static Dictionary<int, MapItemComponent> m_hashcodeMapping = new Dictionary<int, MapItemComponent>();

        public virtual void Update()
        {
            
        }
        
        public virtual void OnAppear()
        {
            m_owner.HostedItem = this;
            m_owner.transform.position = Pos;
            m_owner.transform.localScale = Scale;
            m_owner.transform.Rotate(Vector3.forward, RotationZ);
            m_owner.transform.Rotate(Vector3.right, RotationX);
            m_owner.transform.Rotate(Vector3.up, RotationY);
            
            if (IsInteractive)
            {
                m_owner.gameObject.tag = "Interactive";
            }

            if (m_owner.SpriteRenderer)
            {
                GenerateSprite();
            }

            if (HashCode != 0)
            {
                while (m_hashcodeMapping.ContainsKey(HashCode))
                {
                    // 复用表现类时会有键值重复问题，这里直接让键值自增，保证逻辑类hashcode唯一
                    HashCode++;
                }
                m_hashcodeMapping.Add(HashCode, m_owner);
            }
        }

        private void GenerateSprite()
        {
            var spriteName = m_spriteName;
            if (string.IsNullOrEmpty(spriteName) || string.IsNullOrEmpty(m_atlasName))
            {
                return;
            }
            
            var fallbackSprite = SpriteManager.Instance.GetSprite(m_atlasName, spriteName);

            spriteName += GenerateSuffix();
            var sprite = SpriteManager.Instance.GetSprite(m_atlasName, spriteName);
            if (sprite)
            {
                m_owner.SpriteRenderer.sprite = sprite;
                return;
            }

            if (fallbackSprite)
            {
                //Debug.LogError($"未找到名为：{spriteName} 的图，回退为：{fallbackSprite.name}");
                m_owner.SpriteRenderer.sprite = fallbackSprite;
            }
        }

        private string GenerateSuffix()
        {
            switch (WithDecoration)
            {
                case AdditionalDecoration.None:
                    return "";
                case AdditionalDecoration.Snow1:
                    return "_Snow1";
                case AdditionalDecoration.Grass:
                    return "_Grass";
                case AdditionalDecoration.Rune:
                    return "_Rune";
                case AdditionalDecoration.Snow2:
                    return "_Snow2";
                case AdditionalDecoration.RuneSnow:
                    return "_RuneSnow";
            }

            return "";
        }

        public virtual void OnDisappear()
        {
            m_beCombined = false;
            if (m_owner != null)
            {
                GameObjectPool.Return(m_owner);
                m_owner.HostedItem = null;
                m_owner.gameObject.tag = "Default";
                m_owner = null;
                if (HashCode != 0)
                {
                    m_hashcodeMapping.Remove(HashCode);
                }
                MapItemPool.Instance.ReturnMapItem(this);
            }
        }

        /// <summary>
        /// OnDestroy是在场景被摧毁的时候会调用，一般是游戏时切场景和编辑器关闭的时候调用
        /// 因此在这里不必还回对象池也不必将成员变量reset，直接将自己摧毁即可，对于聚合物来说
        /// 直接调用自己持有的子物体的OnDestroy即可
        /// </summary>
        public virtual void OnDestroy()
        {
            
        }
        
        public virtual void OnTriggerIn(Collider2D collider2D)
        {
            
        }

        public virtual void OnTriggerOut(Collider2D collider2D)
        {
            
        }
        
        /// <summary>
        /// 更新数据，如果在scene面板下编辑了item位置以后需要调用此接口进行数据更新
        /// 防止gameobject与item类数据不同步
        /// </summary>
        public virtual void UpdateDatas()
        {
            var transform = m_owner.transform;
            var eulerAngles = transform.rotation.eulerAngles;
            Pos = transform.position;
            RotationX = (int) eulerAngles.x & 360;
            RotationY = (int) eulerAngles.y % 360;
            RotationZ = (int) eulerAngles.z % 360;
            Scale = transform.localScale;
            if (HashCode == 0)
            {
                HashCode = GetHashCode();
            }
        }

        public static MapItemComponent GetItemComponent(int hashcode)
        {
            if (m_hashcodeMapping.TryGetValue(hashcode, out var outValue))
            {
                return outValue;
            }
            return null;
        }

        /// <summary>
        /// 重新生成item，在从序列化数据重新生成item实体的时候调用此函数从源数据中取需要被反序列化的数据
        /// 另一部分是mapItemPool中的ResetItem中读表的字段，mapItem的数据由此两部分组成
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static MapItemBase ReGenerate(MapItemBase src)
        {
            var obj = MapItemPool.Instance.GetMapItem(src.Id);
            obj.Pos = src.Pos;
            obj.RotationX = src.RotationX;
            obj.RotationY = src.RotationY;
            obj.RotationZ = src.RotationZ;
            obj.IsInteractive = src.IsInteractive;
            obj.HashCode = src.HashCode;
            obj.TimeLineName = src.TimeLineName;
            obj.WithDecoration = src.WithDecoration;
            obj.Scale = src.Scale;
            return obj;
        }
    }
}