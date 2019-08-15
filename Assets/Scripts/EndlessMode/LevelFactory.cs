using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct LevelData{
    public List<EnemyConfig> enemies;
    public List<Question> questions; // list since bonus levels have multiple questions
    public LevelData(List<EnemyConfig> enemies, List<Question> questions){
        this.enemies = enemies;
        this.questions = questions;
    }
}

public struct Question{
    public string val1;
    public string val2;
    public string op;
    public string val3;
    public mathOp mo;
    public quesType qt;
    public string question;
    public string ans;
    public Question(string val1, string op, string val2, string val3, mathOp mo, quesType qt, string q){
        this.val1 = val1;
        this.val2 = val2;
        this.op = op;
        this.val3 = val3;
        this.mo = mo;
        this.qt = qt;
        this.question = q;
        switch(qt){
        case quesType.Basic:
            ans = val3.ToString();
            break;
        case quesType.Algebra:
            ans = val2.ToString();
            break;
        case quesType.Operation:
            ans = op;
            break;
        default:
            ans = "Error";
            Debug.LogErrorFormat("Unhandled question type: {0}", qt);
            break;
        }
    }
    public override string ToString(){
        return question;
    }
}

public enum mathOp{
    Add,
    Subtract,
    Multiply,
    //Divide // add later
}

public enum quesType{
    Basic,
    Algebra,
    Operation
}

public struct LevelParams{
    public List<mathOp> useOps; // need at least 1
    public List<quesType> quesTypes; // Need  at least 1
    public bool bonuslvl; // if this should be a bonus level
    public int bonusNum; // number of bonus levels to return
    //Difficulty parameters
    public Vector2Int shipCountRange;
    public Vector2 speedRange;
    public Vector2 shotDelayRange;
    public float bulletSpeed;
    public LevelParams(List<mathOp> mo, List<quesType> qt, bool bonuslvl, int bonusNum, Vector2Int shipCountR, Vector2 speedR, Vector2 shotDelayR, float bulletSpeed){
        useOps = mo;
        quesTypes = qt;
        this.bonuslvl = bonuslvl;
        this.bonusNum = bonusNum;
        shipCountRange = shipCountR;
        speedRange = speedR;
        shotDelayRange = shotDelayR;
        this.bulletSpeed = bulletSpeed;
    }
} 

public class LevelFactory{    

    const int MINNUM = -9;
    const int MAXNUM = 9;
    const int MAXROWS = 3;
    float[] rowPos = {0f, 1f, 2f};
    float width = 10f;
    Vector3 posOffset = new Vector3(0f,0f,0f);
    //Fake answer range (should be changed in the future)
    const int FAKEMIN = -9;
    const int FAKEMAX = 9;

    readonly Dictionary<mathOp,string> opMap= new Dictionary<mathOp,string>(){
        {mathOp.Add, "+"},
        {mathOp.Subtract, "-"},
        {mathOp.Multiply, "x"}
    };

    public LevelData GetLevel(LevelParams lp){
        List<Question> qs = GetQuestions(lp);
        List<EnemyConfig> ecs = GetEnemies(lp, qs);
        LevelData outlvl = new LevelData(ecs, qs);
        return outlvl;
    }

    public void SetSpawnArea(RectTransform r){
        width =  r.rect.width;
        // Debug.LogFormat("Width set to {0}", width);
        // Debug.LogFormat("height is {0}", r.rect.height);
        for(int i = 0; i < MAXROWS; i++){
            rowPos[i] = (i + 1) * r.rect.height/((float)MAXROWS+1.0f);
            // Debug.LogFormat("Set row {0} to {1}", i, rowPos[i]);
        }
        posOffset = r.position;
    }

