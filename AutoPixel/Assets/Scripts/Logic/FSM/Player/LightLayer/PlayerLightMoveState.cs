using Logic.Temp;
using Logic.Map.LevelMap.MapItem.MapItem;
using UnityEngine;

namespace Logic.FSM.Player
{
    public class PlayerLightMoveState : IStateObject
    {
        public int GetID()
        {
            return (int)EPlayerLightState.Move;
        }

        public void OnEnter(StateMachine FSM, IStateObject stateFrom)
        {
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            playerFSM.m_lightInitLocalPos = player.LightGameObject.transform.localPosition;  

            playerFSM.m_lightInitParentTransform = player.LightGameObject.transform.parent;
            player.LightGameObject.transform.parent = player.transform;
            player.LightGameObject.transform.localPosition = Vector3.zero;
        }

        public void OnExit(StateMachine FSM, IStateObject stateTo)
        {
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            player.LightGameObject.transform.parent = playerFSM.m_lightInitParentTransform;
            player.LightGameObject.transform.localPosition = Vector3.zero;
        }

        public IStateObject OnUpdate(StateMachine FSM)
        {
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;
            Transform lightTransform = player.LightGameObject.transform;

            bool isControlStateChanged = playerFSM.m_isInControlStateCache != (FSM.GetCurrentState((int)EPlayerStateLayer.Action) == playerFSM.m_actionLayer.m_controlState);
            if (isControlStateChanged)
            {
                playerFSM.m_isInControlStateCache = !playerFSM.m_isInControlStateCache;
                
                playerFSM.m_lightMoveTimer = playerFSM.m_lightMoveTime;
            }

            if (playerFSM.m_isInControlStateCache)
            {
                playerFSM.m_lightMoveTarget = (player.GetCurrMapItemCombiner().HostedItem as MapItemCombiner).GetRuneStonePosition();
            }
            else
            {
                playerFSM.m_lightMoveTarget = playerFSM.m_lightInitParentTransform.TransformPoint(playerFSM.m_lightInitLocalPos);
            }

            Vector3 lightToTargetVector = playerFSM.m_lightMoveTarget - lightTransform.position;
            lightToTargetVector.z = 0.0f;
            lightTransform.position += lightToTargetVector * Time.deltaTime / playerFSM.m_lightMoveTimer;

            if (playerFSM.m_lightMoveTimer < 0.0f)
            {
                lightTransform.position = playerFSM.m_lightMoveTarget;
                if (playerFSM.m_isInControlStateCache == false)
                {
                    return playerFSM.m_lightLayer.m_idleState;
                }
            }
            return this;
        }

        public IStateObject OnTriggerEvent(StateMachine FSM, int eventID)
        {
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (eventID == (int)EPlayerEvent.Reset)
            {
                return playerFSM.m_lightLayer.m_idleState;
            }
            else if (eventID == (int)EPlayerEvent.Die)
            {
                return playerFSM.m_lightLayer.m_idleState;
            }
            return this;
        }

        public override string ToString()
        {
            return "MoveState";
        }
    }
}