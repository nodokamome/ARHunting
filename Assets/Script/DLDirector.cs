using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class DLDirector : MonoBehaviour {
    //UI
    public RawImage MonsterImage;
    public Text PercentText;
    //FadeController
	private FadeController fadeController;
	//DLDataクラス
    private DLData dlData = new DLData();
    //UserDataクラス
    private UserData userData = new UserData();
    //UserDataDBクラス   
    private UserDataDB userDataDB = new UserDataDB();   
    //通信部分
	private ServerHostSetting serverHostSetting = new ServerHostSetting();
    private float TimeOutSec;
    private string URL;
    //DL管理
    private bool DLStart = true;
    public static float DLpercent;
	[System.NonSerialized]
    //DLするデータ
    public static string monsterName;
    public static int currentHPMonster;
    public static int initialHPMonster;
    public static int power;
    public static int defence;

    void Start() {
        //URL, TimeOutSec取得
        TimeOutSec = serverHostSetting.TimeOutSec;
		URL = serverHostSetting.GetURL("DLDirector");
        //FadeController
        GameObject FadeScreen = GameObject.Find ("FadeScreen"); 
        fadeController = FadeScreen.GetComponent<FadeController>();
        MonsterImage.texture = ScrollViewScript.monsterImage;
    }

    void Update(){
        //DLのフラグ管理
        if(!fadeController.isFadeIn && DLStart){
            userData.id = userDataDB.GetUserData("id");
            userData.entryMonsterNum = ScrollViewScript.entryMonsterNum;
            StartCoroutine(HttpPost(URL, userData , "DL"));  // POST
            DLStart = false;
            PercentText.text = DLpercent + " %";
        }
    }


	IEnumerator HttpPost(string URL, UserData data ,string key)
    {   
        string jsondata = JsonUtility.ToJson(data);
        WWWForm form = new WWWForm ();
        form.AddField (key, jsondata);
        //1.UnityWebRequestを生成
        UnityWebRequest request = UnityWebRequest.Post(URL,form);
        //2.SendWebRequestを実行し、送受信開始
        yield return StartCoroutine(CheckTimeOut(request, TimeOutSec));

        //3.isNetworkErrorとisHttpErrorでエラー判定
        if(request.isHttpError || request.isNetworkError) {
            //4.エラー確認
            Debug.Log("HttpPost NG: " + request.error);
            DLStart = true;
        }
        else if(request.downloadHandler.isDone){
            //4.結果確認
            dlData = JsonUtility.FromJson<DLData>(request.downloadHandler.text);
            Debug.Log("DLLLLLLLHttpPost OK: " + request.downloadHandler.text);
            //ダウンロードしたデータ
            monsterName = dlData.monsterName;
            initialHPMonster = dlData.initialHPMonster;
            currentHPMonster = dlData.currentHPMonster;
            power = dlData.power;
            defence = dlData.defence;
    
            fadeController.nextScene("Game");
        }
    }

    IEnumerator CheckTimeOut(UnityWebRequest request, float timeout)
    {
        request.SendWebRequest();
        float requestTime = Time.time;
        while(!request.downloadHandler.isDone)
        {
            if(Time.time - requestTime < timeout)
                yield return null;
            else
            {
                DLStart = true;
                Debug.Log("TimeOut");  //タイムアウト
                break;
            }
        }
        yield return null;
    }
}