using System.Collections;
using UnityEngine;

namespace Logic.Core.Scenes
{
    [SceneDefine("MainScene")]
    public sealed class MainScene : SceneLogic
    {
        public override IEnumerator OnEnter()
        {
            yield return null;
        }
    }
}