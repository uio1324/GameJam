using System;
using Logic.Manager.EventMgr;
using Logic.Manager.PlayerManager;
using Logic.Map.LevelMap;
using UnityEngine;
using EventType = Logic.Manager.EventMgr.EventType;

namespace Logic.Temp
{
    public class TempScroller : MonoBehaviour
    {
        public float ScrollSpeed;

        public bool AllowScroll
        {
            set => m_allowScroll = value;
            get => m_allowScroll && !MapLogic.m_instance.isOver;
        }

        private bool m_allowScroll;
        public float Upper;
        public float Lower;
        private float m_needSpeedUpHeight; // 玩家高度高于此高度时需要将摄像机插值至玩家高度直至玩家处于安全线以下
        private float m_needSpeedDownHeight;
        private bool m_needSpeedUp;
        private bool m_needSpeedDown;
        private float m_playerHeight;
        private Camera m_camera;

        private void Start()
        {
            m_camera = GetComponent<Camera>();
            EventMgr.Instance.Register(EventType.BeginScroll, Scroll);
        }

        private void OnDestroy()
        {
            EventMgr.Instance.UnRegister(EventType.BeginScroll, Scroll);
        }

        private void Scroll()
        {
            AllowScroll = true;
        }

        public void SetScrollSpeed(int scrollSpeed)
        {
            ScrollSpeed = scrollSpeed;
        }

		public float GetCameraOrthographicSize()
		{
			return m_camera.orthographicSize;
		}

        public void Reset()
        {
            UpdateZone();
        }

        private void UpdateZone()
        {
            float y = transform.position.y;
            float size = m_camera.orthographicSize;
            m_playerHeight = PlayerManager.Instance.m_player.position.y;
            Upper = y + size;
            Lower = y - size;
            m_needSpeedUpHeight = Lower + (Upper - Lower) * 0.6f;
            m_needSpeedDownHeight = Lower + (Upper - Lower) * 0.5f;
            m_needSpeedUp = m_playerHeight > m_needSpeedUpHeight;
            m_needSpeedDown = m_playerHeight < m_needSpeedDownHeight;
        }

        private void Update()
        {
            UpdateZone();
            if (Input.GetKeyDown(KeyCode.Return))
            {
                AllowScroll = !AllowScroll;
            }

            if (AllowScroll)
            {
                if (m_needSpeedUp)
                {
                    var position = transform.position;
                    float nextHeight = Mathf.Lerp(position.y, m_playerHeight, 0.01f);
                    position = new Vector3(0, nextHeight, -10);
                    transform.position = position;
                }
                else if (m_needSpeedDown)
                {
                    transform.position += ScrollSpeed * Time.deltaTime * 0.001f * 0.75f * Vector3.up;
                }
                else
                {
                    transform.position += ScrollSpeed * Time.deltaTime * 0.001f * Vector3.up;
                }
            }
        }
    }
}