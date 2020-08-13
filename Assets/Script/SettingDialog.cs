using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class SettingDialog : MonoBehaviour {
    //UI
	public InputField inputFiled;
    public Text warningText;
    public Text NameText;
	public Toggle isSoundCheck;
    public GameObject toggleObject;
    private Toggle toggle01;
    //Name, Sound, nameCheck
    private string newName;
    private string Name;
    private int Sound;
    private int newSound;
    private string id;
	private string nameCheck;
    //FadeController
	private FadeController fadeController;
    //通信部分
	private ServerHostSetting serverHostSetting = new ServerHostSetting();
    private float TimeOutSec;
    private string URL;
    //UserDataDBクラス
    private UserDataDB userDataDB = new UserDataDB();
    //Methodクラス（名前チェックのため）
    private Method method = new Method();
    private UserData userData = new UserData();
    //DialogControlloer
    private DialogController SettingDialogController;
    
    void Start(){
        //SettingDialog
        GameObject SettingDialog = GameObject.Find ("SettingDialog");                             
        SettingDialogController = SettingDialog.GetComponent<DialogController>();
	   	//FadeScreenDialog
        GameObject FadeScreen = GameObject.Find ("FadeScreen"); 
        fadeController = FadeScreen.GetComponent<FadeController>();
        //自分のIDを取得
        userData.id = userDataDB.GetUserData("id");
        //Toggle
        toggleObject = GameObject.Find ("Toggle01");                             
        toggle01 = toggleObject.GetComponent<Toggle>();
        //URL, timeoutを取得
        TimeOutSec = serverHostSetting.TimeOutSec;
        URL = serverHostSetting.GetURL("SettingDialog");
    }
	public void StartDialog () {
        //名前入力、効果音設定
		inputFiled.text = userDataDB.GetUserData("name");
        Sound = int.Parse(userDataDB.GetUserData("sound"));
        //設定初期化
        SettingInitialize();
	}

	public void onClickSave()
    {
        //名前チェック
        nameCheck = method.NameCheck(inputFiled.text);
        if(nameCheck != ""){
            warningText.text = nameCheck;
        }else{
            userData.name = inputFiled.text;
            newName = inputFiled.text;
            if(toggle01.isOn){
                newSound = 1;
            }else{
                newSound = 0;
            }
            SettingDialogController.isDialogOff = true;
            //新しい名前をCloudへ送信
            StartCoroutine(HttpPost(URL, userData , "update"));  // POST
        }
    }

    public void onButtonTitle(){
		fadeController.nextScene("Start");
    }

	IEnumerator HttpPost(string URL,UserData data ,string key)
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
            SetWarningText();
        }
        else if(request.downloadHandler.isDone){
            //4.結果確認
            if(key == "update"){
                userDataDB.SetUserData("name", newName);
                userDataDB.SetUserData("sound", newSound.ToString());
                NameText.text = newName;
            }
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
                Debug.Log("TimeOut");  //タイムアウト
                SetWarningText();
                break;
            }
        }
        yield return null;
    }

    private void SetWarningText(){
        warningText.text = "サーバに接続できません";
    }

    private void SettingInitialize(){
		warningText.text = "";
        newName = "";
		newSound = Sound;
        nameCheck = "";

        
		if(Sound == 1){
			toggle01.isOn = true;
		}else{
			toggle01.isOn = false;
        }
    }
}
