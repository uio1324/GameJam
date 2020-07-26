using System.Collections;
using System.Collections.Generic;
using Logic.Core;
using Logic.Map.LevelMap;
using Logic.Manager.AudioMgr;
using Logic.Temp;
using UnityEngine;
using EventType = Logic.Manager.EventMgr.EventType;

namespace Logic.Manager.ProgressManager
{
    [ManagerDefine(120, true)]
    public sealed class ProgressMgr : Manager<ProgressMgr>, IManager
    {
        public bool achievedTeachingLevel;
        private int m_curLevel;
        private Dictionary<int, HashSet<int>> m_savePointCache;
        private Vector3 m_playerPoint;
        private Vector3 m_cameraPoint;
        private HashSet<int> m_cache;
        private int m_mostClose;
        public override void OnAwake()
        {
            Reset();
            m_savePointCache = new Dictionary<int, HashSet<int>>();
            EventMgr.EventMgr.Instance.UnRegister(EventType.FinishLevel, OnFinishedLevel);
            EventMgr.EventMgr.Instance.Register(EventType.FinishLevel, OnFinishedLevel);
        }

        public void SaveProgress(int hashCode)
        {
            m_cache.Add(hashCode);

            m_playerPoint = PlayerManager.PlayerManager.Instance.m_player.position;
            m_cameraPoint = MapLogic.m_instance.GetCameraPos();
            m_mostClose = hashCode;
        }

        public int GetCurLevel()
        {
            if (m_curLevel == 0)
            {
                return 0;
            }
            return m_curLevel - DataTableMgr.DataTableMgr.DataTableMgrDefine.LEVEL_ID_BASE;
        }

        public void LoadProgress()
        {
            var transform = PlayerManager.PlayerManager.Instance.m_player;
            transform.GetComponent<TempPlayerController>().FSM.Reset();
            transform.position = m_playerPoint;
            MapLogic.m_instance.SetCameraPos(m_cameraPoint);
            MapLogic.m_instance.Reset();
            MapLogic.m_instance.SetAllowScroll(true);
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2();

            AudioMgr.AudioMgr.Instance.PlayBackBg();
            PlayerManager.PlayerManager.Instance.GetPlayerScript().AddLightTo(50f);

            GameRoot.m_instance.StartCoroutine(FadeInBgm());
        }

        private IEnumerator FadeInBgm()
        {
            var start = 0f;
            while(start < 1f)
            {
                AudioMgr.AudioMgr.Instance.SetBgVolume(start);
                start += Time.deltaTime;

                yield return null;
            }
            AudioMgr.AudioMgr.Instance.SetBgVolume(1);

            yield return null;
        }

        public bool HasSavePoint()
        {
            return m_mostClose != 0;
        }

        public void Reset()
        {
            if(PlayerPrefs.HasKey("progress"))
            {
                achievedTeachingLevel = true;
            }
            m_playerPoint = default(Vector3);
            m_cameraPoint = default(Vector3);
            m_mostClose = 0;
            if (m_cache == null)
            {
                m_cache = new HashSet<int>();
            }
            else
            {
                m_cache.Clear();
            }
        }

        private void OnFinishedLevel()
        {
            m_curLevel = GameRoot.m_instance.m_levelId + 1;
            if(!m_savePointCache.ContainsKey(m_curLevel))
            {
                m_savePointCache.Add(m_curLevel, m_cache);
            }
            else
            {
                m_savePointCache[m_curLevel] = m_cache;
            }
            m_mostClose = 0;
            m_cache.Clear();
        }

        public bool IsArrived(int hashCode)
        {
            if (m_savePointCache.TryGetValue(GameRoot.m_instance.m_levelId, out var cache))
            {
                if (cache.Contains(hashCode))
                {
                    return true;
                }
            }

            if (m_mostClose == hashCode)
            {
                return true;
            }

            return false;
        }

        public void RestartLevel()
        {
            m_cache.Clear();
            m_mostClose = 0;
        }

        public IEnumerator PreInit()
        {
            yield return null;
        }
    }
}