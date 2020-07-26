using UnityEngine;

namespace MapEditor
{
	public class ViewController : MonoBehaviour
	{
		[Range(-1f, 1f)]
		public float ViewSpeed = 1;

		public float ScrollSpeed = 3;
		private Vector3 m_lastCursorPos;

		private void Start()
		{
			m_lastCursorPos = Vector3.zero;
		}

		// Update is called once per frame
		void Update () {
			
			if (Input.GetMouseButton(1))
			{
				var curMousePos = Input.mousePosition;
				if (m_lastCursorPos == default(Vector3))
				{
					m_lastCursorPos = curMousePos;
				}
				
				var deltaPos = (curMousePos - m_lastCursorPos) * ViewSpeed;
				deltaPos.z = 0;
				transform.position += deltaPos;
				m_lastCursorPos = curMousePos;
			}
			else
			{
				m_lastCursorPos = default(Vector3);
			}

			var scrollDelta = Input.mouseScrollDelta.y;
			if (Mathf.Abs(scrollDelta) > 0)
			{
				transform.position += ScrollSpeed * scrollDelta * Vector3.up;
			}
		}
	}
}
