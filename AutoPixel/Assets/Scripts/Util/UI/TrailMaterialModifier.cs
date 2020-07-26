using System;
using UnityEngine;
using UnityEngine.UI;

namespace Util.UI
{
    public class TrailMaterialModifier : MonoBehaviour
    {
        public float clip;
        private Material m_material;
        private static readonly int ClipThreshold = Shader.PropertyToID("_ClipThreshold");

        private void Start()
        {
            m_material = GetComponent<Image>().material;
        }

        private void Update()
        {
            m_material.SetFloat(ClipThreshold, clip);
        }
        
    }
}