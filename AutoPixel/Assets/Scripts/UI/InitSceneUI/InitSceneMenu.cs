using System;
using Logic.Core;
using Logic.Core.Scenes;
using Logic.Manager.SceneMgr;
using UI.CommonUI;
using UnityEngine;

namespace UI.InitSceneUI
{
    public class InitSceneMenu : SceneUiBase
    {
        private void Start()
        {
            GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(MainScene)));
        }
    }
}