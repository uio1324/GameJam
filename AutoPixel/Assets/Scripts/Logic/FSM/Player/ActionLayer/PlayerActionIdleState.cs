using Logic.Temp;
using Logic.Manager.InputManager;
using UnityEngine;

namespace Logic.FSM.Player
{
    public class PlayerActionIdleState : IStateObject
    {
        public int GetID()
        {
            return (int)EPlayerActionState.Idle;
        }

        public void OnEnter(StateMachine FSM, IStateObject stateFrom)
        { }

        public void OnExit(StateMachine FSM, IStateObject stateTo)
        { }

        public IStateObject OnUpdate(StateMachine FSM)
        {
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (player.IsOnGround == false)
            {
                return playerFSM.m_actionLayer.m_fallState;
            }
            else
            {
                Vector2 Axis = InputManager.Instance.Axis;
                //float velocityMag = player.RigidBody2D.velocity.magnitude;

                if (Axis.x < -float.Epsilon || Axis.x > float.Epsilon)// || velocityMag > float.Epsilon)
                {
                    return playerFSM.m_actionLayer.m_walkState;
                }
            }

            return this;
        }

        public IStateObject OnTriggerEvent(StateMachine FSM, int eventID)
        {
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (eventID == (int)EPlayerEvent.Jump)
            {
                return playerFSM.m_actionLayer.m_jumpState;
            }
            else if (eventID == (int)EPlayerEvent.Control)
            {
                if (playerFSM.m_interactiveCoolDownTimer < 0 && player.GetCanInteractive())
                {
                    return playerFSM.m_actionLayer.m_controlState;
                }
            }
            else if (eventID == (int)EPlayerEvent.Die)
            {
                return playerFSM.m_actionLayer.m_deadState;
            }
            return this;
        }

        public override string ToString()
        {
            return "IdleState";
        }
    }
}
