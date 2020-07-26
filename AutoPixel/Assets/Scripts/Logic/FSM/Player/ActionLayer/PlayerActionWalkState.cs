using Logic.Manager.InputManager;
using Logic.Manager.PlayerManager;
using Logic.Map.LevelMap;
using Logic.Temp;
using UnityEngine;

namespace Logic.FSM.Player
{
	public class PlayerActionWalkState : IStateObject
    {
		public int GetID()
		{
			return (int)EPlayerActionState.Walk;
		}

		public void OnEnter(StateMachine FSM, IStateObject stateFrom)
		{}

		public void OnExit(StateMachine FSM, IStateObject stateTo)
		{}

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
                float xAxis = InputManager.Instance.Axis.x;

                if (player.CanClimb)
                {
                    return playerFSM.m_actionLayer.m_climbState;
                }
                else if (-float.Epsilon <= xAxis && xAxis <= float.Epsilon)
                {
                    return playerFSM.m_actionLayer.m_idleState;
                }
                else
                {
                    playerFSM.m_isHeadingLeft = xAxis < 0.0f;

                    player.RigidBody2D.velocity = new Vector2(xAxis * player.MoveSpeed, player.RigidBody2D.velocity.y);

                    return this;
                }
            }
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
				if (playerFSM.m_interactiveCoolDownTimer < player.GetInteractiveCoolDown() && player.GetCanInteractive())
				{
                    return playerFSM.m_actionLayer.m_controlState;
				}
			}
            else if (eventID == (int)EPlayerEvent.Reset)
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
            return "WalkState";
        }
    }
}