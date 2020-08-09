using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Logic.Core.PlayerController
{
    public class PlayerController : MonoBehaviour
    {
        public float Velocity;
        private Collider2D m_collider2D;
        private Rigidbody2D m_rigidbody2D;
        
        private void Awake()
        {
            m_collider2D = GetComponent<Collider2D>();
            m_rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            transform.rotation = Quaternion.Euler(0, 0, m_angle - 90);
            m_rigidbody2D.velocity = m_direction * Velocity;
        }

        private Vector2 m_direction;
        public void OnMove(InputAction.CallbackContext callbackContext)
        {
            m_direction = callbackContext.ReadValue<Vector2>();
        }

        private bool m_fireTriggering;
        public void OnFire(InputAction.CallbackContext callbackContext)
        {
            m_fireTriggering = callbackContext.phase == InputActionPhase.Performed;
        }

        private int m_angle;
        public void OnLook(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.control.device == Pointer.current || callbackContext.control.device == Mouse.current)
            {
                var position = callbackContext.ReadValue<Vector2>();
                var curCamera = Camera.main;
                if (curCamera == null)
                {
                    Debug.LogWarning("场景相机获取为空");
                    return;
                }
                
                var playerCameraPos = (Vector2) curCamera.WorldToScreenPoint(transform.position);
                m_angle = (int) (Mathf.Rad2Deg * Mathf.Atan2(position.y - playerCameraPos.y, position.x - playerCameraPos.x));
                
            }
            else
            {
                var axis = callbackContext.ReadValue<Vector2>();
                m_angle = (int) (Mathf.Rad2Deg * Mathf.Atan2(axis.y , axis.x));
            }
        }
    }
}