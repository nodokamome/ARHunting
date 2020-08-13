using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DeviceDB : MonoBehaviour
{
    private string data;
    private SqliteDatabase sqlDB;
    private string query;
    private string CloudHost = "192.168.11.132";         //クラウド接続先のURL
    private string MecHost = "192.168.11.131";          //MEC接続先のURL

    public ToggleGroup toggleGroup;

    void Awake()
    {
        sqlDB = new SqliteDatabase("device_db.db");

        UserDataDB userDataDB = new UserDataDB();
        if (userDataDB.GetUserData("id") == null)
        {
            query = "insert into UserData values('1','','','1','1','100','100','1')";
            sqlDB.ExecuteNonQuery(query);
            query = "insert into ServerHostSetting values('" + 1 + "','" + CloudHost + "','" + MecHost + "')";
            sqlDB.ExecuteNonQuery(query);

            query = "insert into ServerHostSetting values('" + 2 + "','" + CloudHost + "','" + CloudHost + "')";
            sqlDB.ExecuteNonQuery(query);

            userDataDB.SetUserData("id", RandomID());
        }


        Debug.Log(" id: " + userDataDB.GetUserData("id") + "\n name: " + userDataDB.GetUserData("name") + "\n sound: " + userDataDB.GetUserData("sound") + "\n weponNo: " + userDataDB.GetUserData("weponNo") + "\n initialHP: " + userDataDB.GetUserData("initialHP") + "\n currentHP: " + userDataDB.GetUserData("currentHP") + "\n SettingNo : " + userDataDB.GetUserData("SettingNo"));

    }

    public string RandomID()
    {
        string random = "0123456789abcdefghijklmnopqrstuvwxzABCDEFGHIJKLMNOPQRSTUVWXZ";

        string randID = "";
        int legnth = random.Length;
        int r;
        string tmp;

        for (int i = 0; i < 8; i++)
        {
            r = UnityEngine.Random.Range(0, legnth - 1);
            tmp = random.Substring(r, 1);
            randID += tmp;
        }
        return randID;
    }

    public void onChangeSettingNo()
    {
        int NewSettingNo = 1;

        Toggle tgl = toggleGroup.ActiveToggles().FirstOrDefault();
        if (tgl.ToString().Contains("01"))
        {
            NewSettingNo = 1;
        }
        else if (tgl.ToString().Contains("02"))
        {
            NewSettingNo = 2;
        }
        query = "update UserData set SettingNo = '" + NewSettingNo + "' where file = 1";
        sqlDB.ExecuteNonQuery(query);
        Debug.Log("設定変更されました: " + NewSettingNo);

    }

}
