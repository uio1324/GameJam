using Logic.Manager.AudioMgr;
using Logic.Temp;
using UnityEngine;

namespace Logic.FSM.Player
{
    public class PlayerActionClimbJumpState : IStateObject
    {
        public int GetID()
        {
            return (int)EPlayerActionState.ClimbJump;
        }

        public void OnEnter(StateMachine FSM, IStateObject stateFrom)
        {
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;
            
            AudioMgr.Instance.Play(AudioDefine.Jump);

            playerFSM.m_isInAirFromJump = true;
            playerFSM.m_climbJumpTimer = playerFSM.m_climbJumpTime;

            player.RigidBody2D.velocity = new Vector2(0.0f, player.JumpHeight + player.TempGetLadder().GetComponent<Collider2D>().attachedRigidbody.velocity.y);
        }

        public void OnExit(StateMachine FSM, IStateObject stateTo)
        {
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            playerFSM.m_isInAirFromJump = false;
        }

        public IStateObject OnUpdate(StateMachine FSM)
        {
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (player.CanClimb == false)
            {
                return playerFSM.m_actionLayer.m_fallState;
            }
            else if (playerFSM.m_climbJumpTimer < 0.0f)
            {
                return playerFSM.m_actionLayer.m_climbState;
            }
            return this;
        }

        public IStateObject OnTriggerEvent(StateMachine FSM, int eventID)
        {
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (eventID == (int)EPlayerEvent.Reset)
            {
                return playerFSM.m_actionLayer.m_idleState;
            }
            else if (eventID == (int)EPlayerEvent.Die)
            {
                return playerFSM.m_actionLayer.m_deadState;
            }
            return this;
        }

        public override string ToString()
        {
            return "ClimbJumpState";
        }
    }
}