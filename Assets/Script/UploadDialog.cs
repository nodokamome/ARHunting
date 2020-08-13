using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;

public class UploadDialog : MonoBehaviour {
	//通信部分
	private ServerHostSetting serverHostSetting = new ServerHostSetting();
    private string URL;
	//UserDataDBクラス
    private UserDataDB userDataDB = new UserDataDB();
	//fileの高さとスペース
	public GameObject filePref;  
	GameObject[] file = new GameObject[10];

	private float fileSpace;
	private float fileHeight;
	private RectTransform content; 
	private int fileCount =  3;
	private Color collectColor = new Color(27f / 255f, 176f / 255f, 76f / 255f);
    private Color warningColor = new Color(255f / 255f, 32f / 255f, 32f / 255f);
	void Start () {
		//URL, TimeOutSec取得
		URL = serverHostSetting.GetURL("UploadDialog");
		//スクロールとテキストの設定
		content = GameObject.Find("UploadDialog/BackGround/ScrollBack/ScrollView/Viewport/Content").GetComponent<RectTransform>();
		fileSpace = content.GetComponent<VerticalLayoutGroup>().spacing;
		fileHeight = filePref.GetComponent<LayoutElement>().preferredHeight;
		content.sizeDelta = new Vector2(0, (fileHeight + fileSpace) * fileCount);
	}
	
	public void onUplaod(){
		if(file[1]!=null){
			if(file[1].transform.Find("Toggle").GetComponent<Toggle>().isOn){
				StartCoroutine(HttpUpload(URL, "file", 1));
			}
		}if(file[2]!=null){
			if(file[2].transform.Find("Toggle").GetComponent<Toggle>().isOn){
        		StartCoroutine(HttpUpload(URL, "file", 2));
			}
		}if(file[3]!=null){
			if(file[3].transform.Find("Toggle").GetComponent<Toggle>().isOn){
        		StartCoroutine(HttpUpload(URL, "file", 3));
			}
		}
	}

	public void ListUpdate(){
		for(int i = 1; i <= 3; i++){
			string fileName = userDataDB.GetUserData("name")+"_SettingNo"+i+".csv";
        	string filePath = Application.persistentDataPath + "/"+fileName;
        	// 画像ファイルをbyte配列に格納
			if(File.Exists(filePath) && file[i] == null){
				//テキスト生成
				file[i] = (GameObject)Instantiate(filePref);
				//テキストをContentの子に設定
				file[i].transform.SetParent(content, false);
				//テキスト変更を更新
				file[i].transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
				file[i].transform.Find("fileText").GetComponent<Text>().text = userDataDB.GetUserData("name")+"_SettingNo"+i+".csv";
				file[i].transform.Find("status").GetComponent<Text>().text = "";
			}if(file[i] != null){
				//テキスト変更を更新
				file[i].transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
				file[i].transform.Find("status").GetComponent<Text>().text = "";
			}
		}
	}

	IEnumerator HttpUpload(string URL,string key, int SettingNo)
    {
		string fileName = userDataDB.GetUserData("name")+"_SettingNo"+SettingNo+".csv";
        string filePath = Application.persistentDataPath + "/"+fileName;
        // 画像ファイルをbyte配列に格納
        byte[] csvFile = File.ReadAllBytes (filePath);
		
		//formにcsvファイルを追加
		WWWForm form = new WWWForm ();
        form.AddBinaryData ("file", csvFile, fileName, "text/csv");
		
		UnityWebRequest request = UnityWebRequest.Post(URL, form);
        yield return request.SendWebRequest();

		if (request.isHttpError || request.isNetworkError) {
            // POSTに失敗した場合，エラーログを出力
            Debug.Log (request.error);
        } else {
            // POSTに成功した場合，レスポンスコードを出力
			Debug.Log(request.downloadHandler.text);
			if(request.downloadHandler.text=="successfully upload"){
				file[SettingNo].transform.Find("status").GetComponent<Text>().color = collectColor;
				file[SettingNo].transform.Find("status").GetComponent<Text>().text = "アップロード成功";
			
			}else if(request.downloadHandler.text=="successfully overwrite"){
				file[SettingNo].transform.Find("status").GetComponent<Text>().color = collectColor;
				file[SettingNo].transform.Find("status").GetComponent<Text>().text = "上書き成功";
				
			}else{
				file[SettingNo].transform.Find("status").GetComponent<Text>().color = warningColor;
				file[SettingNo].transform.Find("status").GetComponent<Text>().text = "アップロード失敗";
			}
        }
    }
}
