using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {
	public Sprite[] numerals;
	public Sprite ques;
	public Sprite plus;
	public Sprite minus;
	public Sprite mult;
	public Sprite eq;
	public Sprite highlight;
	public Sprite err;
	public Sprite noteq;
	//public Sprite[] letters;

	void Start(){
		//getCharaterSprite ("1");
	}

	public Sprite getCharaterSprite(string s){
		int x = 0;
		if (int.TryParse (s, out x)) {
			//We got a number, now what sprite is it?
			if (x <= this.numerals.Length && x > -1) {
				return this.numerals [x];
			} else {
				Debug.Log (string.Format ("getCharacterSprite(): got a number, but it's not one we think we can handle: {0}", x));
			}
		} else if (s == "?")
			return this.ques;
		else if (s == "+")
			return this.plus;
		else if (s == "-")
			return this.minus;
		else if (s == "*")
			return this.mult;
		else if (s == "=")
			return this.eq;
		else if (s == "!=")
			return this.noteq;
		//else err
		return this.err;
	}
}
