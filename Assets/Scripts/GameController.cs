using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public static GameController instance; // singletonio

	public GameObject mainMenuCanvas;
	public GameObject retroUICanvas;
	public GameObject endlessUICanvas;
	GameObject playerShipMenu;
	GameObject retroButton;
	GameObject endlessButton;
	SessionManager sm;
	Coroutine gameStart;
	SessionStats stats;

	Text maxLvlTxt;
	Text lastLvlTxt;
	Text prevCorrectTxt;

	void Awake(){
		if(instance == null){
			instance = this;
		}
		else if(instance != this){
			Debug.LogErrorFormat("Singleton {0} instantiated multiple times, destroy all but first one up",this.GetType().Name);
			Destroy(this);
		}
		this.sm = GameObject.FindWithTag("SessionManager").GetComponent<SessionManager>();
		playerShipMenu = transform.Find("PlayerShipMenu").gameObject;
		retroButton = transform.Find("RetroButton").gameObject;
		endlessButton = transform.Find("EndlessButton").gameObject;
		endlessUICanvas.SetActive(false);
		retroUICanvas.SetActive(false);
		stats = new SessionStats();
		//Stats txt objects
		maxLvlTxt = mainMenuCanvas.transform.Find("StatsUI/MaxLvLUI/NumText").GetComponent<Text>();
		maxLvlTxt.text = "--";
		lastLvlTxt = mainMenuCanvas.transform.Find("StatsUI/LastLvLUI/NumText").GetComponent<Text>();
		lastLvlTxt.text = "--";
		prevCorrectTxt = mainMenuCanvas.transform.Find("StatsUI/Correct%UI/NumText").GetComponent<Text>();
		prevCorrectTxt.text = "--";
	}

	void Start () {
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)){
			Application.Quit();
		}
	}

	public void DelayedStart(bool retro){
		if(gameStart != null){
			StopCoroutine(gameStart);
		}
		gameStart = StartCoroutine(IEDelayedStart(retro));
	}

	IEnumerator IEDelayedStart(bool retro){
		yield return new WaitForSeconds(1.5f);
		if(retro){
			StartGameRetro();
		}
		else{
			StartGameEndless();
		}
	}

	void StartGameEndless(){
		UIMainMenuEnable(false);
		endlessUICanvas.SetActive(true);
		sm.StartSession(0);
	}

	//Retro game is the original game jam version
	void StartGameRetro(){
		UIMainMenuEnable(false);
		retroUICanvas.SetActive(true);
		EventController.instance.prevHealth = 3;
		EventController.instance.StartEventController();
	}

	public void StopGame(){
		UIMainMenuEnable(true);
		retroUICanvas.SetActive(false);
		endlessUICanvas.SetActive(false);
	}

	public void UpdateStats(SessionStats inStats){
		if(inStats.maxlvl > stats.maxlvl){
			stats.maxlvl = inStats.maxlvl;
			maxLvlTxt.text = stats.maxlvl.ToString();
		}
		//Now get last percentage
		stats.lvlCorrect = inStats.lvlCorrect;
		stats.lvlWrong = inStats.lvlWrong;
		prevCorrectTxt.text = ((int)(stats.GetWinPct() * 100f)).ToString();
		//And last lvl
		lastLvlTxt.text = inStats.maxlvl.ToString();
	}

	void UIMainMenuEnable(bool enabled){
		this.mainMenuCanvas.SetActive(enabled); // Could fade out
		this.retroButton.SetActive (enabled); //Could add animation
		this.endlessButton.SetActive (enabled); //Could add animation
		this.playerShipMenu.SetActive (enabled); // disable player (poor naming, I know)
	}
}
