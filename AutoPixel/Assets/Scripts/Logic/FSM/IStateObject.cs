using Logic.Temp;

namespace Logic.FSM
{
	public interface IStateObject
    {
		int GetID();

		void OnEnter(StateMachine FSM, IStateObject StateFrom);

		void OnExit(StateMachine FSM, IStateObject StateTo);

		IStateObject OnUpdate(StateMachine FSM);

		IStateObject OnTriggerEvent(StateMachine FSM, int EventID);
    }
}