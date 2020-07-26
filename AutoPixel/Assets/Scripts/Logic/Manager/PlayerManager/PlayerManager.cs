using System.Collections;
using UnityEngine;

namespace Logic.Manager.PlayerManager
{
    [ManagerDefine(100, false)]
    public class PlayerManager : Manager<PlayerManager>, IManager
    {
        public bool m_isControllingStone;
        public float m_maxPulseFrequency = 6;
        public float m_minPulseFrequency = 3;

        public Transform m_playerTransform; // todo rename
        public Transform m_player
        {
            get { return m_playerTransform; }
            set {
                m_playerTransform = value;
                m_playerScript = m_playerTransform.gameObject.GetComponent<Logic.Temp.TempPlayerController>();
            }
        }

        protected Logic.Temp.TempPlayerController m_playerScript; // todo rename

        public PlayerManager()
        {
            m_isControllingStone = false;
        }
        
        public IEnumerator PreInit()
        {
            yield return null;
        }

        public Logic.Temp.TempPlayerController GetPlayerScript()
        {
            return m_playerScript;
        }
    }
}