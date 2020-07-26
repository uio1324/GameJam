using Logic.Temp;
using Logic.Manager.InputManager;
using UnityEngine;

namespace Logic.FSM.Player
{
	public class PlayerActionFallState : IStateObject
	{
		public int GetID()
		{
			return (int)EPlayerActionState.Fall;
		}

		public void OnEnter(StateMachine FSM, IStateObject stateFrom)
		{
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (stateFrom != playerFSM.m_actionLayer.m_jumpState)
            {
                player.RigidBody2D.velocity = new Vector2(0.0f, player.RigidBody2D.velocity.y);
            }
            // playerFSM.m_isInAirFromJump = true;
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

            float xAxis = InputManager.Instance.Axis.x;           

            if (xAxis < -float.Epsilon || float.Epsilon < xAxis)
            {
                playerFSM.m_isHeadingLeft = xAxis < 0.0f;
            }
            if (player.RigidBody2D.velocity.y <= -float.Epsilon)
            {
                playerFSM.m_isInAirFromJump = false;
            }

            if (player.IsOnGround)
            {
                return playerFSM.m_actionLayer.m_idleState;
            }
            else if (player.CanClimb)
            {
                if (playerFSM.m_reClimbCoolDownTimer < 0.0f)
                {
                    return playerFSM.m_actionLayer.m_climbState;
                }
            }

            player.RigidBody2D.velocity += new Vector2(xAxis * player.MoveSpeed * Time.deltaTime* 5.0f , 0.0f);//下落左右
            if (Mathf.Abs(player.RigidBody2D.velocity.x) > Mathf.Abs(player.MoveSpeed))
            {
                if (player.RigidBody2D.velocity.x > float.Epsilon)
                {
                    player.RigidBody2D.velocity = new Vector2(player.MoveSpeed, player.RigidBody2D.velocity.y);
                }
                else
                {
                    player.RigidBody2D.velocity = new Vector2(-player.MoveSpeed, player.RigidBody2D.velocity.y);
                }
            }

            /*if(xAxis > -float.Epsilon && float.Epsilon < xAxis)
            {
                player.RigidBody2D.velocity = new Vector2(player.MoveSpeed, player.RigidBody2D.velocity.y);
            }
            else
            {
                player.RigidBody2D.velocity = new Vector2(-1 * player.MoveSpeed, player.RigidBody2D.velocity.y);
            }*/



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
			return "FallState";
		}
	}
}
