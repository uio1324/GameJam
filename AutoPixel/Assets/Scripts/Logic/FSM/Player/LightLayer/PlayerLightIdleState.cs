using Logic.Temp;
using Logic.Manager.InputManager;
using UnityEngine;

namespace Logic.FSM.Player
{
    public class PlayerLightIdleState : IStateObject
    {
        public int GetID()
        {
            return (int)EPlayerLightState.Idle;
        }

        public void OnEnter(StateMachine FSM, IStateObject stateFrom)
        {}

        public void OnExit(StateMachine FSM, IStateObject stateTo)
        {}

        public IStateObject OnUpdate(StateMachine FSM)
        {
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (FSM.GetCurrentStateID((int)EPlayerStateLayer.Action) == (int)EPlayerActionState.Control)
            {
                return playerFSM.m_lightLayer.m_moveState;
            }
            return this;
        }

        public IStateObject OnTriggerEvent(StateMachine FSM, int eventID)
        {
            return this;
        }

        public override string ToString()
        {
            return "IdleState";
        }
    }
}