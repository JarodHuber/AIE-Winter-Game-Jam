using System.Collections.Generic;
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

    public GameObject enemyFab;
    public Bounds spawnBounds;

    // Spawning Speed variables
    public int startCount = 6;
    public float waveSizeScaler = 1.76f;
    public int spawnAggression = 3;

    [Space(15)]

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
        for (int x = 0; x < curEnemies.Count; x++)
        {
            Enemy enem = curEnemies[x].GetComponent<Enemy>();

            if (!enem.attackRate.IsComplete(false))
                enem.attackRate.CountByTime();

            enem.agent.CanMove = enem.agent.path.Length > enem.range / 2;

            if (Vector2.Distance(enem.transform.position, enem.agent.Target.position) < enem.range)
                enem.Shoot();
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
                    GameObject tmpObj = Instantiate(enemyFab, RandomPos(), enemyFab.transform.rotation);
                    curEnemies.Add(tmpObj);

                    Enemy tmpEnem = tmpObj.GetComponent<Enemy>();
                    tmpEnem.agent = tmpObj.GetComponent<SAP2D.SAP2DAgent>();
                    tmpEnem.agent.Target = player;

                    DeOverlap(tmpObj);

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

    public void RemoveEnemy(GameObject enemy)
    {
        curEnemies.Remove(enemy);
        currentNumberOfEnemies--;
        Destroy(enemy);
    }

    public void EnemyTakeDamage(Enemy enemy)
    {
        enemy.health.CountByValue(1);

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
        return spawnBounds.ClosestPoint(((Vector2)spawnBounds.center + Random.insideUnitCircle.normalized) * Random.Range(0f, spawnBounds.max.magnitude));
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
