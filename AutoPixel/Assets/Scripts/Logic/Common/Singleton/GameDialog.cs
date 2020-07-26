using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;


public class GameDialog : MonoBehaviour
{
    public Text showTxt;
    public Button leftButton;
    public Button rightButton;

    public void DlgController(string msg, UnityAction action)
    {
        showTxt.text = msg;
        leftButton.onClick.AddListener(action);
        rightButton.onClick.AddListener(() => { Destroy(this.gameObject);});
    }  
}