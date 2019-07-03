using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharSystem : MonoBehaviour {
    public static UICharSystem instance; // singletonio

	//Health System
	public Image[] healthImages;
	public Sprite[] healthSprites;
	//Math System
	public Image[] charUI;
	public Image[] highlightUI;
	public Text goalText;
	public Text currLevel;
	public Image GoImage;
	public Sprite GoOn;
	public Sprite GoOff;
	public CharacterManager cm;

	void Awake(){
		if(instance == null){
			instance = this;
		}
		else if(instance != this){
			Debug.LogErrorFormat("Singleton {0} instantiated multiple times, destroy all but first one up",this.GetType().Name);
			Destroy(this);
		}
	}

	void Start(){
		this.cm = GetComponent<CharacterManager> ();
		if(cm == null){
			Debug.LogError("UIChar system relies on character manager, componenet");
		}
		setHighlightUI (-1);
	}

	public void setGoImage(bool on){
		if (on) // We want the image turned on with the off sprite
			this.GoImage.sprite = this.GoOn;
		else {
			this.GoImage.sprite = this.GoOff;
		}
	}

	public void updateHealthBar(int cur_health){
		for (int i = 0; i < healthImages.Length; i++) {
			if (cur_health <= i) {
				healthImages [i].sprite = healthSprites [0];
			}
			else {
				healthImages [i].sprite = healthSprites [1];
			}
		}
	}

	public void setCharUI(string[] input){
		if (input.Length != 5) {
			Debug.Log (string.Format ("setHighlightUI(): ERROR bad input {0}", input));
			return;
		}
		for (int i = 0; i < 5; i++) {
			charUI [i].sprite = this.cm.getCharaterSprite (input [i]);
		}
	}

	public void setHighlightUI(int x){
		if (x < -1 || x > 4)
			Debug.Log(string.Format("setHighlightUI(): ERROR bad input: {0}", x));
		if (x == -1) { // disable all
			foreach (Image img in this.highlightUI) {
				img.enabled = false;
			}
		}
		else { // Enable only specific highlightUI element
			for (int i = 0; i < 5; i++) {
				if (i == x)
					this.highlightUI [i].enabled = true;
				else
					this.highlightUI [i].enabled = false;
			}
		}
	}		
}
