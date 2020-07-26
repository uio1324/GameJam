using System;
using Logic.Manager.EventMgr;
using Logic.Manager.PlayerManager;
using ScriptableObjects.DataTable;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using EventType = Logic.Manager.EventMgr.EventType;

namespace Render.LightEffector
{
    public class LightDepresser : MonoBehaviour
    {
        public PulseLight mapItemEffector;
        public PulseLight bgmgEffector;

        public float LightLevel
        {
            get => m_lightLevel;

            set => m_lightLevel = Mathf.Clamp(value, 0f, 100f);
        }

        private float m_lightLevel;

        private float m_lightDelta;
        private float m_lightDepressSpeed;
        private float[] m_itemInnerParam;
        private float[] m_itemOuterParam;
        private float[] m_itemIntensityParam;
        private float[] m_mgbgInnerParam;
        private float[] m_mgbgOuterParam;
        private float[] m_mgbgIntensityParam;
        public void InitData(LevelData levelData)
        {
            m_lightDelta = levelData.LightDelta;
            m_lightDepressSpeed = levelData.LightDepressSpeed * 0.001f;
            var str = "";
            
            str = levelData.ItemInnerParam;
            var param = str.Split(',');
            m_itemInnerParam = new[]
            {
                float.Parse(param[0]) * 0.001f, 
                float.Parse(param[1]) * 0.001f, 
                float.Parse(param[2]) * 0.001f,
                float.Parse(param[3]) * 0.001f
            };

            str = levelData.ItemOuterParam;
            param = str.Split(',');
            m_itemOuterParam = new[]
            {
                float.Parse(param[0]) * 0.001f, 
                float.Parse(param[1]) * 0.001f, 
                float.Parse(param[2]) * 0.001f,
                float.Parse(param[3]) * 0.001f
            };

            str = levelData.ItemIntensityParam;
            param = str.Split(',');
            m_itemIntensityParam = new[]
            {
                float.Parse(param[0]) * 0.001f, 
                float.Parse(param[1]) * 0.001f, 
                float.Parse(param[2]) * 0.001f,
                float.Parse(param[3]) * 0.001f
            };

            str = levelData.MgBgInnerParam;
            param = str.Split(',');
            m_mgbgInnerParam = new[]
            {
                float.Parse(param[0]) * 0.001f, 
                float.Parse(param[1]) * 0.001f, 
                float.Parse(param[2]) * 0.001f,
                float.Parse(param[3]) * 0.001f
            };

            str = levelData.MgBgOuterParam;
            param = str.Split(',');
            m_mgbgOuterParam = new[]
            {
                float.Parse(param[0]) * 0.001f, 
                float.Parse(param[1]) * 0.001f, 
                float.Parse(param[2]) * 0.001f,
                float.Parse(param[3]) * 0.001f
            };

            str = levelData.MgBgIntensityParam;
            param = str.Split(',');
            m_mgbgIntensityParam = new[]
            {
                float.Parse(param[0]) * 0.001f, 
                float.Parse(param[1]) * 0.001f, 
                float.Parse(param[2]) * 0.001f,
                float.Parse(param[3]) * 0.001f
            };
        }

        private void Awake()
        {
            EventMgr.Instance.Register(EventType.FillLight, OnLightLevelUp);
        }

        private void OnDestroy()
        {
            EventMgr.Instance.UnRegister(EventType.FillLight, OnLightLevelUp);
        }

        private void FixedUpdate()
        {
            var step = Time.fixedDeltaTime * m_lightDepressSpeed;
            step *= PlayerManager.Instance.GetPlayerScript().IsControlling() ? 2 : 1;
            LightLevel -= step;

            // y = kx + b
            var min = m_lightLevel * m_itemInnerParam[0] + m_itemInnerParam[1];
            var max = m_lightLevel * m_itemInnerParam[2] + m_itemInnerParam[3];
            mapItemEffector.InnerRadiusMin = min;
            mapItemEffector.InnerRadiusMax = max;

            min = m_lightLevel * m_itemOuterParam[0] + m_itemOuterParam[1];
            max = m_lightLevel * m_itemOuterParam[2] + m_itemOuterParam[3];
            mapItemEffector.OuterRadiusMin = min;
            mapItemEffector.OuterRadiusMax = max;

            min = m_lightLevel * m_itemIntensityParam[0] + m_itemIntensityParam[1];
            max = m_lightLevel * m_itemIntensityParam[2] + m_itemIntensityParam[3];
            mapItemEffector.IntensityMin = min;
            mapItemEffector.IntensityMax = max;

            min = m_lightLevel * m_mgbgInnerParam[0] + m_mgbgInnerParam[1];
            max = m_lightLevel * m_mgbgInnerParam[2] + m_mgbgInnerParam[3];
            bgmgEffector.InnerRadiusMin = min;
            bgmgEffector.InnerRadiusMax = max;

            min = m_lightLevel * m_mgbgOuterParam[0] + m_mgbgOuterParam[1];
            max = m_lightLevel * m_mgbgOuterParam[2] + m_mgbgOuterParam[3];
            bgmgEffector.OuterRadiusMin = min;
            bgmgEffector.OuterRadiusMax = max;

            min = m_lightLevel * m_mgbgIntensityParam[0] + m_mgbgIntensityParam[1];
            max = m_lightLevel * m_mgbgIntensityParam[2] + m_mgbgIntensityParam[3];
            bgmgEffector.IntensityMin = min;
            bgmgEffector.IntensityMax = max;
        }

        private void OnLightLevelUp()
        {
            LightLevel += m_lightDelta;
        }
    }
}