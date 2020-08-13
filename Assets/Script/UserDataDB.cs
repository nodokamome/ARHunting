using UnityEngine;
using System.Collections;

public class UserDataDB{
	private string data;
	private SqliteDatabase sqlDB;
    private string query;
    
	public string GetUserData(string key){
		sqlDB = new SqliteDatabase("device_db.db");

        string selectQuery = "select * from UserData where file = 1";
        DataTable dataTable = sqlDB.ExecuteQuery(selectQuery);
        foreach(DataRow dr in dataTable.Rows){
            data = dr[key].ToString();
        }

		return data;
	}

    public void SetUserData(string element, string data){
		sqlDB = new SqliteDatabase("device_db.db");

        query = "update UserData set "+element+"='"+ data +"' where file ="+1;
        sqlDB.ExecuteNonQuery(query);  
	}
}
