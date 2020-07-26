using System;
using System.Collections.Generic;

namespace Logic.FSM
{
    public class StateMachineLayerInitializer
    {
        public Dictionary<int, StateMachineLayer> m_layerMap;

        public StateMachineLayerInitializer()
        {
            m_layerMap = new Dictionary<int, StateMachineLayer>();
        }

        public StateMachineLayerInitializer SetLayer(int layerIndex, StateMachineLayer layer)
        {
            m_layerMap.Add(layerIndex, layer);
            return this;
        }
    }
}