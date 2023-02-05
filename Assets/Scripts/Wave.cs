using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    public List<WaveEnemy> enemiesToSpawn;

    List<Enemy> aliveEnemies;
    List<List<int>> allSubwavePathIndeces;

    float lastSpawnTime;
    float lastRemovalTime;

    public Wave()
    {
        enemiesToSpawn = new List<WaveEnemy>();
        aliveEnemies = new List<Enemy>();
        allSubwavePathIndeces = new List<List<int>>();
    }

    public void tryToSpawn()
    {
        if (isComplete())
            return;

        removeNullAliveEnemies();

        if (isComplete())
        {
            //Debug.Log("iscomplete");
            WaveManager.instance.completeCurrentWave();

            return;
        }

        if (canSpawnEnemy())
            spawnNextEnemy();        
    }

    public void addEnemy(Enemy newEnemy, Vector3 startPos, List<int> pathIndeces)
    {
        enemiesToSpawn.Add(new WaveEnemy(newEnemy, WaveManager.instance.timeBetweenEnemySpawns, pathIndeces));
    }

    public void addSubwavePathIndeces(List<int> subwavePathIndeces) {
        allSubwavePathIndeces.Add(subwavePathIndeces);
    }

    public List<List<int>> getAllSubwavePathIndeces() {
        return allSubwavePathIndeces;
    }

    bool canSpawnEnemy()
    {
        return !isComplete() && !noMoreEnemiesToSpawn() && Time.time - lastSpawnTime >= enemiesToSpawn[0].timeAfterPrev /*&& !Physics.DrawSphere(enemies[0].enemy.transform.position, .25f*/ /* enemy radius *//*, enemyLayer*//*)*/;
    }

    void spawnNextEnemy()
    {
        Enemy newEnemy = Object.Instantiate(enemiesToSpawn[0].enemy, Vector3.zero, Quaternion.identity);

        List<int> indecesList = new List<int>();
        for (int i = 0; i < enemiesToSpawn[0].pathIndeces.Length; i++)
            indecesList.Add(enemiesToSpawn[0].pathIndeces[i]);

        newEnemy.setPath(indecesList);
        enemiesToSpawn.RemoveAt(0);

        aliveEnemies.Add(newEnemy);

        lastSpawnTime = Time.time;
    }

    bool isComplete()
    {
        return noMoreEnemiesToSpawn() && aliveEnemies.Count <= 0;
    }

    bool noMoreEnemiesToSpawn()
    {
        return enemiesToSpawn.Count <= 0;
    }

    void removeNullAliveEnemies()
    {
        if (Time.time - lastRemovalTime < 1f)
            return;

        if (aliveEnemies == null)
            return;

        for (int i = 0; i < aliveEnemies.Count; i++)
        {
            if (aliveEnemies[i] == null)
            {
                aliveEnemies.RemoveAt(i);
                i--;
            }
        }

        lastRemovalTime = Time.time;
    }
}

public struct WaveEnemy
{
    public Enemy enemy;
    public float timeAfterPrev;
    public int[] pathIndeces;

    public WaveEnemy(Enemy enemy, float timeAfterPrev, List<int> pathIndeces)
    {
        this.enemy = enemy;
        this.timeAfterPrev = timeAfterPrev;
        this.pathIndeces = new int[pathIndeces.Count];
        pathIndeces.CopyTo(this.pathIndeces);
    }
}