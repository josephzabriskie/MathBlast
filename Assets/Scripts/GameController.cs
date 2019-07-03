using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public static GameController instance; // singletonio

	public GameObject playerMenu;
	public GameObject RetroButton;
	public GameObject EndlessButton;
	public Image Title;
	//Vector3 playerMenuStartPos;

	void Awake(){
		if(instance == null){
			instance = this;
		}
		else if(instance != this){
			Debug.LogErrorFormat("Singleton {0} instantiated multiple times, destroy all but first one up",this.GetType().Name);
			Destroy(this);
		}
	}

	void Start () {
		//this.playerMenuStartPos = this.playerMenu.transform.position;
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)){
			Application.Quit();
		}
	}

	public void DelayedStart(bool retro){
		StartCoroutine(IEDelayedStart(retro));
	}

	IEnumerator IEDelayedStart(bool retro){
		yield return new WaitForSeconds(1.5f);
		if(retro){
			StartRetroGame();
		}
		else{
			StartGame();
		}
	}

	void StartGame(){

	}

	//Retro game is the original game jam version
	void StartRetroGame(){
		this.Title.enabled = false; // Could fade out
		this.RetroButton.SetActive (false); //Could add animation
		this.EndlessButton.SetActive (false); //Could add animation
		EventController.instance.prevHealth = 3;
		EventController.instance.StartEventController();
		//this.playerMenu.transform.position = this.playerMenuStartPos;
		this.playerMenu.SetActive (false);
	}

	public void StopGame(){
		this.Title.enabled = true;
		this.RetroButton.SetActive (true);
		this.EndlessButton.SetActive (true);
		this.playerMenu.SetActive (true);
	}
}
