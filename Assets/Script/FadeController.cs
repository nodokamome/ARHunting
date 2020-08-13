using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI; //パネルのイメージを操作するのに必要

public class FadeController : MonoBehaviour {

	//fadeIn,outのスピードを管理
	float fadeInSpeed = 0.2f;        
	float fadeOutSpeed = 0.07f;        
	//パネルの色、不透明度を管理
	float red, green, blue, alpha;   
	//フェードアウト処理の開始、終了の完了を管理するフラグ
	public bool isFadeOut = false;  
	public bool isFadeIn = true;   
	private string nextSceneName;
	//透明度を変更するパネルのイメージ
	private Image fadeImage;                

	//　非同期動作で使用するAsyncOperation
	private AsyncOperation async;
	//　シーンロード中に表示するUI画面
	[SerializeField]
	public GameObject loadUI;
	public GameObject loadTitle;
	public GameObject loadText;
 

	void Start () {
		fadeImage = GetComponent<Image> ();
		red = fadeImage.color.r;
		green = fadeImage.color.g;
		blue = fadeImage.color.b;
		alpha = fadeImage.color.a;
		loadUI.SetActive(false);
		loadTitle.SetActive(false);
		loadText.SetActive(false);
	}

	void Update () {
		if(isFadeIn){
			StartFadeIn ();
		}

		if (isFadeOut) {
			StartFadeOut ();
		}
	}

	void StartFadeIn(){
		alpha -= fadeInSpeed;                //a)不透明度を徐々に下げる
		SetAlpha ();                      //b)変更した不透明度パネルに反映する
		if(alpha <= 0){                    //c)完全に透明になったら処理を抜ける
			isFadeIn = false;             
			fadeImage.enabled = false;    //d)パネルの表示をオフにする
		}
	}
	

	void StartFadeOut(){
		fadeImage.enabled = true;  // a)パネルの表示をオンにする
		alpha += fadeOutSpeed;         // b)不透明度を徐々にあげる
		SetAlpha ();               // c)変更した透明度をパネルに反映する
		
		if(alpha >= 0.5){
			loadUI.SetActive(true);
			loadTitle.SetActive(true);
			loadText.SetActive(true);
		}
		if(alpha >= 1){             // d)完全に不透明になったら処理を抜ける
			isFadeOut = false;
			//　ロード画面UIをアクティブにする
			//　コルーチンを開始
			StartCoroutine(LoadData());
		}
	}

	void SetAlpha(){
		fadeImage.color = new Color(red, green, blue, alpha);
	}

	public void nextScene(string Scene){
    	isFadeOut = true;
		nextSceneName = Scene;
    }

	IEnumerator LoadData() {
		// シーンの読み込みをする
		async = SceneManager.LoadSceneAsync(nextSceneName);
 
		//　読み込みが終わるまで進捗状況をスライダーの値に反映させる
		while(!async.isDone) {

			var progressVal = Mathf.Clamp01(async.progress / 0.9f);
            DLDirector.DLpercent = progressVal*100;

			yield return null;
		}
	}
}