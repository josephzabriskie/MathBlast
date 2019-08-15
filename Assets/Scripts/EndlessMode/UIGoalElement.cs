using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGoalElement : MonoBehaviour
{
    //This is a simple UI element. Contains a text object and a highlighting sprite toggle
    Text text;
    Image highlight;
    void Awake(){
        text = GetComponentInChildren<Text>();
        highlight = GetComponentInChildren<Image>();
        highlight.enabled = false;
    }

    public void SetText(string text){
        this.text.text = text; // huh
    }

    public void Highlight(bool en){
        highlight.enabled = en;
    }
}
