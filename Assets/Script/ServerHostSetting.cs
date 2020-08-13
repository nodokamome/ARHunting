using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ServerHostSetting{
	//Cloudのホストアドレス格納
    private string CloudHost;
	//MECのホストアドレス格納         
    private string MecHost;
	//さくらの（Cloud）ホストアドレス格納         
	private string SakuraCloud;
	//さくらの（MEC）ホストアドレス格納         
	private string SakuraMec;
	//SettingNo
	private string SettingNo;
	//SqlDB
    private SqliteDatabase　sqlDB;
	private string query;
	//TimeOutSec
    public float TimeOutSec = 3f;
	//URL一覧                                                   
	public string cloud_user_insert = "cloud_user_insert.php";
	public string cloud_user_update = "cloud_user_update.php";
	public string cloud_monster_list_send = "cloud_monster_list_send.php";
	public string cloud_data_send = "cloud_data_send.php";
	public string mec_data_sharing = ":8080";
	//public string mec_data_sharing = "test.php";
	public string cloud_user_escape = "cloud_user_escape.php";
	public string cloud_data_receive = "cloud_data_receive.php";

	//SettingNo取得
	public string GetSettingNo(){
		sqlDB = new SqliteDatabase("device_db.db");
        string selectQuery = "select SettingNo from UserData where file=1";
        DataTable dataTable = sqlDB.ExecuteQuery(selectQuery);
        foreach(DataRow dr in dataTable.Rows){
        	SettingNo = dr["SettingNo"].ToString();
        }
		return SettingNo;
	}

	//Cloudのホストアドレス取得
	public string GetCloudHost(){
		sqlDB = new SqliteDatabase("device_db.db");
        string selectQuery = "select CloudHost from ServerHostSetting where SettingNo="+ GetSettingNo();
        DataTable dataTable = sqlDB.ExecuteQuery(selectQuery);
        foreach(DataRow dr in dataTable.Rows){
        	CloudHost = (string)dr["CloudHost"];
        }
		return CloudHost;
	}

	//Cloudのホストアドレス取得
	public string GetUploadHost(){
		sqlDB = new SqliteDatabase("device_db.db");
        string selectQuery = "select CloudHost from ServerHostSetting where SettingNo="+ 1;
        DataTable dataTable = sqlDB.ExecuteQuery(selectQuery);
        foreach(DataRow dr in dataTable.Rows){
        	CloudHost = (string)dr["CloudHost"];
        }
		return CloudHost;
	}

	//MECのホストアドレス取得
	public string GetMecHost(){
		sqlDB = new SqliteDatabase("device_db.db");
        string selectQuery = "select * from ServerHostSetting where SettingNo="+ GetSettingNo();
        DataTable dataTable = sqlDB.ExecuteQuery(selectQuery);
        foreach(DataRow dr in dataTable.Rows){
        	MecHost = (string)dr["MecHost"];
        }

		return MecHost;
	}

	//アクセスURLを取得
	public string GetURL(string script){
		string URL = "";
		switch(script){
			case "StartDirector":
				URL = "http://"+GetCloudHost()+"/"+cloud_user_insert;
				break;
			case "SettingDialog":
				URL = "http://"+GetCloudHost()+"/"+cloud_user_update;
				break;
			case "ScrollViewScript":
				URL = "http://"+GetCloudHost()+"/"+cloud_monster_list_send;
				break;
			case "DLDirector":
				URL = "http://"+GetCloudHost()+"/"+cloud_data_send;
				break;
			case "GameDataSharing":
				URL = "ws://"+GetMecHost()+mec_data_sharing;
				break;
			case "MonsterImage":
				URL = "http://"+GetCloudHost()+"/"+"img/";
				break;
			case "GameResult":
				URL = "http://"+GetCloudHost()+"/"+cloud_user_escape;
				break;
			case "UploadDialog":
				URL = "http://"+GetUploadHost()+"/"+cloud_data_receive;
				break;

		}
		return URL;
	}
}
