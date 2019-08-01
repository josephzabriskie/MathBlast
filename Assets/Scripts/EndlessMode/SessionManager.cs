using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionStats{ // Some stats we should collect
    int maxlvl;
    int lvlCorrect;
    int lvlWrong;
    int shotsFired;
    int damage;
}

public class SessionManager : MonoBehaviour
{
    public int lvl {get; private set;}
    LevelFactory lf = new LevelFactory();
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    RectTransform spawnRect;

    //Context for currently playing level
    bool firstHit = false;
    string firstVal = "";
    bool levelClear = false;
    List<GameObject> currentEnemies = null;
    GameObject currentPlayer = null;
    LevelData currentLevel;
    Question currentQuestion;
    Transform enemyContainer;

    Coroutine waitingEnemyDeadCR;

    void Awake(){
        spawnRect = transform.Find("SpawnRect").GetComponent<RectTransform>();
        lf.SetSpawnArea(spawnRect);
        enemyContainer = transform.Find("Enemies");
        waitingEnemyDeadCR = null;
    }

    public void StartSession(int level){
        lvl = level;
        StartLevel();
    }
    
    public void EndSession(){
        CleanupLevel();
        GameController.instance.StopGame();
    }

    void StartLevel(){
        ResetLevelCtxt(); // First reset the context around the current level
        LevelParams lp = ComputeParams(lvl);
        LevelData ld = lf.GetLevel(lp);
        currentLevel = ld;
        currentQuestion = ld.questions[0];
        SpawnLevel(ld);
    }

    void OnLevelEnd(){
        CleanupLevel();
        if(levelClear){ //Player hit the right answer! Increment
            lvl++;
        }
        StartLevel(); //Start level. Either current or next
    }

    void OnPlayerKill(PlayerController pc){
        Debug.Log("Player was killed, end session");
        EndSession();
    }

    void CleanupLevel(){
        //Remove remaining enemies
        foreach(EnemyScript es in enemyContainer.GetComponentsInChildren<EnemyScript>()){
            Destroy(es.gameObject);
        }
        Destroy(currentPlayer); // And remove player, level instantiate creates player
    }

    void ResetLevelCtxt(){
        if(currentEnemies!= null){
            foreach(GameObject go in currentEnemies){
                Destroy(gameObject);
            }
            currentEnemies.Clear();
        }
        currentEnemies = new List<GameObject>();
        currentPlayer = null;
        firstHit = false;
        firstVal = "";
        levelClear = false;
    }

    void SpawnLevel(LevelData ld){
        currentPlayer = Instantiate(playerPrefab, playerPrefab.transform.position, playerPrefab.transform.rotation, this.transform);
        PlayerController pc = currentPlayer.GetComponent<PlayerController>();
        pc.OnKillCB = OnPlayerKill;
        if(currentEnemies.Count != 0){
            Debug.LogErrorFormat("Enemies list should be empty. Count: {0}", currentEnemies.Count);
            currentEnemies.Clear();
        }
        foreach(EnemyConfig cfg in ld.enemies){
            GameObject go = Instantiate(enemyPrefab, cfg.position, Quaternion.identity, this.enemyContainer);
            EnemyScript es = go.GetComponent<EnemyScript>();
            es.ApplyConfig(cfg);
            es.OnKillCB = EnemyKillCB;
            es.SetTargetGO(currentPlayer);
            currentEnemies.Add(go);
        }
    }

    void EnemyKillCB(GameObject go, string val){
        if(!firstHit){
            firstVal = val;
            firstHit = true;
            if(firstVal == currentQuestion.ans){
                levelClear = true; // They did it, got the right answer
                Debug.LogFormat("Got the right answer! {0}", firstVal);
            }
            else{
                //Bad failure poor job darn it
                Debug.LogFormat("Got the wrong answer! {0}!={1}", firstVal, currentQuestion.ans);
            }
        }
        //Remove from our current gameobject list
        currentEnemies.Remove(go);
        if(currentEnemies.Count == 0 && waitingEnemyDeadCR != null){
            Debug.Log("that was the last enemy, get ready to end level");
            waitingEnemyDeadCR = StartCoroutine(waitEnemiesDead(2.0f));
        }
    }

    IEnumerator waitEnemiesDead(float time){
        float endtime = Time.time + time;
        while(enemyContainer.gameObject.GetComponentsInChildren<EnemyScript>().Length != 0 && Time.time < endtime){
            yield return null;
        }
        Debug.Log("All Enemies are gone, or we timed out");
        OnLevelEnd();
    }

    LevelParams ComputeParams(int level){
        List<mathOp> mo = new List<mathOp>(){mathOp.Add, mathOp.Subtract, mathOp.Multiply}; // Hard code allow all right now
        List<quesType> qt = new List<quesType>(){quesType.Basic}; // only basic for now
        bool bonuslvl = false;
        int bonuscount = 0;
        Vector2Int shiprange = new Vector2Int(2,5);
        Vector2 speedR= new Vector2(2.0f, 3.0f);
        Vector2 fireRateR = new Vector2(2.0f, 3.0f);
        float bulletSpeed = 1.0f;
        LevelParams lp = new LevelParams(mo, qt, bonuslvl, bonuscount, shiprange, speedR, fireRateR, bulletSpeed);
        return lp;
    }
}
