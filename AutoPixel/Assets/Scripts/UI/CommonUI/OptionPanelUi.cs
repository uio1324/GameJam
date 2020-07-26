using System;
using System.Collections.Generic;
using Logic.Core;
using Logic.Core.Scenes;
using Logic.Manager.SceneMgr;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CommonUI
{
    public class OptionPanelUi : MonoBehaviour
    {
        public Button Template;
        public Text Title;

        public static OptionPanelUi s_unique;
        public void FillOptions(List<UiOptionPair> optionPairs, string panelTitle)
        {
            if (s_unique)
            {
                s_unique.ClosePanel();
            }

            s_unique = this;
            
            Title.text = panelTitle;
            var parent = transform.Find("Content");

            foreach (var uiOptionPair in optionPairs)
            {
                uiOptionPair.Callback += ClosePanel;
                var button = Instantiate(Template, parent, false);
                button.gameObject.SetActive(true);
                button.transform.Find("Text").GetComponent<Text>().text = uiOptionPair.Title;
                button.onClick.AddListener(uiOptionPair.Callback);
            }
        }

        private void ClosePanel()
        {
            Destroy(gameObject);
        }
    }
}