    //------------Generate Enemies from parameters
    List<EnemyConfig> GetEnemies(LevelParams lp, List<Question> qs){
        List<EnemyConfig> outCfg = new List<EnemyConfig>();
        // pick random number of enemy ships between specified level params, making sure we have enough to cover all questions
        int shipcount = Random.Range(Mathf.Max(lp.shipCountRange.x, qs.Count), Mathf.Max(lp.shipCountRange.y, qs.Count));
        int row = MAXROWS - 1; // The row selection should be random, but starting from back for now
        if(shipcount > 9 || shipcount < 2){
            Debug.LogWarningFormat("I'm not prepared to handle only {0} ship(s)", shipcount);
            return outCfg;
        }
        while(shipcount != 0 || row >= 0){
            int s = Random.Range(1,4); // pick number between 1 and 3 for # ships per row
            if(shipcount < s){
                s = shipcount;
                shipcount = 0;
            }
            else{
                shipcount -= s;
            }
            float spacing = width/(s+1);
            float startx = spacing - width/2f;
            for(int i = 0; i < s; i++){
                MovementType mt = (MovementType)Random.Range(0, System.Enum.GetNames(typeof(MovementType)).Length); // random movement type
                List<ShootType> availAttacks = System.Enum.GetValues(typeof(ShootType)).Cast<ShootType>().ToList(); // list of all actions at first
                switch(mt){
                case MovementType.Static:
                    availAttacks.Remove(ShootType.None);
                    break;
                case MovementType.Horizontal:
                    availAttacks.Remove(ShootType.CircleBurst);
                    break;
                case MovementType.Circle:
                    availAttacks.Remove(ShootType.CircleBurst);
                    break;
                }

                ShootType at = availAttacks[Random.Range(0, availAttacks.Count)];
                float shotDelay = Random.Range(lp.shotDelayRange.x, lp.shotDelayRange.y);
                float moveSpeed = Random.Range(lp.speedRange.x, lp.speedRange.y);
                float bulletSpeed = lp.bulletSpeed;
                int bulletCount = 0;
                if(at == ShootType.CircleBurst){ // if we're a circle burst, we need to set addtl parameters
                    bulletCount = 12;
                    bulletSpeed = bulletSpeed/2.5f; //Reduce the speed here, it's pretty wacky if too fast
                    shotDelay = shotDelay*2.0f; //Also so many bullets, slow em down
                }
                Debug.LogFormat("Row: {0}", row);
                Vector3 pos = new Vector3(startx + spacing * i, rowPos[row], 0f) - posOffset;
                string ans = Random.Range(FAKEMIN, FAKEMAX).ToString(); // Get fake ans (ok if randomly matches right ans for now)
                outCfg.Add(new EnemyConfig(mt, at, pos, shotDelay, moveSpeed, bulletSpeed, ans, bulletCount));
            }
            row--;
        }
        //Now that you've got the ships in the correct positions, add the held values
        if(qs.Count > outCfg.Count){
            Debug.LogErrorFormat("Somehow we have more questions than ships to put ans on? {0} > {1}", qs.Count, outCfg.Count);
        }

        List<int> posAvail = Enumerable.Range(0, outCfg.Count).ToList(); // get list of places where we could put answers so we don't double up
        foreach(Question q in qs){
            int pos = Random.Range(0, posAvail.Count);
            int idx = posAvail[pos];
            posAvail.RemoveAt(pos);
            EnemyConfig cfg = outCfg[idx];
            outCfg[idx] = new EnemyConfig(cfg, q.ans); // update cfg with new answer, this is a pretty kludgey way to get around struct immutability 
        }
        return outCfg;
    }

    //------------Generate math questions
    List<Question> GetQuestions(LevelParams lp){
        List<Question> outputQues = new List<Question>();
        if(!lp.bonuslvl){ //make single question
            outputQues.Add(MakeQuestion(lp.useOps, lp.quesTypes));
        }
        else{
            Debug.LogWarning("Not ready to make bonus levels yet");
        }
        return outputQues;
    }

    Question MakeQuestion(List<mathOp> ops, List<quesType> types){
        quesType qt = types[Random.Range(0, types.Count)];
        mathOp mo = ops[Random.Range(0, types.Count)];
        int val1 = Random.Range(MINNUM, MAXNUM+1);
        int val2 = Random.Range(MINNUM, MAXNUM+1);
        int val3 = -23; // Poor choice of error value, but rolling with it. TODO
        string sol = "Error"; // Correct solution that a ship will carry
        switch(mo){
        case mathOp.Add:
            val3 = val1+val2;
            break;
        case mathOp.Subtract:
            val3 = val1-val2;
            break;
        case mathOp.Multiply:
            val3 = val1*val2;
            break;
        default:
            Debug.LogErrorFormat("Unhandled math operator: {0}", mo);
            break;
        }
        string quesStr = "Error";
        switch(qt){
        case quesType.Basic:
            quesStr = string.Format("{0}{1}{2}=?",val1, opMap[mo], val2);
            sol = val3.ToString();
            break;
        case quesType.Algebra:
            quesStr = string.Format("{0}{1}?={2}",val1, opMap[mo],val3);
            sol = val2.ToString();
            break;
        case quesType.Operation:
            quesStr = string.Format("{0}?{1}={2}",val1, val2, val3);
            sol = opMap[mo];
            break;
        default:
            Debug.LogErrorFormat("Unhandled ques type: {0}", qt);
            break;
        }
        Question quesOut = new Question(val1.ToString(), opMap[mo], val2.ToString(), val3.ToString(), mo, qt, quesStr);
        return quesOut;
    }

    /*Math options
    Math type, boolean for each
    Addition,subtracion,multiplication,division,algebra
    */ 
}
