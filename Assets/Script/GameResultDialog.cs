using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameResultDialog : MonoBehaviour
{
    //UI
    public Text TextResult;
    public Text TextExplain;
    public Image ImageResultBack;
    private string result01;
    private string explain01;
    //UserDataDBクラス
    private UserDataDB userDataDB = new UserDataDB();
    private EscapeData escapeData = new EscapeData();
    //通信部分
    private ServerHostSetting serverHostSetting = new ServerHostSetting();
    private float TimeOutSec;
    private string URL;
    //FadeController
    private FadeController fadeController;
    //DialogController
    private DialogController resultDialog;
    public static bool DeleteStart = false;
    //Color
    private Color loseColor = new Color(66.0f / 255.0f, 110.0f / 255.0f, 253.0f / 255.0f);
    private Color winColor = new Color(253.0f / 255.0f, 195.0f / 255.0f, 66.0f / 255.0f);
    GameObject gameDataSharing;
    GameDataSharing script;


    void Start()
    {
        //URL, TimeOutSec取得
        TimeOutSec = serverHostSetting.TimeOutSec;
        URL = serverHostSetting.GetURL("GameResult");
        //FadeScreenDialog
        GameObject FadeScreen = GameObject.Find("FadeScreen");
        fadeController = FadeScreen.GetComponent<FadeController>();
        //ResultDialog
        resultDialog = GetComponent<DialogController>();
        gameDataSharing = GameObject.Find("GameDataSharing");
        script = gameDataSharing.GetComponent<GameDataSharing>();
    }

    public void StartDialog(string result, string explain)
    {
        resultDialog.isDialogOn = true;
        result01 = result;
        explain01 = explain;
    }

    public void Escape()
    {
        DeleteStart = true;
        script.Close();
    }

    void Update()
    {
        if (resultDialog.isDialogOn)
        {
            TextResult.text = result01;
            TextExplain.text = explain01;
            if (result01 == "敗北")
            {
                ImageResultBack.color = loseColor;
            }
            else if (result01 == "勝利")
            {
                ImageResultBack.color = winColor;
            }
            else
            {
                ImageResultBack.color = loseColor;
            }
        }
        if (!fadeController.isFadeIn && DeleteStart)
        {
            escapeData.idUser = userDataDB.GetUserData("id");
            escapeData.entryMonsterNum = ScrollViewScript.entryMonsterNum;
            StartCoroutine(HttpPost(URL, escapeData, "escape"));
        }
    }

    void OnApplicationPause()
    {
        script.Close();
        Debug.Log("OnApplicationPause()");
        fadeController.nextScene("Home");
        DeleteStart = false;
    }

    IEnumerator HttpPost(string URL, EscapeData data, string key)
    {
        string jsondata = JsonUtility.ToJson(data);
        WWWForm form = new WWWForm();
        form.AddField(key, jsondata);
        UnityWebRequest request = UnityWebRequest.Post(URL, form);
        //2.SendWebRequestを実行し、送受信開始
        yield return StartCoroutine(CheckTimeOut(request, TimeOutSec));
        ////ここまで
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log("HttpPost NG: " + request.error);
            DeleteStart = true;
        }
        else if (request.downloadHandler.isDone)
        {
            fadeController.nextScene("Home");
            DeleteStart = false;
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
                Debug.Log("TimeOut");  //タイムアウト
                break;
            }
        }
        yield return null;
    }



}
