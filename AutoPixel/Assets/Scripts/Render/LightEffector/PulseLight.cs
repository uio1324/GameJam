using System;
using Logic.Manager.PlayerManager;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

namespace Render.LightEffector
{
    public class PulseLight : MonoBehaviour
    {
        private UnityEngine.Experimental.Rendering.Universal.Light2D m_light2D;
        public bool UseVariableScaler;
        public float Scaler = 1f;
        public float Phase = 0f;
        public bool EffectInnerRadius;
        public float InnerRadiusMax;
        public float InnerRadiusMin;
        public bool EffectOuterRadius;
        public float OuterRadiusMax;
        public float OuterRadiusMin;
        public bool EffectIntensity;
        public float IntensityMax;
        public float IntensityMin;

        private float time = 0;
        private void Start()
        {
            m_light2D = GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        }

        private void Update()
        {
            var x = Time.time;
            var sin = 0f;
            if(!UseVariableScaler)
            {
                sin = (Mathf.Sin(x * Scaler + Phase) + 1) / 2;
            }
            else
            {
                var t = PlayerManager.Instance.GetPlayerScript().GetLightRatio();
                var variableScaler = Mathf.Lerp(PlayerManager.Instance.m_maxPulseFrequency, PlayerManager.Instance.m_minPulseFrequency, t);
                time += Time.deltaTime * variableScaler;
                sin = (Mathf.Sin(time + Phase) + 1) / 2;
            }
            
            if (EffectInnerRadius)
            {
                m_light2D.pointLightInnerRadius = InnerRadiusMin + (InnerRadiusMax - InnerRadiusMin) * sin;
            }

            if (EffectOuterRadius)
            {
                m_light2D.pointLightOuterRadius = OuterRadiusMin + (OuterRadiusMax - OuterRadiusMin) * sin;
            }

            if (EffectIntensity)
            {
                m_light2D.intensity = IntensityMin + (IntensityMax - IntensityMin) * sin;
            }
        }
    }
}