using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //パネルのイメージを操作するのに必要

public class DialogController : MonoBehaviour {
	
	float offSpeed = 0.12f;        //透明度が変わるスピードを管理
	float onSpeed = 0.08f;        //透明度が変わるスピードを管理
	float alpha;   //パネルの色、不透明度を管理

	public bool isDialogOff = false;   //フェードイン処理の開始、完了を管理するフラグ
	public bool isDialogOn = false;  //フェードアウト処理の開始、完了を管理するフラグ

	CanvasGroup dialogAlfa;                //透明度を変更するパネルのイメージ
	Canvas dialogCanvas;

	
	void Start () {

		dialogAlfa = GetComponent<CanvasGroup> ();
		dialogCanvas = GetComponent<Canvas> ();


		alpha = dialogAlfa.alpha; 
	}
	void Update () {

		if(isDialogOff){
			StartDialogOff ();
		}

		if (isDialogOn) {
			StartDialogOn ();
		}
	}

	void StartDialogOff(){
		alpha -= offSpeed;                //a)不透明度を徐々に下げる
		SetAlpha ();               		  //b)変更した透明度をパネルに反映する
		if(alpha <= 0){                    //c)完全に透明になったら処理を抜ける
			isDialogOff = false;             
			dialogCanvas.enabled = false;    //d)パネルの表示をオフにする
		}
	}

	void StartDialogOn(){
		dialogCanvas.enabled = true;  // a)パネルの表示をオンにする
		alpha += onSpeed;         // b)不透明度を徐々にあげる
		SetAlpha ();               // c)変更した透明度をパネルに反映する
		if(alpha >= 1){             // d)完全に不透明になったら処理を抜ける
			isDialogOn = false;
		}
	}

	void SetAlpha(){
		dialogAlfa.alpha = alpha;
	}

	public void OpenDialog(){
		isDialogOn = true;
	}

	public void CloseDialog(){
		isDialogOff = true;
	}
}