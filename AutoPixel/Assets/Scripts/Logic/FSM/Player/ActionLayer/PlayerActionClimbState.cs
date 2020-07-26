using Logic.Manager.InputManager;
using Logic.Temp;
using Logic.Map.LevelMap;
using UnityEngine;

namespace Logic.FSM.Player
{
	public class PlayerActionClimbState : IStateObject
    {
		public int GetID()
		{
			return (int)EPlayerActionState.Climb;
		}

		public void OnEnter(StateMachine FSM, IStateObject stateFrom)
		{
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            playerFSM.m_gravityScaleCache = player.RigidBody2D.gravityScale;

            player.RigidBody2D.velocity = new Vector2(0.0f, 0.0f);
            player.RigidBody2D.gravityScale = 0.0f;

            Vector3 playerToLadderVector = player.TempGetLadder().transform.position - player.transform.position;
            Vector3 ladderUpVector = player.TempGetLadder().transform.up;
            Vector3 ladderLeftOrRightVector = (new Vector3(-ladderUpVector.y, ladderUpVector.x, 0.0f)).normalized;
            float playerToLadderProjLength = Vector3.Dot(playerToLadderVector, ladderLeftOrRightVector);
            if (playerToLadderProjLength <= -float.Epsilon)
            {
                playerToLadderProjLength = -playerToLadderProjLength;
                ladderLeftOrRightVector = -ladderLeftOrRightVector;
            }
            /*if (ladderUpVector.y >= float.Epsilon)
            {
                playerFSM.m_isHeadingLeft = false;
            }
            else
            {
                playerFSM.m_isHeadingLeft = true;
            }*/
             if (Vector3.Dot(playerToLadderVector, Vector3.left) >= float.Epsilon)
             {
                 playerFSM.m_isHeadingLeft = true;
             }
             else
             {
                 playerFSM.m_isHeadingLeft = false;
             }

            player.transform.position += ladderLeftOrRightVector * (playerToLadderProjLength - 0.225f);
            //player.transform.rotation = player.TempGetLadder().transform.rotation;

            playerFSM.m_climbStateLocalSpacePositionCache = player.TempGetLadder().transform.InverseTransformPoint(player.transform.position);
        }

        public void OnExit(StateMachine FSM, IStateObject stateTo)
		{
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            playerFSM.m_reClimbCoolDownTimer = playerFSM.m_reClimbCoolDownTime;

            //player.transform.rotation = Quaternion.identity;
            player.RigidBody2D.gravityScale = playerFSM.m_gravityScaleCache;
        }

		public IStateObject OnUpdate(StateMachine FSM)
        {
            TempPlayerController player = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            player.transform.position = player.TempGetLadder().transform.TransformPoint(playerFSM.m_climbStateLocalSpacePositionCache);
            //player.transform.rotation = player.TempGetLadder().transform.rotation;

            return this;
        }

        public IStateObject OnTriggerEvent(StateMachine FSM, int eventID)
        {
            TempPlayerController playter = FSM.GetOwner();
            PlayerStateMachine playerFSM = FSM as PlayerStateMachine;

            if (eventID == (int)EPlayerEvent.Jump)
            {
                float horizontalSpeed = InputManager.Instance.Axis.x;
                if (horizontalSpeed < -float.Epsilon || float.Epsilon < horizontalSpeed)
                {
                    return playerFSM.m_actionLayer.m_jumpState;
                }
                else
                {
                    float playerRotationZ = playter.transform.rotation.eulerAngles.z;
                    if (playerRotationZ < -float.Epsilon || float.Epsilon < playerRotationZ)
                    {
                        return playerFSM.m_actionLayer.m_jumpState;
                    }
                    else
                    {
                        return playerFSM.m_actionLayer.m_climbJumpState;
                    }
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
            return "ClimbState";
        }
    }
}
