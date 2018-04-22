using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldCharScript : MonoBehaviour {
	public string heldValue;
	CharacterManager cm;
	SpriteRenderer sr;

	void Start(){
		this.sr = GetComponent<SpriteRenderer> ();
		this.cm = GameObject.Find ("EventController").GetComponent<CharacterManager> ();
		Sprite s = this.cm.getCharaterSprite (this.heldValue);
		sr.sprite = s;
	}
}
