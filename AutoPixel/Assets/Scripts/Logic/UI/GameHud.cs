using UnityEngine;
using UnityEngine.UI;

namespace Logic.UI
{
    public class GameHud : MonoBehaviour
    {
        public Image ThrowProgress;

        public void Press(float curTime, float maxTime)
        {
            ThrowProgress.gameObject.SetActive(true);
            ThrowProgress.fillAmount = curTime / maxTime;
        }

        public void Release()
        {
            ThrowProgress.gameObject.SetActive(false);
        }
    }
}