using Logic.Manager.AudioMgr;
using Logic.Manager.InputManager;
using Logic.Manager.PlayerManager;
using Logic.Map.LevelMap;
using Logic.Map.LevelMap.MapItem.MapItem;
using Logic.Map.LevelMap.MapItemCommon.Component;
using Logic.Temp;
using UnityEngine;

namespace Logic.FSM.Player
{
	public class PlayerActionControlState : IStateObject
    {
		public int GetID()
		{
			return (int)EPlayerActionState.Control;
		}

		public void OnEnter(StateMachine FSM, IStateObject stateFrom)
		{
			TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;
            
            AudioMgr.Instance.Play(AudioDefine.Control);

            // Reset interactive cool down timer.
            playerFSM.m_interactiveCoolDownTimer = player.GetInteractiveCoolDown();

            playerFSM.m_ctrlMapItemCombinerRigidbodyCache = player.GetCurrMapItemCombinerRigidbody();

            MapItemCombiner combiner = playerFSM.m_ctrlMapItemCombinerRigidbodyCache.gameObject.GetComponent<MapItemCombinerComponent>().HostedItem as MapItemCombiner;
            combiner.SetUnderPlayerControl(true);
		}

		public void OnExit(StateMachine FSM, IStateObject stateTo)
		{
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            MapItemCombiner combiner = playerFSM.m_ctrlMapItemCombinerRigidbodyCache.gameObject.GetComponent<MapItemCombinerComponent>().HostedItem as MapItemCombiner;
            combiner.SetUnderPlayerControl(false);
        }

		public IStateObject OnUpdate(StateMachine FSM)
		{
			TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            // 如果不能交互了，也立刻退出交互状态
            if (!player.GetCanInteractive())
			{
				return playerFSM.m_actionLayer.m_idleState;
			}

			return this;
		}

		public IStateObject OnTriggerEvent(StateMachine FSM, int eventID)
		{
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (eventID == (int)EPlayerEvent.Jump)
            {
                return playerFSM.m_actionLayer.m_jumpState;
            }
            else if (eventID == (int)EPlayerEvent.Control)
            {
                return playerFSM.m_actionLayer.m_idleState;
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
            return "ControlState";
        }
    }
}