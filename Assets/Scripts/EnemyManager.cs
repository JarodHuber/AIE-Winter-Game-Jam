﻿using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Spawn enums
    public enum SpawnStage
    {
        WAITFORWAVEEND,
        PREPAREWAVE,
        SPAWNENEMIES
    }
    [Space(10)]
    public GameObject enemyFab;
    public Bounds spawnBounds;

    public GameObject collectibleFab;

    [Header("Variables to control wave spawning power")]
    // Spawning Speed variables
    [Tooltip("How many initial enemies are spawned, scaler adds on top of this for later levels")]
    public int startCount = 6;
    [Tooltip("How many more enemies are added per wave")]
    public float waveSizeScaler = 1.76f;
    [Tooltip("What fraction of enemies need to remain before next wave spawned, 3 = 1/3 of the enemies, 4 = 1/4 of the enemies, etc.")]
    public int spawnAggression = 3;

    [Header("Do Not Touch, just for monitoring")]
    // Current stage for spawning
    public SpawnStage curStage = SpawnStage.WAITFORWAVEEND;

    // Wave and enemy numbers
    public int waveNum, totalEnemiesForWave, currentNumberOfEnemies;

    // Enemies currently in the scene
    [HideInInspector]
    public List<GameObject> curEnemies = new List<GameObject>();

    // Current Waves
    List<SubWave> Wave = new List<SubWave>();

    Transform player = null;

    #region Pause Vars
    public bool paused = false;
    bool lastVal = false;

    float pauseTime = 0;
    #endregion

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Enemy Update
        for (int x = 0; x < curEnemies.Count; x++)
        {
            EnemUpdate(curEnemies[x].GetComponent<Enemy>());
        }

        if (PauseCheck())
            return;

        // Spawning
        if (curStage == SpawnStage.WAITFORWAVEEND)
            Wait();
        else if (curStage == SpawnStage.PREPAREWAVE)
            GenerateWave();
        else if (curStage == SpawnStage.SPAWNENEMIES)
            SpawnWave();
    }

    void EnemUpdate(Enemy enem)
    {
        if (!enem.attackRate.IsComplete(false))
            enem.attackRate.CountByTime();

        enem.agent.CanMove = enem.agent.path.Length > enem.range / 2;

        if (Vector2.Distance(enem.transform.position, enem.agent.Target.position) < enem.range)
            enem.Shoot();

        EnemSpriteUpdate(enem);

        enem.prevPosition = enem.transform.position;
    }
    void EnemSpriteUpdate(Enemy enem)
    {
        Vector2 travelDir = (Vector2)enem.transform.position - enem.prevPosition;

        if (travelDir == Vector2.zero)
            return;

        float angle = Vector2.Angle(Vector2.up, travelDir);

        if (angle < 30)
        {
            enem.sp.sprite = enem.enemySprites[2];
        }
        else if (angle > 150)
        {
            enem.sp.sprite = enem.enemySprites[0];
        }
        else
        {
            enem.sp.sprite = enem.enemySprites[1];

            angle = Vector2.SignedAngle(Vector2.up, (Vector2)enem.transform.position - enem.prevPosition);
            if (angle < 0)
            {
                enem.sp.flipX = true;
            }
            else if (angle > 0)
            {
                enem.sp.flipX = false;
            }
        }
    }

    bool PauseCheck()
    {
        if (paused && !lastVal)
        {
            pauseTime = Time.time;
            lastVal = true;
        }

        if (!paused && lastVal)
        {
            pauseTime = Time.time - pauseTime;
            lastVal = false;

            if (waveNum > 0)
            {
                foreach (SubWave sw in Wave)
                {
                    print("screeeeeeeeeeeeeeeee");
                    sw.timestamp += pauseTime;
                }
            }
        }

        return paused;
    }

    /// <summary>
    /// Wait for the next wave
    /// </summary>
    void Wait()
    {
        if (currentNumberOfEnemies <= totalEnemiesForWave / spawnAggression)
            curStage = SpawnStage.PREPAREWAVE;
    }

    /// <summary>
    /// Prepare the next wave for spawning
    /// </summary>
    void GenerateWave()
    {
        waveNum++;
        totalEnemiesForWave = currentNumberOfEnemies;

        // Adding enemies to subwaves
        Wave.Add(new SubWave(waveNum, 0f, 0.5f, startCount + (int)(waveSizeScaler * (waveNum - 1))));
        //if (waveNum > 6)
        //    for (int i = 0; i < waveNum / 6; i++)
        //        Wave.Add(new SubWave(waveNum, (i * 3) + 2, 0.75f, 1 + (int)(waveNum / 4.67f)));

        // Preping for end of wave
        foreach (SubWave sw in Wave)
        {
            sw.ResetTimestamp(); //We need this so that the timestamps are correct
            totalEnemiesForWave += sw.amount;
        }

        currentNumberOfEnemies = totalEnemiesForWave;
        curStage = SpawnStage.SPAWNENEMIES;
    }

    /// <summary>
    /// Spawn the enemies
    /// </summary>
    void SpawnWave()
    {
        // Spawns the enmy when it is time
        for (int i = 0; i < Wave.Count; i++)
        {
            if (Wave[i].IsTime())
            {
                if (Wave[i].Spawn())
                {
                    SpawnEnemy();
                }
            }
            if (Wave[i].IsDone()) // Once all enemies in the subwave have spawned
                Wave.Remove(Wave[i]);
        }

        // Once all waves have spawned all their enemies
        if (Wave.Count == 0)
        {
            print("spawning finished"); //Signal the end of the wave spawning
            curStage = SpawnStage.WAITFORWAVEEND;
        }
    }

    void SpawnEnemy()
    {
        GameObject tmpObj = Instantiate(enemyFab, RandomPos(), enemyFab.transform.rotation);
        curEnemies.Add(tmpObj);

        Enemy tmpEnem = tmpObj.GetComponent<Enemy>();
        tmpEnem.agent = tmpObj.GetComponent<SAP2D.SAP2DAgent>();
        tmpEnem.agent.Target = player;
        tmpEnem.sp = tmpObj.GetComponentInChildren<SpriteRenderer>();

        DeOverlap(tmpObj);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        TrySpawnCollectible(enemy.transform.position);
        curEnemies.Remove(enemy);
        currentNumberOfEnemies--;
        Destroy(enemy);
    }

    void TrySpawnCollectible(Vector2 pos)
    {
        if (Random.value > .3f)
            return;

        float val = Random.value;
        Instantiate(collectibleFab, pos, Quaternion.identity).GetComponent<Collectible>().type = (val < .5f) ? CollectibleType.DOUBLEDAMAGE : CollectibleType.HEALTH;
    }

    public void EnemyTakeDamage(Enemy enemy, int damage)
    {
        enemy.health.CountByValue(damage);

        if (enemy.health.IsComplete(false))
        {
            RemoveEnemy(enemy.gameObject);
        }
    }

    /// <summary>
    /// Ensures no collectible is spawned on top of another, or the player
    /// </summary>
    /// <param name="obj">collectible to test</param>
    void DeOverlap(GameObject obj)
    {
        bool locChanged = true;

        for (int x = 0; x < 100 && locChanged; x++)
        {
            locChanged = false;

            //SAP2D.SAP2DAgent tmpAgent = obj.GetComponent<SAP2D.SAP2DAgent>();
            //tmpAgent.StartCoroutine(tmpAgent.FindPath());

            if (/*(obj.GetComponent<SAP2D.SAP2DAgent>().path == null) ||*/
                Vector3.Distance(obj.transform.position, player.position) < 15)
            {
                obj.transform.position = RandomPos();
                locChanged = true;
                continue;
            }

            for (int y = 0; y < curEnemies.Count; y++)
            {
                if (obj == curEnemies[y])
                    continue;

                if (Vector3.Distance(obj.transform.position, curEnemies[y].transform.position) < 2)
                {
                    obj.transform.position = RandomPos();
                    locChanged = true;
                    break;
                }
            }
        }
    }

    // simple code to make things look nicer
    Vector2 RandomPos()
    {
        return spawnBounds.ClosestPoint(((Vector2)spawnBounds.center + Random.insideUnitCircle.normalized) * Random.Range(0f, spawnBounds.extents.magnitude - 5));
    }
}

