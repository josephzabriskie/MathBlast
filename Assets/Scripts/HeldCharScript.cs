using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldCharScript : MonoBehaviour {
	public string heldValue;
	SpriteRenderer sr;

	void Start(){
		this.sr = GetComponent<SpriteRenderer> ();
		Sprite s = UICharSystem.instance.cm.getCharaterSprite (this.heldValue);
		sr.sprite = s;
	}
}
