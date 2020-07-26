using System;
using System.Collections.Generic;
using Logic.Core;
using UnityEngine;

namespace Logic.Map.LevelMap.ParticleSystem
{
    public class ParticleModifier : MonoBehaviour
    {
        public List<ParticlePair> particlePairs;
        private Renderer m_particleSystemRenderer;
        private void Awake()
        {
            m_particleSystemRenderer = GetComponent<Renderer>();
            foreach (var particlePair in particlePairs)
            {
                if (GameRoot.m_instance.m_levelId == particlePair.levelId)
                {
                    Instantiate(particlePair.particle, transform);
                }
            }
            
        }
    }

    [Serializable]
    public class ParticlePair
    {
        public int levelId;
        public GameObject particle;
    }
}