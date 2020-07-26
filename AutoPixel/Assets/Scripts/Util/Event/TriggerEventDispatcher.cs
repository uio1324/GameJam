using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Util.Event
{
	/// <summary>
	/// 是一个触发器事件派发器，如果将许多触发器挂载在角色下，在角色的OnTriggerEnter2D中进行处理的话会没法知道到底是哪个触发器触发
	/// 所以需要将触发器事件派发器放至到挂载在角色下的触发器对象上，然后指向角色的回调函数，可以实现对指定触发器的事件进行响应
	/// </summary>
	public class TriggerEventDispatcher : MonoBehaviour
	{
		public Event OnTriggerEnter2DEvent;
		public Event OnTriggerStay2DEvent;
		public Event OnTriggerExit2DEvent;
		public string Layer;

		private int m_layer;

		private void Awake()
		{
			m_layer = LayerMask.NameToLayer(Layer);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.layer == m_layer)
			{
				OnTriggerEnter2DEvent.Invoke(other);
			}
			
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if (other.gameObject.layer == m_layer)
			{
				OnTriggerStay2DEvent.Invoke(other);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.gameObject.layer == m_layer)
			{
				OnTriggerExit2DEvent.Invoke(other);
			}
		}
	}
}
