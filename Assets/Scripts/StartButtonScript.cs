using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonScript : MonoBehaviour {
	GameController gc;

	// Use this for initialization
	void Start () {
		this.gc = GetComponentInParent<GameController> ();
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.layer == 10) {//player shot
			this.gc.launch = true;
			other.gameObject.GetComponent<BulletScript> ().Hit ();
		}
	}
}
