using System.Collections.Generic;
using Logic.Manager.PrefabMgr;
using UnityEngine;

namespace UI.CommonUI
{
    public class SceneUiBase : MonoBehaviour
    {
        public static SceneUiBase Instance;
        public void ShowOptionPanel(List<UiOptionPair> optionPairs, string panelTitle)
        {
            var prefab = PrefabMgr.Instance.GetPrefab(PrefabPathDefine.PREFAB_PATH_UI_OPTION_PANEL);
            var optionPanel = Instantiate(prefab).GetComponent<OptionPanelUi>();
            optionPanel.transform.SetParent(transform, false);
            optionPanel.FillOptions(optionPairs, panelTitle);
        }
    }
}