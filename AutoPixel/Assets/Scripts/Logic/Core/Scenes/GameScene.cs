using System.Collections;
using Logic.Manager.EventMgr;
using Logic.Manager.MapManager;
using Logic.Manager.AudioMgr;
using Logic.Map.LevelMap;
using UI.CommonUI;
using UnityEngine;
using EventType = Logic.Manager.EventMgr.EventType;

namespace Logic.Core.Scenes
{
    [SceneDefine("GameScene")]
    public sealed class GameScene : SceneLogic
    {
        public override IEnumerator OnEnter()
        {
            SceneUi = GameObject.Find("Canvas").GetComponentInChildren<SceneUiBase>();

            AudioMgr.Instance.PlayBG(MapLogic.m_instance.levelData.BGMusic, 0f);

            var start = 0f;
            while(start < 1f)
            {
                AudioMgr.Instance.SetBgVolume(start);
                start += Time.deltaTime;
                yield return null;
            }
            AudioMgr.Instance.SetBgVolume(1);

            yield return null;
        }

        public override IEnumerator BeforeEnter()
        {
            yield return MapLogic.m_instance.PreInit();
            MapManager.Instance.LoadLevel(GameRoot.m_instance.m_levelId);
            EventMgr.Instance.Dispatch(EventType.BeginScroll);
            yield return null;
        }

        public override IEnumerator OnExit()
        {
            yield return null;
        }

        public override IEnumerator BeforeExit()
        {
            yield return null;
        }
    }
}