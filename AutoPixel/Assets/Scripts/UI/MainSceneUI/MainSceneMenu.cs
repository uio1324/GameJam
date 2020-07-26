using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Core;
using Logic.Core.Scenes;
using Logic.Manager.AudioMgr;
using Logic.Manager.DataTableMgr;
using Logic.Manager.ProgressManager;
using Logic.Manager.SceneMgr;
using UI.CommonUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.MainSceneUI
{
    public class MainSceneMenu : SceneUiBase
    {
        public List<Button> enterLevelButton;
        public Image mask;
        private int m_curLevel;

        private void Awake()
        {
            InitUi();
            Instance = this;
        }

        private void Start()
        {
            
        }

        public void InitUi()
        {
            if(ProgressMgr.Instance.achievedTeachingLevel)
            {
                mask.gameObject.SetActive(false);
            }

            AudioMgr.Instance.PlayBG(AudioDefine.MainSceneBgm, 1f);
            m_curLevel = 0;
            if (PlayerPrefs.HasKey("progress"))
            {
                m_curLevel = PlayerPrefs.GetInt("progress");
            }
            var type = GetType();
            for (int i = 0; i < enterLevelButton.Count; i++)
            {
                if (!enterLevelButton[i])
                {
                    continue;
                }
                var method = type.GetMethod("OnEnterLevel" + i + "Click");
                if (method == null)
                {
                    continue;
                }

                var cb = (UnityAction) Delegate.CreateDelegate(typeof(UnityAction), this, method);
                enterLevelButton[i].onClick.AddListener(cb);

                var achievedTeaching = ProgressMgr.Instance.achievedTeachingLevel;
                if (i > 0 && i <4)
                {
                    enterLevelButton[i].interactable = false;
                    var _lock = enterLevelButton[i].transform.Find("Lock");
                    if (m_curLevel + 1 >= i && achievedTeaching)
                    {
                        _lock.gameObject.SetActive(false);
                    }
                    else
                    {
                        _lock.gameObject.SetActive(true);
                        enterLevelButton[i].GetComponent<Image>().color = Color.gray;
                    }
                }
            }
        }
        
        public void OnEnterLevel1Click()
        {
            AudioMgr.Instance.Play(AudioDefine.CommonClick);
#if UNITY_EDITOR
            GameRoot.m_instance.m_levelId = DataTableMgr.DataTableMgrDefine.LEVEL_ID_BASE + 1;
            GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(GameScene)));
#else
            //if (m_curLevel + 1 >= 1 && ProgressMgr.Instance.achievedTeachingLevel)
            //{
                GameRoot.m_instance.m_levelId = DataTableMgr.DataTableMgrDefine.LEVEL_ID_BASE + 1;
                GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(GameScene)));
            //}
#endif
        }

        public void OnEnterLevel2Click()
        {
            AudioMgr.Instance.Play(AudioDefine.CommonClick);
#if UNITY_EDITOR
            GameRoot.m_instance.m_levelId = DataTableMgr.DataTableMgrDefine.LEVEL_ID_BASE + 2;
            GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(GameScene)));
#else
            //if (m_curLevel + 1 >= 2)
            //{
                GameRoot.m_instance.m_levelId = DataTableMgr.DataTableMgrDefine.LEVEL_ID_BASE + 2;
                GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(GameScene)));
            //}
#endif
        }

        public void OnEnterLevel3Click()
        {
            AudioMgr.Instance.Play(AudioDefine.CommonClick);
#if UNITY_EDITOR
            GameRoot.m_instance.m_levelId = DataTableMgr.DataTableMgrDefine.LEVEL_ID_BASE + 3;
            GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(GameScene)));
#else
            //if (m_curLevel + 1 >= 3)
            //{
                GameRoot.m_instance.m_levelId = DataTableMgr.DataTableMgrDefine.LEVEL_ID_BASE + 3;
                GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(GameScene)));
            //}
#endif
        }

        public void OnEnterLevel0Click()
        {
            AudioMgr.Instance.Play(AudioDefine.CommonClick);
            GameRoot.m_instance.m_levelId = DataTableMgr.DataTableMgrDefine.LEVEL_ID_BASE;
            GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(GameScene)));
        }

        public void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            ProgressMgr.Instance.Reset();
        }
    }
}