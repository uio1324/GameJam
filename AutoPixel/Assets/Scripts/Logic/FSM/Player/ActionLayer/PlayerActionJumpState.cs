using Logic.Manager.AudioMgr;
using Logic.Temp;
using Logic.Manager.InputManager;
using UnityEngine;

namespace Logic.FSM.Player
{
	public class PlayerActionJumpState : IStateObject
    {
		public int GetID()
		{
			return (int)EPlayerActionState.Jump;
		}

		public void OnEnter(StateMachine FSM, IStateObject stateFrom)
		{
			TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            playerFSM.m_isInAirFromJump = true;
            playerFSM.m_jumpTimer = playerFSM.m_jumpTime;
            
            AudioMgr.Instance.Play(AudioDefine.Jump);

            if (stateFrom == playerFSM.m_actionLayer.m_climbState)
            {
                player.RigidBody2D.velocity = new Vector2(InputManager.Instance.Axis.x, player.JumpHeight + player.RigidBody2D.velocity.y);
            }
            else // idle/walk/control
            {
                player.RigidBody2D.velocity = new Vector2(0.0f, 0.0f);

                playerFSM.m_isDelayJump = true;
                playerFSM.m_delayJumpXInput = InputManager.Instance.Axis.x;
                playerFSM.m_jumpTimer += playerFSM.m_jumpDelayTime;
                
            }
		}

		public void OnExit(StateMachine FSM, IStateObject stateTo)
		{}

		public IStateObject OnUpdate(StateMachine FSM)
		{
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (playerFSM.m_isDelayJump && playerFSM.m_jumpTimer < playerFSM.m_jumpTime)
            {
                playerFSM.m_isDelayJump = false;

                player.RigidBody2D.velocity = new Vector2(playerFSM.m_delayJumpXInput * player.MoveSpeed * 0.2f, player.JumpHeight);//ÆðÌø¾àÀë
            }
            else if (playerFSM.m_jumpTimer < 0.0f)
            {
                return playerFSM.m_actionLayer.m_fallState;
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
            return "JumpState";
        }
    }
}
