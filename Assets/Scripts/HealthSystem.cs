using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour {
	EventController ec;
	GameController gc;
	UICharSystem uics;

	private int maxHealth = 3;
	public int startHealth = 3;
	int curHealth;
	public AudioClip hitSound;
	AudioSource audioS;

	public bool Invincible = true; //for testing cuz I suck

	// Use this for initialization
	void Start () {
		this.ec = GetComponentInParent<EventController> ();
		this.uics = ec.GetComponent<UICharSystem> ();
		this.curHealth = this.startHealth;
		this.audioS = GetComponent<AudioSource> ();
		this.uics.updateHealthBar (this.curHealth);
	}

	public int GetHealth(){
		return this.curHealth;
	}

	void changeHealth(int change){
		this.curHealth += change;
		if (this.curHealth < 0) {
			this.curHealth = 0;
		}
		else if (this.curHealth > this.maxHealth) {
			this.curHealth = this.maxHealth;
		}
		//if (this.Invincible)
		//	this.curHealth = this.maxHealth;
		this.uics.updateHealthBar (this.curHealth);
		if (this.curHealth <= 0) { //We ded
			this.ec.StopEventController(false);
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.layer == 12){ //current layer for enemy bullet
			this.audioS.PlayOneShot(this.hitSound);
			changeHealth(-1);
			other.gameObject.GetComponent<BulletScript>().Hit();
		}
	}

	void OnCollisionEnter2D (Collision2D coll){
		if (coll.collider.gameObject.layer == 11){ //current layer for enemy
			changeHealth(-this.maxHealth); //Instakill
		}
	}
}
