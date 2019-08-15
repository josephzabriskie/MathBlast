using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICountDown : MonoBehaviour
{
    public int startNum;
    public int endNum;
    int _currentNum;
    Text txt;
    Animator anim;
    bool done;

    void Awake(){
        txt = GetComponent<Text>();
        anim = GetComponent<Animator>();
        done = true;
    }

    private void _Restart(){
        _currentNum = startNum;
        txt.text = _currentNum.ToString();
        done = false;
        anim.SetBool("Stop", false);
    }

    public void Restart(){
        _Restart();
    }

    public void Restart(int start, int stop){
        startNum = start;
        endNum = stop;
        _Restart();
    }

    public IEnumerator WaitForDone(){
        if(startNum != 0){
            yield return new WaitUntil(() => done);
        }
    }

    public void Decrement(){
        _currentNum--;
        txt.text = _currentNum.ToString();
        if(_currentNum == endNum){
            anim.SetBool("Stop", true);
            done = true;
        }
    }
}
