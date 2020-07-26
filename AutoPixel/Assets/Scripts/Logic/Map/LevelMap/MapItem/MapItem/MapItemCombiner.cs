using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Core;
using UI.CommonUI;
using Logic.Manager.AudioMgr;
using Logic.Manager.InputManager;
using Logic.Manager.DataTableMgr;
using Logic.Map.LevelMap.MapItemCommon;
using Logic.Map.LevelMap.MapItemCommon.Component;
using Logic.Map.LevelMap.MapItemCommon.Pool;
using ScriptableObjects.DataTable;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

namespace Logic.Map.LevelMap.MapItem.MapItem
{
    [Serializable]
    public class MapItemCombiner : MapItemBase
    {
        [SerializeField] public List<MapItemBase> MapItems;
        private readonly float m_powerDelayTime = 0.2f; // configurable
        public bool m_isUnderPlayerControl;
        private bool m_isMovingUpwards;
        private float m_maxRadius;
        private Dictionary<int, List<MapLogicItem>> m_sameEventItems;
        private int m_elemsCountUnderPlayerFeet;
        private bool m_hasPower;
		private float m_powerDelayTimer;
        private MapItemBase m_runeStone;

        public MapItemCombiner()
        {
            MapItems = new List<MapItemBase>();
            m_sameEventItems = new Dictionary<int, List<MapLogicItem>>();
        }

        public override void OnAppear()
        {
            m_owner = MapLogic.m_instance.GetBehaviorObject<MapItemCombinerObjectPool>();
            base.OnAppear();
            m_isUnderPlayerControl = false;
            m_isMovingUpwards = false;
            m_elemsCountUnderPlayerFeet = 0;
            m_hasPower = false;
            m_powerDelayTimer = 0.0f;

            var isIncreaseEdit = !MapItems[0].m_owner;

            if (!Util.Util.IsEditorScene()) // 如果是在游戏模式，根据反序列化出来的数据按照标准流程获取完整Item然后更新到list里
            {
                RegenerateItems();
            }
            else // 如果是在编辑器模式
            {
                // 如果编辑器下owner是空，说明items里面是纯数据，这个combiner是由序列化文件反序列化出来的，即通过load按钮要做增量编辑
                // 而非正常实例化流程
                
                if (isIncreaseEdit)
                {
                    RegenerateItems();
                }
                
                ReLocation();
                
                foreach (var mapItemBase in MapItems)
                {
                    mapItemBase.m_owner.transform.parent = m_owner.transform;
                    var combinable = mapItemBase as ICombinable;
                    combinable?.BeCombined();
                }
            }
            CalculateMaxRadius();
            if (isIncreaseEdit)
            {
                m_owner.transform.position = Pos;
                m_owner.transform.localScale = Scale;
            }
            // Set comboiner's rigidbody freezeRotation.
            LevelData levelData = MapLogic.m_instance.levelData;
            m_owner.Rigidbody2D.freezeRotation = (levelData.IsFreezeZ == 1) || (m_canMoveUpward == false);
            m_runeStone = GetRuneStone();
        }

        public override void OnDisappear()
        {
            foreach (var mapItem in MapItems)
            {
                mapItem.OnDisappear();
            }

            m_runeStone = null;
            m_isUnderPlayerControl = false;
            m_isMovingUpwards = false;
            MapItems.Clear();
            m_owner.HostedItem = null;
            m_owner.Rigidbody2D.bodyType = RigidbodyType2D.Static;
            GameObjectPool.Return(m_owner);
            MapItemPool.Instance.ReturnMapItem(this);
        }

        public override void Update()
        {
            if (!Util.Util.IsEditorScene())
            {
                m_powerDelayTimer -= Time.deltaTime;

                if (m_powerDelayTimer > 0.0f || (m_hasPower && (m_isUnderPlayerControl || m_isMovingUpwards)))
                {
                    float horizontalSpeed = 0.0f;
                    float verticalSpeed = 0.0f;

                    if (m_isUnderPlayerControl)
                    {
                        horizontalSpeed = InputManager.Instance.Axis.x * (float)m_horizontalSpeed * 0.001f;
                    }
                    if (m_powerDelayTimer > 0.0f || m_isMovingUpwards)
                    {
                        verticalSpeed = (float)m_verticalSpeed * 0.001f;
                    }

                    if (MapLogic.m_instance.isOver || OptionPanelUi.s_unique)
                    {
                        verticalSpeed = 0f;
                    }
                    m_owner.Rigidbody2D.velocity = new Vector2(horizontalSpeed, verticalSpeed);
                }
                else if (m_hasPower == false && m_powerDelayTimer <= -float.Epsilon)
                {
                    m_owner.Rigidbody2D.bodyType = RigidbodyType2D.Static;
                }

                var bounds = MapLogic.m_instance.GetUpperAndLower();
                if (m_owner.transform.position.y + m_maxRadius < bounds.x)
                {
                    OnDisappear();
                }
            }
        }
        
