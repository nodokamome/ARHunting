using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class ScrollViewScript : MonoBehaviour {

	[SerializeField]
	//MonsterButton
	public GameObject btnPref;  
	private int buttonCount = 10;
	GameObject[] btn = new GameObject[10];
	
	//BattleDialogのもの
	private DialogController BattleDialogController;
	public Text NameText;
	public Text HPText;
	public Text PowerText;
	public Text DefenceText;
	public Text popPlaceText;
	public Text userCountText;
	public Sprite Monster1;
	public RawImage MonsterImage;
	public Text warningText;
	//buttonの高さとスペース
	private float btnSpace;
	private float btnHeight;
	private RectTransform content; 
	public static Texture monsterImage;

	[System.NonSerialized]
	//選択したMonsterNum
	public static int entryMonsterNum;
	//通信部分
	private ServerHostSetting serverHostSetting = new ServerHostSetting();
    private float TimeOutSec;
    private string URL;
	private float leftTime = 1.0f;
	private float Span = 1.0f;
	//MonsterListDataクラス
    public MonsterListData monsterListData = new MonsterListData();                                         
	//UserDataDBクラス
    private UserDataDB userDataDB = new UserDataDB();
    private UserData userData = new UserData();
	//FadeController
	private FadeController fadeController;
	//Color
    private Color collectColor = new Color(47f / 255f, 180f / 255f, 60f / 255f);
    private Color warningColor = new Color(255f / 255f, 32f / 255f, 32f / 255f);
    private AudioSource SelectSound;

	void Start (){
		AudioSource audioSource = GetComponent<AudioSource>();
 	    SelectSound = audioSource;

		//BattleDialog
		GameObject BattleDialog = GameObject.Find ("BattleDialog");                             
        BattleDialogController = BattleDialog.GetComponent<DialogController>();
	   	//FadeScreenDialog
        GameObject FadeScreen = GameObject.Find ("FadeScreen"); 
        fadeController = FadeScreen.GetComponent<FadeController>();
		//URL, TimeOutSec取得
        TimeOutSec = serverHostSetting.TimeOutSec;
		URL = serverHostSetting.GetURL("ScrollViewScript");
		//スクロールとボタンの設定
		content = GameObject.Find("MainCanvas/BackGround/ScrollBlueBack/ScrollView/Viewport/Content").GetComponent<RectTransform>();
		btnSpace = content.GetComponent<VerticalLayoutGroup>().spacing;
		btnHeight = btnPref.GetComponent<LayoutElement>().preferredHeight;
		content.sizeDelta = new Vector2(0, (btnHeight + btnSpace) * buttonCount);
		//IDと名前取得
		userData.id = userDataDB.GetUserData("id");
		userData.hp = int.Parse(userDataDB.GetUserData("currentHP"));
        StartCoroutine(HttpPost(URL,userData, "MonsterList"));
	}
		
	void Update(){
		leftTime -= Time.deltaTime;
            if (leftTime <= 0.0) {
                leftTime = Span;
				userData.hp = int.Parse(userDataDB.GetUserData("currentHP"));
                StartCoroutine(HttpPost(URL,userData, "MonsterList"));
            }
    }

	public void onClick(int monsterNum){
		if(userDataDB.GetUserData("sound") == "1"){
			SelectSound.PlayOneShot(SelectSound.clip);
		}	
		entryMonsterNum = monsterNum;
		BattleDialogController.isDialogOn = true;
		NameText.text = btn[entryMonsterNum].transform.Find("WhiteBox/NameText").GetComponent<Text>().text;
		HPText.text = btn[entryMonsterNum].transform.Find("HPText").GetComponent<Text>().text;
		PowerText.text = btn[entryMonsterNum].transform.Find("PowerText00/PowerText").GetComponent<Text>().text;
		DefenceText.text = btn[entryMonsterNum].transform.Find("DefenceText00/DefenceText").GetComponent<Text>().text;
		popPlaceText.text = btn[entryMonsterNum].transform.Find("popPlaceText00/popPlaceText").GetComponent<Text>().text;
		userCountText.text = btn[entryMonsterNum].transform.Find("userCountText00/userCountText").GetComponent<Text>().text;;
		MonsterImage.texture = btn[entryMonsterNum].transform.Find("MonsterImage").GetComponent<RawImage>().texture;
		monsterImage = btn[entryMonsterNum].transform.Find("MonsterImage").GetComponent<RawImage>().texture;
	}

	public void EntryMonsterBattle(){
		fadeController.nextScene("DL");
	}
	
	private void Initialize(){
		NameText.text = "";
		HPText.text = "? / ?";
		PowerText.text = "?";
		popPlaceText.text = "?";
		userCountText.text = "? 人";
	}

	
	IEnumerator HttpPost(string URL,UserData data ,string key)
    {

        string jsondata = JsonUtility.ToJson (data);
        WWWForm form = new WWWForm ();
        form.AddField (key, jsondata);
        UnityWebRequest request = UnityWebRequest.Post(URL,form);

        // CheckTimeOut()の終了を待つ。5秒を過ぎればタイムアウト
		//通信速度測定
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();   
		stopwatch.Start ();
        yield return StartCoroutine(CheckTimeOut(request, TimeOutSec));

        if(request.isHttpError || request.isNetworkError) {
            //Debug.Log("HttpPost NG: " + www.error);
		
            warningText.color = warningColor;
            warningText.text = "サーバに接続できません";
			Initialize();
        }
        else if(request.downloadHandler.isDone){
			stopwatch.Stop ();
			//処理時間を秒で取得
			float elapsed = (float)stopwatch.Elapsed.TotalSeconds; 
            //Debug.Log(elapsed*1000+" ms");

            warningText.color = collectColor;
        	warningText.text = "サーバに接続しました";
            
			// サーバからのレスポンスを表示
            monsterListData = JsonUtility.FromJson<MonsterListData>(request.downloadHandler.text);
           	//Debug.Log("HttpPost OK: " + www.text);
			if(monsterListData.num.Length != 0){
			
            	if(key == "MonsterList"){
					for(int i = 0; i < monsterListData.num.Length; i++){
						int num = i;
						if(btn[monsterListData.num[i]] == null){
							//ボタン生成
							btn[monsterListData.num[i]] = (GameObject)Instantiate(btnPref);
							//ボタンをContentの子に設定
							btn[monsterListData.num[i]].transform.SetParent(content, false);
							btn[monsterListData.num[i]].transform.GetComponent<Button>().onClick.AddListener(() => onClick(num+1));
							// wwwクラスのコンストラクタに画像URLを指定
        					string url = serverHostSetting.GetURL("MonsterImage")+monsterListData.monsterImage[i];
        					UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
					        yield return www.SendWebRequest();

        					// 画像ダウンロード完了を待機
        					yield return www;
							// webサーバから取得した画像をRaw Imagで表示する
							btn[monsterListData.num[i]].transform.Find("MonsterImage").GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

						}
						//ボタンのテキスト変更を更新
						btn[monsterListData.num[i]].transform.Find("WhiteBox/NameText").GetComponent<Text>().text = monsterListData.name[i];
						btn[monsterListData.num[i]].transform.Find("HPText").GetComponent<Text>().text = monsterListData.currentHP[i] +"/"+ monsterListData.initialHP[i];
						btn[monsterListData.num[i]].transform.Find("PowerText00/PowerText").GetComponent<Text>().text = monsterListData.power[i].ToString();
						btn[monsterListData.num[i]].transform.Find("DefenceText00/DefenceText").GetComponent<Text>().text = monsterListData.defence[i].ToString();
						btn[monsterListData.num[i]].transform.Find("popPlaceText00/popPlaceText").GetComponent<Text>().text = monsterListData.popPlace[i];
						btn[monsterListData.num[i]].transform.Find("userCountText00/userCountText").GetComponent<Text>().text = monsterListData.userCount[i].ToString()+" 人";
					}
				}
			}
        }
    }

	public void ListUpdate(){
		for(int i = 0; i < btn.Length; i++){
			Destroy(btn[i]);
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
            	warningText.color = warningColor;
            	warningText.text = "サーバに接続できません";
                break;
            }
        }
        yield return null;
    }
}