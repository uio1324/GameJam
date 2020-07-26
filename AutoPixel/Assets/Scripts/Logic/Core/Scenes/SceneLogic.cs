using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UI.CommonUI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Logic.Core.Scenes
{
    public abstract class SceneLogic
    {
        public SceneUiBase SceneUi
        {
            get;
            protected set;
        }
        public SceneLogic()
        {
            m_sceneName = ExtractSceneNameByType(GetType());
        }

        public readonly string m_sceneName;

        /// <summary>
        /// 退出场景之前调用，建议在此处做资源的卸载
        /// </summary>
        public virtual IEnumerator BeforeExit()
        {
            yield return null;
        }
        /// <summary>
        /// 退出场景，建议在此处做Prefab、GO的销毁
        /// </summary>
        public virtual IEnumerator OnExit()
        {
            yield return null;
        }
        /// <summary>
        /// 进入场景之前调用，建议在此处做资源的加载
        /// </summary>
        public virtual IEnumerator BeforeEnter()
        {
            yield return null;
        }
        /// <summary>
        /// 进入场景，建议在此处做Prefab、GO的实例化
        /// </summary>
        public virtual IEnumerator OnEnter()
        {
            yield return null;
        }

        public static SceneLogic InstantiateSceneLogicByName(string sceneName)
        {
            var asm = Assembly.GetAssembly(typeof(SceneDefineAttribute));
            var types = asm.GetExportedTypes();
            var type = types.Where(t =>
            {
                return t.GetCustomAttributes(typeof(SceneDefineAttribute), true).Any(define =>
                    ((SceneDefineAttribute) define).m_sceneName == sceneName);
            }).FirstOrDefault();
            if (type != null)
            {
                return (SceneLogic)Activator.CreateInstance(type);
            }

            return null;
        }
        
        public static string ExtractSceneNameByType(Type sceneType)
        {
            var attribute = (SceneDefineAttribute)sceneType.GetCustomAttributes(typeof(SceneDefineAttribute), false).FirstOrDefault();
            return attribute?.m_sceneName;
        }
        public IEnumerator LoadScene()
        {
            var async = SceneManager.LoadSceneAsync(m_sceneName);
            yield return new WaitUntil(()=>async.isDone);
        }
    }
}