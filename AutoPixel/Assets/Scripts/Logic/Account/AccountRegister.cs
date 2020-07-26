using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

//在Mac OS X平台下，存储在~/Library/Preferences文件夹，名为unity.[company name].[product name].plist。

public class AccountRegister : MonoBehaviour
{
    public InputField username;
    public InputField passwd;
    public InputField configPasswd;
    public Button registerButton;
    //public Text logInfo;
    public GameObject loginWindow;  //登录界面
    Toggle seePwd;    //可见密码
    // Use this for initialization
    void Start()
    {
        //seePwd.onValueChanged.AddListener(OnSeePwd);
        username.onEndEdit.AddListener(OnNameEnd);
        configPasswd.onEndEdit.AddListener(OnPwdEnd);
        registerButton.onClick.AddListener(OnRegiserEnd);
    }

    //使密码可见
    void OnSeePwd(bool arg0)
    {
        passwd.contentType = arg0 ? InputField.ContentType.Standard : InputField.ContentType.Password;
        passwd.Select();
    }
    //当结束编辑名字时，查看是否有已经存在该用户名
    void OnNameEnd(string arg0)
    {
        if (PlayerPrefs.HasKey(username.text))
        {
            //logInfo.text = "该用户名已存在";
        }
    }
    //检查前后两次输入密码是否一致
    void OnPwdEnd(string arg0)
    {
        //Debug.Log("testPasswd");
        if (configPasswd.text != passwd.text)
        {
            //logInfo.text = "两次输入的密码不一致";
        }
    }
    //注册接口
    //两个key-value  username->passwd  username_archive->archiveInfo 
    void OnRegiserEnd()
    {
        //注册过程中，players中不存在这个名字，两次密码输入一致，存入playerprefs，跳转到登录界面
        if (!PlayerPrefs.HasKey(username.text))
        {
            if (passwd.text == configPasswd.text)
            {
                //插入用户表中 
                PlayerPrefs.SetString(username.text, passwd.text);
                //存档点 插入
                List<int> archiveList = new List<int>();
                ArchiveInfo archiveInfo = new ArchiveInfo(0, archiveList);
                PlayerPrefs.SetString(username.text + "_archive", archiveInfo.ArchiveToString());
                gameObject.SetActive(false);   //本界面隐藏
                loginWindow.SetActive(true);   //转到登录界面
                loginWindow.GetComponent<AccountLogin>().Init();  //对之前的内容清空
            }
        }
    }
    public void Init()
    {
        username.text = "";
        passwd.text = "";
        configPasswd.text = "";
    }
}
