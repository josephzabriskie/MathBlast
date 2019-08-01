using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldCharScript : MonoBehaviour {
	public string heldValue;
	SpriteRenderer sr;

	void Start(){
		this.sr = GetComponent<SpriteRenderer> ();
		UpdateChar(heldValue);
	}

	public void UpdateChar(string newVal){
		heldValue = newVal;
		if(sr != null){
			sr.sprite = CharacterManager.instance.getCharaterSprite(this.heldValue);
		}
	}
}
