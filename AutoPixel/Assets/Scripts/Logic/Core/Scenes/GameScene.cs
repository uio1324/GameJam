using System.Collections;

namespace Logic.Core.Scenes
{
    [SceneDefine("GameScene")]
    public sealed class GameScene : SceneLogic
    {
        public override IEnumerator OnEnter()
        {
            yield return null;
        }

        public override IEnumerator BeforeEnter()
        {
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