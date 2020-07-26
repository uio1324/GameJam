using Logic.Temp;
using UnityEngine;
using System.Collections.Generic;

namespace Logic.FSM
{
    public class StateMachineLayer
    {
        public bool m_isEnabled;
        public IStateObject m_currentState;
    }

    public class StateMachine
    {
        protected TempPlayerController m_owner;

        Dictionary<int, StateMachineLayer> m_layers;

        public StateMachine()
        {
            m_layers = new Dictionary<int, StateMachineLayer>();
        }

        // Pre initialize layers and states.
        public virtual void PreInit(TempPlayerController InOwner, StateMachineLayerInitializer layerInitializer)
        {
            m_owner = InOwner;

            foreach (var keyValuePair in layerInitializer.m_layerMap)
            {
                m_layers.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        // Reset each layer's current state and start running.
        public void Reset()
        {
            this.ReInitLayers();

            foreach (var keyValuePair in m_layers)
            {
                StateMachineLayer layer = keyValuePair.Value;
                if (layer.m_isEnabled)
                {
                    layer.m_currentState.OnEnter(this, layer.m_currentState);
                }
            }
        }

        public virtual void ReInitLayers()
        {}

		public TempPlayerController GetOwner()
		{
			return m_owner;
		}

        protected StateMachineLayer GetLayer(int layerIndex)
        {
            StateMachineLayer layer;
            if (m_layers.TryGetValue(layerIndex, out layer))
            {
                return layer;
            }
            else
            {
                return null;
            }
        }

		public IStateObject GetCurrentState(int layerIndex)
		{
            StateMachineLayer layer;
            if (m_layers.TryGetValue(layerIndex, out layer))
            {
                return layer.m_currentState;
            }
            else
            {
                return null;
            }
		}

		public int GetCurrentStateID(int layerIndex)
		{
            IStateObject state = GetCurrentState(layerIndex);

            if (state != null)
            {
                return state.GetID();
            }
            else
            {
                return -1;
            }
		}

		public virtual void Update()
		{
            foreach (var keyValuePair in m_layers)
            {
                StateMachineLayer layer = keyValuePair.Value;

                if (layer.m_isEnabled)
                {
                    bool hasStateTransited;
                    do
                    {
                        IStateObject newState = layer.m_currentState.OnUpdate(this);

                        hasStateTransited = this.SetState(layer, newState);
                    } while (hasStateTransited);
                }
            }
		}

		public void TriggerEvent(int layerIndex, int eventID)
		{
            IStateObject currentState = GetCurrentState(layerIndex);
            if (currentState != null)
            {
                IStateObject newState = currentState.OnTriggerEvent(this, eventID);

                this.SetState(GetLayer(layerIndex), newState);
            }
		}

        public void TriggerEvent(int eventID)
        {
            foreach (var keyValuePair in m_layers)
            {
                this.TriggerEvent(keyValuePair.Key, eventID);
            }
        }

		protected bool SetState(StateMachineLayer layer, IStateObject newState)
		{
			if (layer.m_currentState != newState)
			{
                layer.m_currentState.OnExit(this, newState);
                newState.OnEnter(this, layer.m_currentState);

                layer.m_currentState = newState;

				return true;
			}
			else
			{
				return false;
			}
		}	
	}
}