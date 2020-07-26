using System;
using System.Collections;
using Logic.Core;
using Logic.Core.Scenes;
using Logic.Manager.DataTableMgr;
using Logic.Manager.SceneMgr;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Util.UI;

namespace UI.StartPanel
{
    public class StartPanel : MonoBehaviour
    {
        public Button globalButton;
        public PlayableDirector director;

        private void Awake()
        {
            globalButton.onClick.AddListener(OnClickGlobalButton);
        }

        private void OnClickGlobalButton()
        {
            director.Play();
        }
    }
}
