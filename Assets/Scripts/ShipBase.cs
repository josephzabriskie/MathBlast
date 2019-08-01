using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipBase : MonoBehaviour {
    public int healthMax = 1;
    public int health { get; private set; }
    public bool invincible = false;

    protected virtual void Awake(){
        SetHealth(healthMax);
    }

    //Health won't ever be set above max, can go to negative
    public void DoDamage(int amount){
        if(!invincible){
            SetHealth(health - amount);
        }
        OnDamage();
    }

    public void HealDamage(int amount){
        SetHealth(health + amount);
        OnHeal();
    }

    public void SetHealth(int amount){
        health = amount < healthMax ? amount : healthMax;
        if(health <= 0){
            OnKill();
        }
    }

    public abstract void OnDamage();
    public abstract void OnKill();
    public abstract void OnHeal();
}
