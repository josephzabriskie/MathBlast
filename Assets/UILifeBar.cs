using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILifeBar : MonoBehaviour
{
    public int maxLife {get; private set;}
    public int currentLife {get; private set;}
    public Sprite healthySprite;
    public Sprite damagedSprite;
    Image[] images;

    void Awake()
    {
        images = GetComponentsInChildren<Image>();
        maxLife = images.Length;
    }

    public void SetHealth(int health){
        for(int i = 0; i < maxLife; i++){
            if (i + 1 <= health){
                images[i].sprite = healthySprite;
            }
            else{
                images[i].sprite = damagedSprite;
            }
        }
    }
}
