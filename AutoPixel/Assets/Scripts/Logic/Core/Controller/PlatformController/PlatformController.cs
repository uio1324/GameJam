using System;
using UnityEngine;

namespace Logic.Core.Controller.PlatformController
{
    public class PlatformController : MonoBehaviour
    {
        public Transform Target;
        public float Velocity;
        public float Duration;
        public Rigidbody2D Rigidbody2D;
        public Collider2D Collider2D;

        private float m_movingTimer;
        private Vector2 m_direction;
        public void SetTarget(Transform target)
        {
            Target = target;
            m_direction = (target.position - transform.position).normalized;
            m_movingTimer = 0;
        }

        private void FixedUpdate()
        {
            m_movingTimer += Time.fixedDeltaTime;

            if (Target != null)
            {
                var deltaPos = (Vector3) m_direction * Velocity;
                transform.position += deltaPos;
                //Rigidbody2D.velocity = m_direction * Velocity;
            }
            else
            {
                var deltaPos = (Vector3) m_direction * (Velocity * 0.3f);
                transform.position += deltaPos;
                //Rigidbody2D.velocity = m_direction * (Velocity * 0.1f);
            }

            if (m_movingTimer >= Duration)
            {
                Target = null;
            }
        }
    }
}