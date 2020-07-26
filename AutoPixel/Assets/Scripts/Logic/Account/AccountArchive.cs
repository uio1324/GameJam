using UnityEngine;
using System.Collections.Generic;

//存档类
public class ArchiveInfo
{
    public int level;               //关卡点
    public List<int> archiveList;   //关卡存档列表
    public ArchiveInfo(int mlevel, List<int> marchiveList)
    {
        level = mlevel;
        archiveList = marchiveList;
    }
    public string ArchiveToString()
    {
        return JsonUtility.ToJson(this);
    }
}

//存档控制类  提供增加存档点、清空存档列表、获取存档信息的功能
public class ArchiveController
{
    //存档点相关  利用jason->class相互转换进行username_archive->ArchiveInfo维护
    public void AddArchivePoint(string usernameText, int archiveValue)
    {
        string archiveKey = usernameText + "_archive";
        if (PlayerPrefs.HasKey(archiveKey))
        {
            ArchiveInfo archiveInfo = JsonUtility.FromJson<ArchiveInfo>(PlayerPrefs.GetString(archiveKey));
            archiveInfo.archiveList.Add(archiveValue);
            PlayerPrefs.SetString(archiveKey, archiveInfo.ArchiveToString());
        }
    }

    public void ClearAchiveList(string usernameText)
    {
        string archiveKey = usernameText + "_archive";
        if (PlayerPrefs.HasKey(archiveKey))
        {
            ArchiveInfo archiveInfo = JsonUtility.FromJson<ArchiveInfo>(PlayerPrefs.GetString(archiveKey));
            archiveInfo.level++;
            archiveInfo.archiveList.Clear(); //清空存档列表
            PlayerPrefs.SetString(archiveKey, archiveInfo.ArchiveToString());
        }

    }

    public ArchiveInfo GetArchiveInfo(string usernameText)
    {
        List<int> archiveList = new List<int>();
        ArchiveInfo archiveInfo = new ArchiveInfo(0, archiveList);
        string archiveKey = usernameText + "_archive";
        if (PlayerPrefs.HasKey(archiveKey))
        {
            archiveInfo = JsonUtility.FromJson<ArchiveInfo>(PlayerPrefs.GetString(archiveKey));
        }
        return archiveInfo;
    }

    //直接退出 保存当前用户，下一次直接登录
    public void GameOut(string usernameText)
    {
        PlayerPrefs.SetString("CurrentUserName",usernameText);
    }
    //登录之前读上一次的username，免除登录
    public string GameIn()
    {
        string res = "";
        if(PlayerPrefs.HasKey("CurrentUserName"))
        {
            res = PlayerPrefs.GetString("CurrentUserName");
        }
        return res;
    }
}
