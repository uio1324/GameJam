using System.Collections;
using UI.CommonUI;
using UnityEngine;

namespace Logic.Core.Scenes
{
    [SceneDefine("MainScene")]
    public sealed class MainScene : SceneLogic
    {
        public override IEnumerator OnEnter()
        {
            SceneUi = GameObject.Find("Canvas").GetComponentInChildren<SceneUiBase>();
            yield return null;
        }
    }
}