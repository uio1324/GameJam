using UnityEngine;
using UnityEngine.UI;

namespace UI.MainSceneUI
{
    public class ShapeClick : MonoBehaviour
    {
        private PolygonCollider2D m_shape;

        private Button m_button;

        private GameObject m_startPanel;
        // Start is called before the first frame update
        void Start()
        {
            m_shape = GetComponent<PolygonCollider2D>();
            m_button = GetComponent<Button>();
            m_startPanel = GameObject.Find("Canvas/StartPanel");
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !m_startPanel)
            {
                if (m_shape.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                {
                    m_button.onClick.Invoke();
                }
            }
        }
    }
}
