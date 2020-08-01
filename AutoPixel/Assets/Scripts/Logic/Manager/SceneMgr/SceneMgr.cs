using System;
using System.Collections;
using Logic.Core.Scenes;
using UnityEngine.SceneManagement;

namespace Logic.Manager.SceneMgr
{
    public enum SceneMgrState
    {
        Loading, //  加载中
        Normal // 正常状态
    }
    [ManagerDefine(20, true)]
    public sealed class SceneMgr : Manager<SceneMgr>, IManager
    {
        private SceneLogic m_curScene;
        private SceneLogic m_targetScene;
        private SceneMgrState m_curState;
        public SceneMgrState State
        {
            get { return m_curState; }
        }
        public SceneMgr()
        {
            m_curState = SceneMgrState.Normal;
            var curSceneName = SceneManager.GetActiveScene().name;
            m_curScene = SceneLogic.InstantiateSceneLogicByName(curSceneName);
        }
        
        public IEnumerator SwitchScene(Type sceneType)
        {
            if (!sceneType.IsSubclassOf(typeof(SceneLogic)))
            {
                yield break;
            }
            if (m_curScene != null)
            {
                yield return m_curScene.BeforeExit();
                yield return m_curScene.OnExit();
            }
            var scene = (SceneLogic)Activator.CreateInstance(sceneType);
            m_curState = SceneMgrState.Loading;

            yield return scene.LoadScene();

            yield return scene.BeforeEnter();
            yield return scene.OnEnter();
            m_curScene = scene;
            m_curState = SceneMgrState.Normal;
        }

        public string GetCurSceneName()
        {
            return m_curScene.m_sceneName;
        }

        public IEnumerator PreInit()
        {
            yield return null;
        }
    }
}