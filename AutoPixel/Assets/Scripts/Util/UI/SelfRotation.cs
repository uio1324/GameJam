using System;
using UnityEngine;

namespace Util.UI
{
    public class SelfRotation : MonoBehaviour
    {
        public float speed;

        private void Update()
        {
            transform.Rotate(Vector3.forward, Time.deltaTime * speed);
        }
    }
}