using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveScript : MonoBehaviour {
	public string[] initialchars; // this should be a list of 5 values to fill in
	public string[] goalchars;
	// Ex1:["1", "+", "2", "=", "?"]
	// Ex2:["?", "+", "?", "=", "6"]
	public int[] orderOfSelection; // this is the order that the chars will be replaced in (shot down) ex[4]
	// Ex1: [4] means that we put the first thing you shoot in the 4th slot then evaluate
	// Ex2: [0,2] means that we put the first thing shot in first place, the second thing shot in the 3rd place then evaluate
	public string goalText; //Wave instructions printed to player

	//Verifying
	string[] currentchars;
	int currentSelection;
	bool doneWithValues = false;
	bool correctExpression = false;
	int waveSuccessState = 0;
	float startTime;
	float startDelay = 3;
	bool started = false;

	//PlayerHP
	public int startHP;

	//UI
	UICharSystem uics;
	//UI strings
	string finishthemoffGood = "Good work! Eliminate the rest to continue";
	string finishthemoffBad = "Not Quite... eliminate the rest and try again";

	void Start(){
		this.currentchars = this.initialchars;
		this.currentSelection = 0;
		this.uics = GetComponentInParent<UICharSystem> ();
		this.uics.setCharUI (this.currentchars);
		this.uics.setHighlightUI(this.orderOfSelection[currentSelection]);
		this.uics.goalText.text = goalText;
		SetPlayerHealth (this.startHP);
		this.startTime = Time.time;
		Freeze (false); // Freeze false sets enemy shoot, playe shoot/move to false
	}

	void SetPlayerHealth (int h){
		GetComponentInChildren<PlayerController> ().GetComponent<HealthSystem> ().startHealth = h;
	}

	public int GetPlayerHealth(){
		return GetComponentInChildren<PlayerController> ().GetComponent<HealthSystem> ().GetHealth();
	}

	void Freeze(bool b){
		foreach (Transform child in this.transform) {
			if (child.tag == "Enemy") {
				child.gameObject.GetComponent<EnemyScript> ().allowShoot = b;
			} else if (child.tag == "Player") {
				PlayerController pc = child.gameObject.GetComponent<PlayerController> ();
				pc.allowShoot = b;
				pc.allowMove = b;
			}
		}
		this.uics.setGoImage(b);
	}

	void Update(){
		if (!this.started && Time.time - this.startTime > startDelay) {
			Freeze (true);
			this.started = true;
		}
	}

	public void addChar(string s){ //Sorry everyone, add character takes a string...
		if (!this.doneWithValues) {
			this.currentchars [this.orderOfSelection [this.currentSelection++]] = s;
			this.uics.setCharUI (this.currentchars);
			if (this.currentSelection >= this.orderOfSelection.Length) {
				//We're all done with our shot numbers
				this.doneWithValues = true;
				this.uics.setHighlightUI (-1);
				evalExpression ();
			}
			else {
				this.uics.setHighlightUI (this.orderOfSelection [this.currentSelection]);
			}
		} else
			Debug.Log ("addChar: Woah, added another char after we thought we were done?"); // turns out this is ok as long as we have that first if statement
	}

	public int isWaveDone (){ //Return 1 for wave success, 0 for wave not done yet, -1 for wave failure
		if (countEnemy()<= 0) { // If all guys are shot, return success state
			if (this.waveSuccessState == 0){
				Debug.Log ("ERRRRROR this should be set to 1 or -1 by the time all guys are shot..");
				return 1; // return 1 to keep the game moving
			}
			return this.waveSuccessState;
		}
		else { // If not all the guys have been shot, we're not done yet
			return 0;
		}
	}

	int countEnemy(){
		int c = 0;
		foreach (Transform child in this.transform) {
			if (child.tag == "Enemy")
				c++; // :)
		}
		return c;
	}

	void evalExpression(){
		Debug.Log (string.Format("goal: {0}, curr: {1}", string.Join("", this.currentchars), string.Join("", this.goalchars)));
		if (string.Join("", this.currentchars) == string.Join("", this.goalchars)) {
			//Debug.Log ("Nice Job! you did it!");
			this.uics.goalText.text = this.finishthemoffGood;
			this.waveSuccessState = 1;
		}
		else {
			//Debug.Log ("Holy fuck you're terrible!");
			for(int i = 0; i < this.currentchars.Length; i++) {
				if (this.currentchars [i] == "=")
					this.currentchars [i] = "!=";
				this.uics.setCharUI (this.currentchars);
			}
			this.uics.goalText.text = this.finishthemoffBad;
			this.waveSuccessState = -1;
		}
	}


}
