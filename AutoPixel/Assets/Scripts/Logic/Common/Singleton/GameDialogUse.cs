using System;
using UnityEngine.Events;
using Logic.Common.Singleton;
using UnityEngine;


public class GameDialogUse : Singleton <GameDialogUse>
{
    public void UseTarget(string msg, UnityAction action)
    {
        var dialogPrefab = Resources.Load<GameObject>("Prefabs/UI/dialogPrefab");      //进行反序列化
        GameObject instance = GameObject.Instantiate(dialogPrefab);                    //instance = view + controllerSet

        var canvas = GameObject.Find("Canvas");                                        
        instance.transform.SetParent(canvas.transform);                                //将component置于Canvas之下
        instance.GetComponent<RectTransform>().localPosition = Vector3.zero;           //重新置为0,0;

        var component = instance.GetComponent<GameDialog>();                           //component = controllerSet  还有通用的controller
        component.DlgController(msg, action);

        

    }

}
