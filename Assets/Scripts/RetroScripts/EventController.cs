using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour {
	public static EventController instance; // singletonio

	public GameObject gamecont;
	public GameObject[] wavelist;
	GameObject currentWave = null;
	WaveScript currentWaveScript;
	public int startWaveNum = 0;
	public int controllerWaveNum = 0;

	//Save Char Health
	public int prevHealth = 3;

	public AudioClip spawnSound;
	public AudioClip loseSound;
	public AudioClip winSound;
	AudioSource audioS;

	bool start = false;

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
		this.audioS = this.GetComponent<AudioSource> ();
		//UICharSystem.instance.goalText.text = "Welcome!\n Move with arrow keys, fire with 'f'";
	}

	public void StartEventController(){
		this.start = true;
		this.controllerWaveNum = startWaveNum;
		Spawn (this.controllerWaveNum);
	}

	public void StopEventController(bool gamewon){
		if (gamewon) {
			UICharSystem.instance.goalText.text = string.Format("You WON! You beat all {0} levels", this.wavelist.Length);
			this.audioS.PlayOneShot (this.winSound);
		}
		else {
			UICharSystem.instance.goalText.text = string.Format ("Nice try! You got to level {0}", this.controllerWaveNum);
			this.audioS.PlayOneShot (this.loseSound);
		}
		this.start = false;
		if (this.currentWave != null) {
			Despawn ();
		}
		Debug.Log ("StopEventController: Stopping Game");
		GameController.instance.StopGame();
	}

	void Update(){
		if (!this.start)
			return;
		if (this.wavelist.Length == 0) {
			Debug.Log ("Yo, we don't have any waves loaded in Event Controller, exit update early");
			return;
		}
		if (this.currentWave == null) {
			Spawn (this.controllerWaveNum);
		}
		int wavesuccess = this.currentWaveScript.isWaveDone ();
		if (wavesuccess != 0) { // Non zero means that we ended the wave! Destroy old wave
			Despawn ();
			if (wavesuccess == 1) { // Wave was beaten successfully. Spawn next wave
				this.controllerWaveNum++;
				if (this.controllerWaveNum >= this.wavelist.Length) {
					//Player has beaten all of our levels!
					StopEventController (true);
				}
				else { // We've still got more levels, load up the next one
					Spawn (this.controllerWaveNum);
				}
			} else if (wavesuccess == -1) { //Wave was not beaten successfully. Spawn same wave
				Spawn (this.controllerWaveNum);				
			}
		}
	}

	void Spawn(int wave){
		Debug.LogFormat("Spawn wave {0}",wave);
		if (currentWave != null) {
			Debug.Log ("ERROR: Event Controller Spawn()ed when currentwave gameobject wasn't null, don't expect this to happen. Make sure to despawn before spawning");
			return;
		}
		//this.currentWave = Instantiate (this.wavelist[wave], this.transform.position, this.transform.rotation);
		this.currentWave = Instantiate (this.wavelist[wave], this.transform, true);
		EnemyScript[] enemies = currentWave.GetComponents<EnemyScript>();
		foreach(EnemyScript e in enemies){
			Debug.LogFormat("{0}: hpmax = {1}",e.name, e.healthMax);
		}
		this.currentWaveScript = this.currentWave.GetComponent<WaveScript> ();
		this.currentWaveScript.startHP = this.prevHealth;
		UICharSystem.instance.updateHealthBar(this.prevHealth);
		//Debug.Log(string.Format("set HP {0}", this.prevHealth));
		UICharSystem.instance.currLevel.text = (wave + 1).ToString ();
		this.audioS.PlayOneShot(this.spawnSound);
	}

	void Despawn(){
		if (currentWave == null)
			Debug.LogWarning ("Event Controller Despawn() trying to despawn a null gameobject. We'll see how it goes");
		this.prevHealth = this.currentWaveScript.GetPlayerHealth ();
		//Debug.Log(string.Format("Last HP {0}", this.prevHealth));
		Destroy (this.currentWave);
		this.currentWave = null;
	}

}