        public override void OnDestroy()
        {
            foreach (var mapItem in MapItems)
            {
                mapItem.OnDestroy();
            }
            
            MapItems.Clear();
            m_owner.HostedItem = null;
            MapItemPool.Instance.ReturnMapItem(this);
        }

        public override void UpdateDatas()
        {
            foreach (var mapItem in MapItems)
            {
                mapItem.UpdateDatas();
            }
            base.UpdateDatas();
        }

        public override void OnTriggerIn(Collider2D collider2D)
        {
            if (collider2D.gameObject.layer == LayerMask.NameToLayer("Stone"))
            {
                AudioMgr.Instance.Play(AudioDefine.Collide);
            }
        }

        public override void OnTriggerOut(Collider2D collider2D)
        {
            
        }

        private IEnumerator RuneStoneFadeIn()
        {
            if (m_runeStone != null)
            {
                var light2d = m_runeStone.m_owner.gameObject.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
                var maxIntensity = 0.8f;
                var curIntensity = 0f;
                light2d.enabled = true;

                while (curIntensity < maxIntensity)
                {
                    curIntensity += Time.deltaTime * 2f; // 0.5s完成渐变
                    light2d.intensity = curIntensity;
                    yield return null;
                }

                light2d.intensity = maxIntensity;
            }

            yield return null;
        }

        private IEnumerator RuneStoneFadeOut()
        {

            if (m_runeStone != null)
            {
                var light2d = m_runeStone.m_owner.gameObject.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
                var minIntensity = 0f;
                var curIntensity = light2d.intensity;

                while (curIntensity > minIntensity)
                {
                    curIntensity -= Time.deltaTime * 2f;
                    light2d.intensity = curIntensity;
                    yield return null;
                }

                light2d.enabled = false;
            }

            yield return null;
        }

