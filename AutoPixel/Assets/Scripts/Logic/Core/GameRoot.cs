using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Logic.Common.Singleton;
using Logic.Core.Scenes;
using Logic.Manager;
using Logic.Manager.SceneMgr;
using UnityEngine;

namespace Logic.Core
{
	public class GameRoot : MonoBehaviour
	{
		public static GameRoot m_instance;
		private List<IManager> m_managers;
		private SortedDictionary<uint, Type> m_preInitManager;

		/// <summary>
		/// 获取被T标记的类型
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private Type[] ExtractManagersByAttribute<T>() where T : ManagerDefineAttribute
		{
			var asm = Assembly.GetAssembly(typeof(T));
			var types = asm.GetExportedTypes();

			bool IsTargetAttribute(Attribute[] attributes)
			{
				foreach (var attribute in attributes)
				{
					if (attribute is T)
					{
						return true;
					}
				}

				return false;
			}

			return types.Where(t => IsTargetAttribute((Attribute[])t.GetCustomAttributes(typeof(T), true))).ToArray();
		}

		/// <summary>
		/// 从类型中获取该类型的目标基类
		/// </summary>
		/// <param name="type"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		private Type GetBaseType(Type type, Type targetType)
		{
			var ret = type.BaseType;
			while (ret != null && targetType != null && ret.FullName != null && targetType.FullName != null)
			{
				if (ret.FullName.Contains(targetType.FullName))
				{
					return ret;
				}
				ret = ret.BaseType;
			}

			return null;
		}

		private void PreInitLogicMgr()
		{
			var types = ExtractManagersByAttribute<ManagerDefineAttribute>();
			foreach (var type in types)
			{
				var attr = (ManagerDefineAttribute)type.GetCustomAttributes(typeof(ManagerDefineAttribute), true)[0];
				if (attr.m_needPreInit)
				{
					m_preInitManager.Add(attr.m_priority, type);
				}
			}
			
			StartCoroutine(PreInit());
		}
		
		
		private IEnumerator PreInit()
		{
			foreach (var pair in m_preInitManager)
			{
				if (pair.Value.BaseType == null)
				{
					throw new Exception($"管理器 ： {pair.Value.Name} 未继承自Singleton");
				}

				var property = GetBaseType(pair.Value, typeof(Singleton<>)).GetProperty("Instance");
				if (property == null)
				{
					throw new Exception("未找到字段" + "Instance");
				}
				var manager = property.GetValue(null, null) as IManager;
				if (manager == null)
				{
					throw new Exception($"管理器 ： {pair.Value.Name} 未继承自Manager");
				}
				
				yield return manager.PreInit();
				AddManager(manager);
			}

			yield return SceneMgr.Instance.SwitchScene(typeof(MainScene));
		}

		public void AddManager(IManager manager)
		{
			m_managers.Add(manager);
		}
		private void Awake()
		{
			DontDestroyOnLoad(this);
			m_instance = this;
			m_managers = new List<IManager>();
			m_preInitManager = new SortedDictionary<uint, Type>();
			PreInitLogicMgr();
			Application.targetFrameRate = 60;
		}

		// Update is called once per frame
		void Update ()
		{
			foreach (var m in m_managers)
			{
				m.Update();
			}
		}

		private void OnDestroy()
		{
			foreach (var m in m_managers)
			{
				m.OnDestroy();
			}
		}
	}
}
