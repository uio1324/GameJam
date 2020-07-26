using Logic.Temp;
using Logic.Manager.InputManager;
using UnityEngine;

namespace Logic.FSM.Player
{
    public class PlayerCameraIdleState : IStateObject
    {
        public int GetID()
        {
            return (int)EPlayerCameraState.Idle;
        }

        public void OnEnter(StateMachine FSM, IStateObject stateFrom)
        {}

        public void OnExit(StateMachine FSM, IStateObject stateTo)
        {}

        public IStateObject OnUpdate(StateMachine FSM)
        {
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (playerFSM.m_isInAirFromJump)
            {
                return playerFSM.m_cameraLayer.m_zoomState;
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