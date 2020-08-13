using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour {

	private FadeController fadeController;

	void Start () {
		GameObject FadeScreen = GameObject.Find ("FadeScreen"); 
        fadeController = FadeScreen.GetComponent<FadeController>();
	}

	public void onEscape(){
		fadeController.nextScene("Home");
	}
}
