using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FlashingText : MonoBehaviour {

	Text text; //点滅させたい文字
	SqliteDatabase sqlDB;
	private float nextTime;
	public float interval = 0.7f; //点滅周期
	float red, green, blue, alpha;   //文字の色、不透明度を管理
	bool isFlash;
	// Use this for initialization
    void Start()
    {
		text = GetComponent<Text> ();
		red = text.color.r;
		green = text.color.g;
		blue = text.color.b;
		alpha = text.color.a;

		isFlash = true;
		nextTime = Time.time;
	}

    // Update is called once per frame
    void Update()
    {
		if ( Time.time > nextTime ) {
			Flashing();
			nextTime += interval;
		}
    }
	
	void Flashing(){
		if(isFlash == true){
			isFlash = false;
			alpha = 255.0f;
			SetAlpha();
		}else{
			isFlash = true;
			alpha = 0.0f;
			SetAlpha();	
		}
	}
	void SetAlpha(){
		text.color = new Color(red, green, blue, alpha);
	}
}