using System;
using Logic.Map.LevelMap.MapItem.MapItem;
using UnityEngine;

namespace Logic.Map.LevelMap.MapItemCommon.Component
{
    /// <summary>
    /// 逻辑类与表现类是分离的，出现和消失由逻辑类完成，逻辑类驱动表现类
    /// 在出现和消失之间，逻辑类将自己托管给表现类，这段时间里是表现类驱动逻辑类
    /// 因为表现类有天生的active属性，update方法的消耗是动态的，让他来做update的驱动再合适不过
    /// 由于有子物体和聚合体之分，要保证destroy只被调用一次
    /// 本着子物体归聚合体管理的理念，子物体的component应当不实现destroy等方法
    /// 直接与场景交互的如decoration、combiner才应该实现destroy等方法
    /// </summary>
    public class MapItemComponent : MonoBehaviour
    {
        [Tooltip("被托管于表现类，受表现类update方法驱动的逻辑类")]
        public MapItemBase HostedItem;

        public Rigidbody2D Rigidbody2D;

        public Collider2D Collider2D;

        public SpriteRenderer SpriteRenderer;
    }
}