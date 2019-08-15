using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionStats{ // Some stats we should collect
    public int maxlvl;
    public int lvlCorrect;
    public int lvlWrong;
    public SessionStats(int maxlvl, int lvlCorrect, int lvlWrong){
        this.maxlvl = maxlvl;
        this.lvlCorrect = lvlCorrect;
        this.lvlWrong = lvlWrong;
    }
    public SessionStats(){
        this.maxlvl = 0;
        this.lvlCorrect = 0;
        this.lvlWrong = 0;
    }

    public float GetWinPct(){
        int total = lvlCorrect + lvlWrong;
        if(total == 0){
            Debug.Log("PCT Returning 0");
            return 0.0f;
        }
        Debug.LogFormat("PCT Returning {0}, {1}/{2}", (float)lvlCorrect/(float)total, lvlCorrect, total);
        return (float)lvlCorrect/(float)total;
    }

    public override string ToString(){
        return string.Format("MaxLvL: {0}, Correct: {1}, Wrong{2}", maxlvl, lvlCorrect, lvlWrong);
    }
}

public class SessionManager : MonoBehaviour
{
    public int lvl {get; private set;}
    LevelFactory levelFactory = new LevelFactory();
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    const int PLAYER_START_HEALTH = 3;
    int lastPlayerHealth;
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
    Transform bulletContainer;
    //UI
    UIMathTable uiMathTable;
    Text uiLevelCounter;
    UILifeBar uiLifeBar;
    UICountDown uICountDown;

    Coroutine waitingEnemyDeadCR;
    SessionStats currentStats;

    void Awake(){
        spawnRect = transform.Find("SpawnRect").GetComponent<RectTransform>();
        levelFactory.SetSpawnArea(spawnRect);
        enemyContainer = transform.Find("Enemies");
        bulletContainer = transform.Find("BulletContainer");
        waitingEnemyDeadCR = null;
    }

    void Start(){
        uiMathTable = GameController.instance.endlessUICanvas.GetComponentInChildren<UIMathTable>();
        uiLevelCounter = GameController.instance.endlessUICanvas.transform.Find("StaticLvLText/CurrentLevelText").GetComponent<Text>();
        uiLifeBar = GameController.instance.endlessUICanvas.GetComponentInChildren<UILifeBar>();
        uICountDown = GameController.instance.endlessUICanvas.GetComponentInChildren<UICountDown>();
    }

    public void StartSession(int level){
        Debug.Log("Starting Session");
        lvl = level;
        lastPlayerHealth = PLAYER_START_HEALTH;
        uiMathTable.Clear();
        currentStats = new SessionStats();
        StartLevel(true);
    }
    
    public void EndSession(){
        CleanupLevel();
        GameController.instance.UpdateStats(currentStats);
        GameController.instance.StopGame();
    }

    void StartLevel(bool generate){
        Debug.Log("Starting Level");
        ResetLevelCtxt(); // First reset the context around the current level
        if(generate){
            LevelParams lp = ComputeParams(lvl);
            currentLevel = levelFactory.GetLevel(lp);
            currentQuestion = currentLevel.questions[0];
        }
        SpawnLevel(currentLevel);
        FreezePlayer(true);
        FreezeEnemiesShoot(true);
        StartCoroutine(StartAndWaitCountdown(2,0));
        uiMathTable.SetQuestion(currentQuestion);
        uiLevelCounter.text = lvl.ToString();
        currentStats.maxlvl = lvl;
    }

    void OnLevelEnd(){
        CleanupLevel();
        if(levelClear){ //Player hit the right answer! Increment
            lvl++;
            currentStats.lvlCorrect += 1;
        }
        else{
            currentStats.lvlWrong += 1;
        }
        StartLevel(levelClear); //Start level. Either current or next
    }

    void OnPlayerKill(PlayerController pc){
        Debug.Log("Player was killed, end session");
        EndSession();
    }

    void OnPlayerDamage(PlayerController pc){
        Debug.Log("Player took damage");
        uiLifeBar.SetHealth(pc.health);
        lastPlayerHealth = pc.health;
    }

    void CleanupLevel(){
        //Remove remaining enemies
        foreach(EnemyScript es in enemyContainer.GetComponentsInChildren<EnemyScript>()){
            Destroy(es.gameObject);
        }
        BulletScript[] bullets = GetComponentsInChildren<BulletScript>();
        foreach(BulletScript bs in bullets){
            Destroy(bs.gameObject);
        }
        Destroy(currentPlayer); // And remove player, level instantiate creates player
    }

    void ResetLevelCtxt(){
        if(currentEnemies!= null){
            foreach(GameObject go in currentEnemies){
                Destroy(go);
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
        Debug.Log("Spawning Level");
        currentPlayer = Instantiate(playerPrefab, playerPrefab.transform.position, playerPrefab.transform.rotation, this.transform);
        PlayerController pc = currentPlayer.GetComponent<PlayerController>();
        pc.OnKillCB = OnPlayerKill;
        pc.OnDamageCB = OnPlayerDamage;
        pc.SetHealth(lastPlayerHealth);
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
            es.SetBulletParent(bulletContainer);
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
        if(currentEnemies.Count == 0 && waitingEnemyDeadCR == null){
            Debug.Log("that was the last enemy, get ready to end level");
            waitingEnemyDeadCR = StartCoroutine(waitEnemiesDead(2.0f));
        }
        else{
            Debug.LogFormat("We think there's enemies left: {0}", currentEnemies.Count);
        }
    }

    IEnumerator waitEnemiesDead(float time){
        float endtime = Time.time + time;
        while(enemyContainer.gameObject.GetComponentsInChildren<EnemyScript>().Length != 0 && Time.time < endtime){
            yield return null;
        }
        Debug.Log("All Enemies are gone, or we timed out");
        waitingEnemyDeadCR = null;
        OnLevelEnd();
    }

    LevelParams ComputeParams(int level){
        List<mathOp> mo = new List<mathOp>(){mathOp.Add, mathOp.Subtract, mathOp.Multiply}; // Hard code allow all right now
        List<quesType> qt = new List<quesType>(){quesType.Basic}; // only basic for now
        bool bonuslvl = false;
        int bonuscount = 0;
        Vector2Int shiprange = new Vector2Int(2,5);
        Vector2 speedR= new Vector2(0.4f, 1.5f);
        Vector2 shotDelayR = new Vector2(0.5f, 1.5f);
        float bulletSpeed = 1.0f;
        LevelParams lp = new LevelParams(mo, qt, bonuslvl, bonuscount, shiprange, speedR, shotDelayR, bulletSpeed);
        return lp;
    }

    //Level Controlling functions----------------
    IEnumerator StartAndWaitCountdown(int start, int end){
        uICountDown.Restart(start, end);
        yield return uICountDown.WaitForDone();
        FreezePlayer(false);
        FreezeEnemiesShoot(false);
    }

    void FreezePlayer(bool freeze){
        Debug.LogFormat("Setting player freeze to: {0}", freeze);
        PlayerController pc = currentPlayer.GetComponent<PlayerController>();
        pc.allowMove = !freeze;
        pc.allowShoot = !freeze;
    }

    void FreezeEnemiesShoot(bool freeze){
        Debug.LogFormat("Setting Enemies freeze shoot to: {0}", freeze);
        foreach(GameObject e in currentEnemies){
            EnemyScript es = e.GetComponent<EnemyScript>();
            es.allowShoot = !freeze;
        }
    }
    void FreezeEnemiesMove(bool freeze){
        Debug.LogError("Not yet implemented in enemy script");
    }
}
