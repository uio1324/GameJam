using System.Collections.Generic;
using UnityEngine;

namespace Logic.FSM.Player
{
    public class PlayerCameraZoomState : IStateObject
    {
        public int GetID()
        {
            return (int)EPlayerCameraState.Zoom;
        }

        public void OnEnter(StateMachine FSM, IStateObject stateFrom)
        {
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            playerFSM.m_cameraSizeCache = Camera.main.orthographicSize;
            playerFSM.m_cameraSizeZoomTo = Camera.main.orthographicSize * 1.05f;
        }

        public void OnExit(StateMachine FSM, IStateObject stateTo)
        {
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            Camera.main.orthographicSize = playerFSM.m_cameraSizeCache;
        }

        public IStateObject OnUpdate(StateMachine FSM)
        {
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            float zoomTimeScalar = 1.0f / playerFSM.m_zoomInOutTime;
            float zoomFuncSign;

            if (playerFSM.m_isInAirFromJump != playerFSM.m_isInAirFromJumpCache)
            {
                playerFSM.m_zoomTime = 0.0f;

                playerFSM.m_zoomCurveInitValue = playerFSM.m_zoomLerpFactor;
                playerFSM.m_isInAirFromJumpCache = playerFSM.m_isInAirFromJump;
            }

            if (playerFSM.m_isInAirFromJump)
            {
                zoomFuncSign = 1.0f;
            }
            else
            {
                zoomFuncSign = -1.0f;
            }
            playerFSM.m_zoomTime = Mathf.Clamp(playerFSM.m_zoomTime + Time.deltaTime, 0.0f, playerFSM.m_zoomInOutTime);

            playerFSM.m_zoomLerpFactor = Mathf.Clamp01(zoomFuncSign * Mathf.Pow(playerFSM.m_zoomTime * zoomTimeScalar, playerFSM.m_zoomPow) + playerFSM.m_zoomCurveInitValue);

            Camera.main.orthographicSize = Mathf.Lerp(playerFSM.m_cameraSizeCache, playerFSM.m_cameraSizeZoomTo, playerFSM.m_zoomLerpFactor);

            if (playerFSM.m_isInAirFromJump == false && playerFSM.m_zoomTime >= playerFSM.m_zoomInOutTime - float.Epsilon)
            {
                return playerFSM.m_cameraLayer.m_idleState;
            }
            else
            {
                return this;
            }
        }

        public IStateObject OnTriggerEvent(StateMachine FSM, int eventID)
        {
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (eventID == (int)EPlayerEvent.Reset)
            {
                return playerFSM.m_cameraLayer.m_idleState;
            }
            else if (eventID == (int)EPlayerEvent.Die)
            {
                return playerFSM.m_cameraLayer.m_idleState;
            }
            return this;
        }

        public override string ToString()
        {
            return "ZoomState";
        }
    }
}