        public void SetUnderPlayerControl(bool value)
        {
            if (value != m_isUnderPlayerControl)
            {
                if (value)
                {
                    GameRoot.m_instance.StartCoroutine(RuneStoneFadeIn());
                }
                else
                {
                    GameRoot.m_instance.StartCoroutine(RuneStoneFadeOut());
                }
            }
            
            m_isUnderPlayerControl = value;

            if (m_isUnderPlayerControl)
            {
                //m_owner.Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            }
            else if (!m_isMovingUpwards && m_powerDelayTimer <= -float.Epsilon)
            {
                //m_owner.Rigidbody2D.bodyType = RigidbodyType2D.Static;
            }
        }

        public void IncElemsCountUnderPlayerFeet()
        {
            ++m_elemsCountUnderPlayerFeet;

            if (m_elemsCountUnderPlayerFeet == 1)
            {
                if (m_canMoveUpward)
                {
                    m_isMovingUpwards = true;
                    //m_owner.Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }

        public void DecElemsCountUnderPlayerFeet()
        {
            --m_elemsCountUnderPlayerFeet;
            if (m_elemsCountUnderPlayerFeet < 0)
            {
                Debug.LogError("aerlin");
            }

            if (m_elemsCountUnderPlayerFeet == 0)
            {
                m_isMovingUpwards = false;

                if (!m_isUnderPlayerControl && m_powerDelayTimer <= -float.Epsilon)
                {
                    //m_owner.Rigidbody2D.bodyType = RigidbodyType2D.Static;
                }
            }
        }

        public int GetElemsCountUnderPlayerFeet()
        {
            return m_elemsCountUnderPlayerFeet;
        }

        public Vector3 GetRuneStonePosition()
        {
            if (m_runeStone != null)
            {
                return m_runeStone.m_owner.transform.position;
            }
            return Vector3.zero;
        }

        private MapItemBase GetRuneStone()
        {
            foreach (var mapItemBase in MapItems)
            {
                if (mapItemBase.WithDecoration == AdditionalDecoration.Rune || mapItemBase.WithDecoration == AdditionalDecoration.RuneSnow)
                {
                    return mapItemBase;
                }
            }

            return null;
        }

        public void SetPower(bool value)
        {
            if (m_canMoveUpward)
            {
                if (value)
                {
                    m_owner.Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                }
                else if (m_hasPower)
                {
                    m_owner.Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    m_powerDelayTimer = m_powerDelayTime;
                }
                else
                {
                    m_owner.Rigidbody2D.bodyType = RigidbodyType2D.Static;
                }
            }
            m_hasPower = value;
        }

        /// <summary>
        /// 从给定GameObject中将其携带的MapItemComponent解压出来然后获取其中的托管物，调用前要保证GameObject中有该Component
        /// 并且其托管物已经OnAppear过。即走标准流程出来的子物体。
        /// </summary>
        /// <param name="list"></param>
        private void ExtractList(List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                MapItems.Add(list[i].GetComponent<MapItemComponent>().HostedItem);
            }
        }

        private void CalculateMaxRadius()
        {
            Vector3 center = m_owner.transform.position;
            float radius = 0;
            for (int i = 0; i < MapItems.Count; i++)
            {
                var trans = MapItems[i].m_owner.transform;
                var pos = trans.position;
                var scale = trans.localScale;
                scale /= 2;
                var localMax = pos + scale;
                var localMin = pos - scale;
                
                radius = Mathf.Max(radius, Vector2.Distance(center, new Vector2(localMax.x, localMax.y)));
                radius = Mathf.Max(radius, Vector2.Distance(center, new Vector2(localMin.x, localMin.y)));
                radius = Mathf.Max(radius, Vector2.Distance(center, new Vector2(localMax.x, localMin.y)));
                radius = Mathf.Max(radius, Vector2.Distance(center, new Vector2(localMin.x, localMax.y)));
            }
            m_maxRadius = radius;
        }

        private void ReLocation()
        {
            // 获取AABB 计算聚合体的位置
            Vector2 max = Vector2.negativeInfinity, min = Vector2.positiveInfinity;
            foreach (var o in MapItems)
            {
                var transform = o.m_owner.transform;
                var position = transform.position;
                var scale = transform.localScale / 2;
                scale /= 2;
                var localMax = position + scale;
                var localMin = position - scale;
                max.x = Mathf.Max(max.x, localMax.x);
                max.y = Mathf.Max(max.y, localMax.y);
                min.x = Mathf.Min(min.x, localMin.x);
                min.y = Mathf.Min(min.y, localMin.y);
            }

            var pos = (max + min) / 2;
            m_owner.transform.position = new Vector3(pos.x, pos.y, 0);
        }

        /// <summary>
        /// 重新生成item，反序列化以后items中全是数据而非实体，重新生成就是用数据请求实体的过程
        /// </summary>
        private void RegenerateItems()
        {
            var newItems = new List<MapItemBase>();
            bool isInteractive = IsInteractive;
            int horizontalSpeed = m_horizontalSpeed;
            int verticalSpeed = m_verticalSpeed;
            bool canMoveUp = m_canMoveUpward;
            bool canBebreak = m_canBeBreak;
            foreach (var o in MapItems)
            {
                var obj = ReGenerate(o);
                obj.OnAppear();
                obj.m_owner.transform.parent = m_owner.transform;
                if (obj.m_name == "MapStone")
                {
                    horizontalSpeed = obj.m_horizontalSpeed;
                    verticalSpeed = obj.m_verticalSpeed;
                    isInteractive = obj.IsInteractive;
                    canMoveUp = obj.m_canMoveUpward;
                    canBebreak = obj.m_canBeBreak;
                }
                else if (obj.m_name == "MapLogicItem")
                {
                    if (obj.EventId != 0)
                    {
                        List<MapLogicItem> logicItems;
                        if (m_sameEventItems.TryGetValue(obj.EventId, out logicItems))
                        {
                            logicItems.Add((MapLogicItem)obj);
                        }
                        else
                        {
                            logicItems = new List<MapLogicItem>();
                            logicItems.Add((MapLogicItem)obj);
                            m_sameEventItems.Add(obj.EventId, logicItems);
                        }
                    }
                }
                
                var combinable = obj as ICombinable;
                combinable?.BeCombined();
                newItems.Add(obj);
            }
            IsInteractive =isInteractive;
            m_canMoveUpward = canMoveUp;
            m_horizontalSpeed = horizontalSpeed;
            m_verticalSpeed = verticalSpeed;
            m_canBeBreak = canBebreak;
            MapItems = newItems;
        }

        public void HideAllSameLogicItem(int eventId)
        {
            if (m_sameEventItems.TryGetValue(eventId, out var logicItems))
            {
                if (logicItems.Count == MapItems.Count)
                {
                    OnDisappear();
                }
                else
                {
                    foreach (var mapLogicItem in logicItems)
                    {
                        mapLogicItem.OnDisappear();
                        MapItems.Remove(mapLogicItem);
                    }
                    logicItems.Clear();
                }
            }
        }

        public void HideSingleLogicItem(int eventId, int hashCode)
        {
            if (m_sameEventItems.TryGetValue(eventId, out var logicItems))
            {
                foreach (var mapLogicItem in logicItems)
                {
                    if (mapLogicItem.HashCode == hashCode)
                    {
                        mapLogicItem.OnDisappear();
                        MapItems.Remove(mapLogicItem);
                    }
                }
            }
        }

        public void Combine(List<GameObject> list)
        {
            ExtractList(list);
            OnAppear();
        }
    }
}