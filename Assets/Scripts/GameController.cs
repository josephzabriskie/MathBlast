using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public bool launch = false;
	public GameObject EventController;
	public GameObject playerMenu;
	EventController ec;
	public GameObject StartButton;
	public Image Title;
	Vector3 playerMenuStartPos;
	public int LevelStartNum;

	// Use this for initialization
	void Start () {
		this.ec = this.EventController.GetComponent<EventController> ();
		this.playerMenuStartPos = this.playerMenu.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.launch) {
			StartGame ();
		}
		if (Input.GetKeyDown (KeyCode.Escape)){
			Application.Quit();
			//Application.
		}
	}

	void StartGame(){
		this.launch = false;
		this.Title.enabled = false; // Could fade out
		this.StartButton.SetActive (false); //Could add animation
		this.ec.prevHealth = 3;
		this.ec.StartEventController (this.LevelStartNum);
		this.playerMenu.transform.position = this.playerMenuStartPos;
		this.playerMenu.SetActive (false);
	}

	public void StopGame(){
		this.Title.enabled = true;
		this.StartButton.SetActive (true);
		this.playerMenu.SetActive (true);
	}
}
