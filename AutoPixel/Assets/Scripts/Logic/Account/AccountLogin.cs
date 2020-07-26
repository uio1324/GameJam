using UnityEngine;
using UnityEngine.UI;


public class AccountLogin : MonoBehaviour
{
    public InputField username;
    public InputField passwd;
    public Button loginButton;
    public Button registerButton;
    //public Text logInfo;
    public GameObject registerWindow;  //登录界面
    Toggle seePwd;    //可见密码
    // Use this for initialization
    void Start()
    {
        //seePwd.onValueChanged.AddListener(OnSeePwd);
        loginButton.onClick.AddListener(OnLoginEnd);
        registerButton.onClick.AddListener(OnRegisterEnd);

    }
    //使密码可见
    void OnSeePwd(bool arg0)
    {
        passwd.contentType = arg0 ? InputField.ContentType.Standard : InputField.ContentType.Password;
        passwd.Select();
    }
    //之前用户直接登录
    string GameIn()
    {
        string res = "";
        if (PlayerPrefs.HasKey("CurrentUserName"))
        {
            res = PlayerPrefs.GetString("CurrentUserName");
            PlayerPrefs.SetString("CurrentUserName","");
        }
        return res;
    }
    //登录接口
    void OnLoginEnd()
    {
        if(GameIn()!="")
        {
            //logInfo.text = "登录成功";
            gameObject.SetActive(false);
            return ;
        }
        if (PlayerPrefs.HasKey(username.text) && PlayerPrefs.GetString(username.text) == passwd.text)
        {
            //logInfo.text = "登录成功";
            Debug.Log("登录成功");
            gameObject.SetActive(false);
            //todo 存档信息获取后怎么使用？
        }
        else if (!PlayerPrefs.HasKey(username.text) && username.text != "")
        {
            //logInfo.text = "该用户名不存在";
        }
        else if (PlayerPrefs.HasKey(username.text) && PlayerPrefs.GetString(username.text) != passwd.text)
        {
            //logInfo.text = "密码错误";
        }
        else
        {
            //logInfo.text = "用户名或密码不能为空";
        }
    }
    //跳转注册界面
    void OnRegisterEnd()
    {
        gameObject.SetActive(false);
        registerWindow.SetActive(true);
        registerWindow.GetComponent<AccountRegister>().Init();  //之前的值赋空

    }

    public void Init()
    {
        username.text = "";
        passwd.text = "";
    }
}