class SubWave
{
    public int wave;

    private bool isStarted = false;

    public int amount;

    private float time;
    private float spacing;
    public float timestamp;

    public SubWave(int wave, float time, float spacing, int amount)
    {
        this.wave = wave;
        this.time = time;
        this.spacing = spacing;
        this.amount = amount;

        this.timestamp = (float)Time.time;
    }

    /// <summary>
    /// Spawn the next enemy
    /// </summary>
    /// <returns>Returns true when its time to spawn another enemy</returns>
    public bool Spawn()
    {
        if (amount == 0)
            return false;

        if (timestamp + (spacing) < (float)Time.time)
        {
            timestamp = (float)Time.time;
            amount--;
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Tell when it's time to spawn the next Subwave
    /// </summary>
    /// <returns>Returns true when next Subwave is ready to start spawning</returns>
    public bool IsTime()
    {
        if (isStarted)
            return true;

        if (timestamp + (time) < (float)Time.time)
        {
            isStarted = true;
            timestamp = (float)Time.time - spacing; //Minus spacing so they spawn right away instead of with a delay
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Tell when Subwave is done spawning
    /// </summary>
    /// <returns>Returns true when there are no more enemies to spawn in the Subwave</returns>
    public bool IsDone()
    {
        if (amount == 0) return true;
        else return false;
    }

    /// <summary>
    /// sets the times to spawn the enemies in the Subwave
    /// </summary>
    /// <returns>Returns a list of EnemTime to determine spawn time</returns>
    public List<float> GetEnemies()
    {
        List<float> g = new List<float>();
        for (int i = 0; i < amount; i++)
            g.Add(time + (i * spacing));

        return g;
    }

    /// <summary>
    /// resets the enemy timestamp
    /// </summary>
    public void ResetTimestamp()
    {
        timestamp = (float)Time.time;
    }
}
