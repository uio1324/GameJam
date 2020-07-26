using System;
using System.Collections.Generic;
using System.Collections;
using Logic.Core;
using Logic.Core.Scenes;
using Logic.Manager.DataTableMgr;
using Logic.Manager.AudioMgr;
using Logic.Manager.EventMgr;
using Logic.Manager.InputManager;
using Logic.Manager.PlayerManager;
using Logic.Manager.ProgressManager;
using Logic.Manager.SceneMgr;
using Logic.Map.LevelMap;
using UI.CommonUI;
using Logic.Temp;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class GameSceneMenu : SceneUiBase
    {
        public Button ExitButton;

        public Slider m_slider;
        public Image lightLevel;

        public Button leftButton;
        public Button rightButton;

        public Button JumpButton;
        public Button ControlButton;

        public UnityEngine.Experimental.Rendering.Universal.Light2D globalLight;
        public UnityEngine.Experimental.Rendering.Universal.Light2D teachingLight;

        public Button goOnButton;

        protected float horizontalAxis;
        protected float verticalAxis;

        private bool jumpPress;

        private void Awake()
        {
            // Add button event handlers.
            ExitButton.onClick.AddListener(OnExitButtonClick);
            jumpPress = false;
            //JumpButton.onClick.AddListener(OnJumpButtonClick);
            ControlButton.onClick.AddListener(OnControlButtonClick);

            goOnButton.onClick.AddListener((() => goOnButton.interactable = false));

            Instance = this;
        }

        private void Start()
        {
            horizontalAxis = 0.0f;
            verticalAxis = 0.0f;

            if (GameRoot.m_instance.m_levelId == DataTableMgr.DataTableMgrDefine.LEVEL_ID_BASE)
            {
                globalLight.gameObject.SetActive(false);
                teachingLight.gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            // Remove button event handlers.
            ExitButton.onClick.RemoveListener(OnExitButtonClick);

            JumpButton.onClick.RemoveListener(OnJumpButtonClick);
            ControlButton.onClick.RemoveListener(OnControlButtonClick);
        }

        private void Update()
        {
            lightLevel.fillAmount = PlayerManager.Instance.GetPlayerScript().lightDepresser.LightLevel / 100f;
            
            var touches = Input.touches;
            var leftPress = false;
            var rightPress = false;
            if(touches.Length == 0)
            {
                horizontalAxis = 0;
            }
            else
            {
                foreach(var touch in touches)
                {
                    var leftPos = leftButton.transform.position;
                    var leftHalfSize = leftButton.targetGraphic.rectTransform.rect.size / 2;
                    var leftRect = Rect.MinMaxRect(leftPos.x - leftHalfSize.x, leftPos.y - leftHalfSize.y, leftPos.x + leftHalfSize.x, leftPos.y + leftHalfSize.y);
                    if(leftRect.Contains(touch.position))
                    {
                        leftPress = true;
                    }

                    var rightPos = rightButton.transform.position;
                    var rightHalfSize = rightButton.targetGraphic.rectTransform.rect.size / 2;
                    var rightRect = Rect.MinMaxRect(rightPos.x - rightHalfSize.x, rightPos.y - rightHalfSize.y, rightPos.x + rightHalfSize.x, rightPos.y + rightHalfSize.y);
                    if(rightRect.Contains(touch.position))
                    {
                        rightPress = true;
                    }

                    var jumpPos = JumpButton.transform.position;
                    var jumpHalfSize = JumpButton.targetGraphic.rectTransform.sizeDelta / 2;
                    var jumpRect = Rect.MinMaxRect(jumpPos.x - jumpHalfSize.x, jumpPos.y - jumpHalfSize.y, jumpPos.x + jumpHalfSize.x, jumpPos.y + jumpHalfSize.y);
                    if(jumpRect.Contains(touch.position) && !jumpPress)
                    {
                        OnJumpButtonClick();
                        jumpPress = true;
                    }
                    else
                    {
                        jumpPress = false;
                    }
                }

                horizontalAxis = (leftPress ? -1f : 0) + (rightPress ? 1f : 0);
            }
            /*
            if (touches.Length <= 0)
            {
                m_slider.value = 0.5f;
            }

            horizontalAxis = m_slider.value * 2 - 1f;
            if (Mathf.Abs(horizontalAxis) < 0.2f)
            {
                horizontalAxis = 0;
            }
            else
            {
                horizontalAxis *= 10;
            }
            */
            
            InputManager.Instance.ButtonAxis = new Vector2(horizontalAxis, verticalAxis);
        }

        // Button event handlers.
        public void OnExitButtonClick()
        {
            List<UiOptionPair> optionPairs = new List<UiOptionPair>();
            if (ProgressMgr.Instance.HasSavePoint())
            {
                optionPairs.Add(new UiOptionPair
                {
                    Callback = ProgressMgr.Instance.LoadProgress,
                    Title = "返回最近存档点"
                });
            }
            
            optionPairs.Add(new UiOptionPair
            {
                Callback = RestartLevel,
                Title = "重新开始本关"
            });
            
            optionPairs.Add(new UiOptionPair
            {
                Callback = ReturnToMainScene,
                Title = "主界面"
            });
            
            optionPairs.Add(new UiOptionPair
            {
                Callback = Restore,
                Title = "继续"
            });
            ShowOptionPanel(optionPairs, "选 项");
            MapLogic.m_instance.SetAllowScroll(false);
            GameRoot.m_instance.StartCoroutine(FadeOutBgm());
        }

        private IEnumerator FadeOutBgm()
        {
            var start = 1f;
            while (start > 0f)
            {
                AudioMgr.Instance.SetBgVolume(start);
                start -= Time.deltaTime;

                yield return null;
            }
            AudioMgr.Instance.SetBgVolume(0);

            yield return null;
        }

        private IEnumerator FadeInBgm()
        {
            var start = 0f;
            while (start < 1f)
            {
                AudioMgr.Instance.SetBgVolume(start);
                start += Time.deltaTime;

                yield return null;
            }
            AudioMgr.Instance.SetBgVolume(1);

            yield return null;
        }

        private void Restore()
        {
            MapLogic.m_instance.SetAllowScroll(true);
            GameRoot.m_instance.StartCoroutine(FadeInBgm());
        }
        
        private void RestartLevel()
        {
            ProgressMgr.Instance.RestartLevel();
            GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(GameScene)));
        }

        private void ReturnToMainScene()
        {
            ProgressMgr.Instance.RestartLevel();
            GameRoot.m_instance.StartCoroutine(SceneMgr.Instance.SwitchScene(typeof(MainScene)));
            GameRoot.m_instance.StartCoroutine(FadeInBgm());
        }

        public void OnJumpButtonClick()
        {
            PlayerManager.Instance.GetPlayerScript().OnJump();
        }

        public void OnControlButtonClick()
        {
            PlayerManager.Instance.GetPlayerScript().OnControl();
        }
    }
}