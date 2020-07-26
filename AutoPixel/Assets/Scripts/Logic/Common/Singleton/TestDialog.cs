using UnityEngine;
using System.Collections;

//GameDialog测试脚本
public class TestDialog : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        GameDialogUse.Instance.UseTarget("Hello World!",()=> { Debug.Log("Test"); });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
