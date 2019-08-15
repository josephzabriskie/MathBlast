using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMathTable : MonoBehaviour
{
    UIGoalElement[] uiGoalElts;

    void Awake()
    {
        uiGoalElts = GetComponentsInChildren<UIGoalElement>();
    }

    public void SetQuestion(Question ques){
        //For now hardcode the parts of the quest to the correct elt
        Debug.LogFormat("Setting question {0}", ques.ToString());
        uiGoalElts[0].SetText(ques.val1.ToString());
        uiGoalElts[1].SetText(ques.op);
        uiGoalElts[2].SetText(ques.val2.ToString());
        uiGoalElts[3].SetText("="); // If we ever do equality levels, this shouldn't be hard coded
        uiGoalElts[4].SetText(ques.val3.ToString());
        switch(ques.qt){
        case quesType.Basic:
            uiGoalElts[4].SetText("?");
            uiGoalElts[4].Highlight(true);
            break;
        case quesType.Algebra:
            uiGoalElts[3].SetText("?");
            uiGoalElts[3].Highlight(true);
            break;
        case quesType.Operation:
            uiGoalElts[2].SetText("?");
            uiGoalElts[2].Highlight(true);
            break;
        default:
            Debug.LogErrorFormat("Unhandled question type: {0}", ques.qt);
            break;
        }
    }

    public void Clear(){
        Debug.LogFormat("Clearing Questions");
        uiGoalElts[0].SetText("-");
        uiGoalElts[1].SetText("-");
        uiGoalElts[2].SetText("-");
        uiGoalElts[3].SetText("-");
        uiGoalElts[4].SetText("-");
    }
}
