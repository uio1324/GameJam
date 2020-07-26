
namespace Logic.FSM.Player
{
	public class PlayerActionDeadState : IStateObject
    {
		public int GetID()
		{
			return (int)EPlayerActionState.Dead;
		}

		public void OnEnter(StateMachine FSM, IStateObject stateFrom)
        {
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            playerFSM.m_isDead = true;
        }

		public void OnExit(StateMachine FSM, IStateObject stateTo)
		{
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            playerFSM.m_isDead = false;
        }

		public IStateObject OnUpdate(StateMachine FSM)
		{
			return this;
		}

		public IStateObject OnTriggerEvent(StateMachine FSM, int eventID)
		{
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (eventID == (int)EPlayerEvent.Reset)
            {
                return playerFSM.m_actionLayer.m_idleState;
            }
            return this;
		}

        public override string ToString()
        {
            return "DeadState";
        }
    }
}