using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IPDialog : MonoBehaviour
{
    //settingNoの取得
    private GameObject wifiDialog;
    public InputField inputCloudHost;
    public InputField inputMecHost;
    private SqliteDatabase sqlDB;
    private string cloudHost;
    private string mecHost;
    private string query;
    void Start()
    {
        sqlDB = new SqliteDatabase("device_db.db");

        string selectQuery = "select * from ServerHostSetting where SettingNo = 1";

        DataTable dataTable = sqlDB.ExecuteQuery(selectQuery);
        foreach (DataRow dr in dataTable.Rows)
        {
            cloudHost = dr["CloudHost"].ToString();
            mecHost = dr["MecHost"].ToString();
        }

        inputCloudHost.text = cloudHost;
        inputMecHost.text = mecHost;
    }

    public void OpenDialog()
    {
        sqlDB = new SqliteDatabase("device_db.db");

        string selectQuery = "select * from ServerHostSetting where SettingNo = 1";

        DataTable dataTable = sqlDB.ExecuteQuery(selectQuery);
        foreach (DataRow dr in dataTable.Rows)
        {
            cloudHost = dr["CloudHost"].ToString();
            mecHost = dr["MecHost"].ToString();
        }

        inputCloudHost.text = cloudHost;
        inputMecHost.text = mecHost;
    }

    public void ChangeIPAdress()
    {
        query = "update ServerHostSetting set CloudHost = '" + inputCloudHost.text + "', MecHost= '" + inputMecHost.text + "' where SettingNo = 1";
        sqlDB.ExecuteNonQuery(query);

        query = "update ServerHostSetting set CloudHost = '" + inputCloudHost.text + "', MecHost= '" + inputCloudHost.text + "' where SettingNo = 2";
        sqlDB.ExecuteNonQuery(query);

        query = "update ServerHostSetting set CloudHost = '" + inputMecHost.text + "', MecHost= '" + inputMecHost.text + "' where SettingNo = 3";
        sqlDB.ExecuteNonQuery(query);
    }
}
