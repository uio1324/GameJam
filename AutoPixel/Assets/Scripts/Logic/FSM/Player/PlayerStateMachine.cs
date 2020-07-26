using System.Collections.Generic;
using Logic.Temp;
using UnityEngine;

namespace Logic.FSM.Player
{
    enum EPlayerStateLayer
    {
        Action,
        Camera,
        Light
    }

    enum EPlayerActionState
    {
        Idle,
        Walk,
        Jump,
        Fall,
        Climb,
        ClimbJump,
        Control,
        Dead
    }

    enum EPlayerCameraState
    {
        Idle,
        Zoom
    }

    enum EPlayerLightState
    {
        Idle,
        Move
    }

    enum EPlayerEvent
    {
        Jump,
        Control,
        Reset,
        Die
    }
    
    public class PlayerActionLayer : StateMachineLayer
    {
        public IStateObject m_idleState;
        public IStateObject m_walkState;
        public IStateObject m_jumpState;
        public IStateObject m_fallState;
        public IStateObject m_climbState;
        public IStateObject m_climbJumpState;
        public IStateObject m_controlState;
        public IStateObject m_deadState;
    }

    public class PlayerCameraLayer : StateMachineLayer
    {
        public IStateObject m_idleState;
        public IStateObject m_zoomState;
    }

    public class PlayerLightLayer : StateMachineLayer
    {
        public IStateObject m_idleState;
        public IStateObject m_moveState;
    }

    public class PlayerStateMachine : StateMachine
    {
        public PlayerActionLayer m_actionLayer;
        public PlayerCameraLayer m_cameraLayer;
        public PlayerLightLayer m_lightLayer;

        // Action layer.
        private readonly static IStateObject m_actionIdleState = new PlayerActionIdleState();
        private readonly static IStateObject m_actionWalkState = new PlayerActionWalkState();
        private readonly static IStateObject m_actionJumpState = new PlayerActionJumpState();
        private readonly static IStateObject m_actionFallState = new PlayerActionFallState();
        private readonly static IStateObject m_actionClimbState = new PlayerActionClimbState();
        private readonly static IStateObject m_actionClimbJumpState = new PlayerActionClimbJumpState();
        private readonly static IStateObject m_actionControlState = new PlayerActionControlState();
        private readonly static IStateObject m_actionDeadState = new PlayerActionDeadState();

        // Camera layer.
        private readonly static IStateObject m_cameraIdleState = new PlayerCameraIdleState();
        private readonly static IStateObject m_cameraZoomState = new PlayerCameraZoomState();

        // Light layer.
        private readonly static IStateObject m_lightIdleState = new PlayerLightIdleState();
        private readonly static IStateObject m_lightMoveState = new PlayerLightMoveState();

        // Action layer BEGIN
        public readonly float m_jumpTime = 0.1f; // configurable in second
        public readonly float m_jumpDelayTime = 0.01f; // configurable in second
        public readonly float m_climbJumpTime = 0.5f; // configurable in second
        public readonly float m_reClimbCoolDownTime = 0.5f; // configurable in second
        public bool m_isDead;
        public bool m_isHeadingLeft;
        public Rigidbody2D m_ctrlMapItemCombinerRigidbodyCache;
        public Vector3 m_climbStateLocalSpacePositionCache;
        public float m_gravityScaleCache;
        public int m_interactiveCoolDownTimer; // in millsecond
        public bool m_isDelayJump;
        public float m_delayJumpXInput;
        public float m_jumpTimer; // in second
        public float m_reClimbCoolDownTimer; // in second
        public float m_climbJumpTimer; // in second
        // Action layer END

        // Camera layer BEGIN
        public readonly float m_zoomInOutTime = 0.5f; // configurable
        public readonly float m_zoomPow = 0.5f; // configurable
        public bool m_isInAirFromJump;
        public bool m_isInAirFromJumpCache;
        public float m_cameraSizeZoomTo;
        public float m_zoomTime;
        public float m_zoomLerpFactor;
        public float m_zoomCurveInitValue;
        public float m_cameraSizeCache;
        // Camere layer END

        // Light layer BEGIN
        public readonly float m_lightMoveTime = 0.3f; // configurable
        public float m_lightMoveTimer;
        public Vector3 m_lightInitLocalPos;
        public Vector3 m_lightMoveTarget;
        public bool m_isInControlStateCache;
        public Transform m_lightInitParentTransform;
        // Light layer END

        public override void PreInit(TempPlayerController InOwner, StateMachineLayerInitializer layerInitializer)
        {
            layerInitializer.SetLayer((int)EPlayerStateLayer.Action, new PlayerActionLayer())
                .SetLayer((int)EPlayerStateLayer.Camera, new PlayerCameraLayer())
                .SetLayer((int)EPlayerStateLayer.Light, new PlayerLightLayer());

            base.PreInit(InOwner, layerInitializer);

            // Override layer states.
            m_actionLayer = this.GetLayer((int)EPlayerStateLayer.Action) as PlayerActionLayer;
            m_actionLayer.m_idleState = m_actionIdleState;
            m_actionLayer.m_walkState = m_actionWalkState;
            m_actionLayer.m_jumpState = m_actionJumpState;
            m_actionLayer.m_fallState = m_actionFallState;
            m_actionLayer.m_climbState = m_actionClimbState;
            m_actionLayer.m_climbJumpState = m_actionClimbJumpState;
            m_actionLayer.m_controlState = m_actionControlState;
            m_actionLayer.m_deadState = m_actionDeadState;
            
            m_cameraLayer = this.GetLayer((int)EPlayerStateLayer.Camera) as PlayerCameraLayer;
            m_cameraLayer.m_idleState = m_cameraIdleState;
            m_cameraLayer.m_zoomState = m_cameraZoomState;

            m_lightLayer = this.GetLayer((int)EPlayerStateLayer.Light) as PlayerLightLayer;
            m_lightLayer.m_idleState = m_lightIdleState;
            m_lightLayer.m_moveState = m_lightMoveState;
        }

        public override void ReInitLayers()
        {
            base.ReInitLayers();

            this.TriggerEvent((int)EPlayerEvent.Reset);

            m_actionLayer.m_isEnabled = true;
            m_actionLayer.m_currentState = m_actionLayer.m_idleState;

            m_cameraLayer.m_isEnabled = true;
            m_cameraLayer.m_currentState = m_cameraLayer.m_idleState;

            m_lightLayer.m_isEnabled = true;
            m_lightLayer.m_currentState = m_lightLayer.m_idleState;

            // Init state machine variables.
            m_isHeadingLeft = true;
            m_zoomLerpFactor = 0.0f;
            m_isInAirFromJumpCache = false;
            m_isInControlStateCache = false;
            m_lightMoveTimer = 0.0f;
            m_interactiveCoolDownTimer = 0;
            m_jumpTimer = 0.0f;
            m_reClimbCoolDownTimer = 0.0f;
            m_climbJumpTimer = 0.0f;
        }

        public override void Update()
        {
            base.Update();

            m_lightMoveTimer -= Time.deltaTime;
            m_interactiveCoolDownTimer -= (int)(Time.deltaTime * 1000);
            m_jumpTimer -= Time.deltaTime;
            m_reClimbCoolDownTimer -= Time.deltaTime;
            m_climbJumpTimer -= Time.deltaTime;
        }
    }
}
