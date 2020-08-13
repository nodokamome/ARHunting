using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using UnityEngine.Networking;


public class StartDirector : MonoBehaviour
{
    //UI
    public InputField inputFiled;
    public Text warningText;
    public Text warningText2;
    //通信部分
    private ServerHostSetting serverHostSetting = new ServerHostSetting();
    private float TimeOutSec;
    private string URL;
    //UserDataクラス
    private UserData userData = new UserData();
    //UserDataDBクラス
    private UserDataDB userDataDB = new UserDataDB();
    //Methodクラス（名前チェックのため）
    private Method method = new Method();
    //DialogControllerとFadeController
    private DialogController RegistDialogController, WifiDialogController;
    private FadeController fadeController;
    //Color
    private Color collectColor = new Color(47f / 255f, 180f / 255f, 60f / 255f);
    private Color warningColor = new Color(255f / 255f, 32f / 255f, 32f / 255f);

    private AudioSource StartSound;
    private AudioSource ErrorSound;

    private int Sound;

    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        StartSound = audioSources[0];
        ErrorSound = audioSources[1];

        //RegistDialog
        GameObject RegistltDialog = GameObject.Find("RegistDialog");
        RegistDialogController = RegistltDialog.GetComponent<DialogController>();

        //WifiDialog
        GameObject WifiDialog = GameObject.Find("WifiDialog");
        WifiDialogController = WifiDialog.GetComponent<DialogController>();

        //FadeScreenDialog
        GameObject FadeScreen = GameObject.Find("FadeScreen");
        fadeController = FadeScreen.GetComponent<FadeController>();

        //URL, timeoutを取得
        TimeOutSec = serverHostSetting.TimeOutSec;
        userData.id = userDataDB.GetUserData("id");

        Sound = int.Parse(userDataDB.GetUserData("sound"));
    }

    //画面をタップした時の処理
    public void TapStart()
    {
        URL = serverHostSetting.GetURL("StartDirector");
        StartCoroutine(HttpPost(URL, userData, "start"));  // POST
    }

    private void InitializeText()
    {
        warningText.text = "サーバに接続しました";
        inputFiled.text = "";
    }


    //登録ボタンを押した時
    public void onButtonRegist()
    {
        string nameCheck = method.NameCheck(inputFiled.text);
        if (nameCheck != "")
        {
            warningText.color = warningColor;
            warningText.text = nameCheck;
        }
        else
        {
            //idを取得
            userData.id = userDataDB.GetUserData("id");
            userData.name = inputFiled.text;
            StartCoroutine(HttpPost(URL, userData, "regist"));  // POST
        }
    }


    IEnumerator HttpPost(string URL, UserData data, string key)
    {
        string jsondata = JsonUtility.ToJson(data);
        WWWForm form = new WWWForm();
        form.AddField(key, jsondata);
        //1.UnityWebRequestを生成
        UnityWebRequest request = UnityWebRequest.Post(URL, form);
        //2.SendWebRequestを実行し、送受信開始
        yield return StartCoroutine(CheckTimeOut(request, TimeOutSec));

        if (request.isHttpError || request.isNetworkError)
        {
            if (Sound == 1)
            {
                ErrorSound.PlayOneShot(ErrorSound.clip);
            }
            //4.エラー確認
            Debug.Log("HttpPost NG: " + request.error);
            WifiDialogController.isDialogOn = true;
            warningText2.text = "サーバに接続できません\n設定を見直してください";
        }
        else if (request.downloadHandler.isDone)
        {
            if (Sound == 1)
            {
                StartSound.PlayOneShot(StartSound.clip);
            }
            // サーバからのレスポンスを表示

            userData = JsonUtility.FromJson<UserData>(request.downloadHandler.text);
            Debug.Log("HttpPost OK: " + request.downloadHandler.text);

            if (key == "start")
            {
                if (userData.name == "" || userData.name == null)
                {
                    RegistDialogController.isDialogOn = true;
                    warningText.color = collectColor;
                    warningText2.text = "";
                    InitializeText();
                }
                else
                {

                    fadeController.nextScene("Home");
                }
            }

            if (key == "regist")
            {
                string fileName = userDataDB.GetUserData("name") + "_SettingNo" + serverHostSetting.GetSettingNo() + ".csv";
                string filePath = Application.persistentDataPath + "/" + fileName;
                // 画像ファイルをbyte配列に格納
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                userDataDB.SetUserData("name", userData.name);
                StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/" + userData.name + "_SettingNo" + serverHostSetting.GetSettingNo() + ".csv", false, Encoding.GetEncoding("UTF-8"));
                // ヘッダー出力
                string[] s1;
                string s2;
                //ユーザ名
                s1 = new string[] { "ユーザ名", userData.name };
                s2 = string.Join(",", s1);
                sw.WriteLine(s2);
                //SettingNo
                s1 = new string[] { "接続設定", serverHostSetting.GetSettingNo() };
                s2 = string.Join(",", s1);
                sw.WriteLine(s2);
                //表題
                s1 = new string[] { "No.", "日時", "参加人数", "フレームレート（FPS）", "通信遅延（ms）" };
                s2 = string.Join(",", s1);
                sw.WriteLine(s2);
                // StreamWriterを閉じる
                sw.Close();
                fadeController.nextScene("Home");
            }
        }
    }

    IEnumerator CheckTimeOut(UnityWebRequest request, float timeout)
    {
        request.SendWebRequest();
        float requestTime = Time.time;
        while (!request.downloadHandler.isDone)
        {
            if (Time.time - requestTime < timeout)
                yield return null;
            else
            {
                if (Sound == 1)
                {
                    ErrorSound.PlayOneShot(ErrorSound.clip);
                }
                Debug.Log("TimeOut");  //タイムアウト
                WifiDialogController.isDialogOn = true;
                warningText2.text = "サーバに接続できません\n設定を見直してください";
                break;
            }
        }
        yield return null;
    }
}
