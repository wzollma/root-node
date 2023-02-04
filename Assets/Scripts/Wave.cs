using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    List<WaveEnemy> enemies;

    float lastSpawnTime;

    void Start()
    {
        enemies = new List<WaveEnemy>();

        // populate enemies list
    }

    void Update()
    {
        if (isComplete())
            return;

        if (canSpawnEnemy())
        {
            spawnNextEnemy();

            if (isComplete())
            {
                WaveManager.instance.completeCurrentWave();

                return;
            }
        }            
    }

    bool canSpawnEnemy()
    {
        return !isComplete() && Time.time - lastSpawnTime >= enemies[0].timeAfterPrev && !Physics.DrawSphere(enemies[0].enemy.transform.position, .25f /* enemy radius *//*, enemyLayer*/);
    }

    void spawnNextEnemy()
    {
        //Instantiate(enemies[0], transform.position, Quaternion.identity);
        enemies[0].enemy.gameObject.enabled = true;
        enemies.RemoveAt(0);

        lastSpawnTime = Time.time;
    }

    bool isComplete()
    {
        return enemies.Count <= 0;
    }
}

public struct WaveEnemy
{
    public Enemy enemy;
    public float timeAfterPrev;

    public WaveEnemy(Enemy enemy, float timeAfterPrev)
    {
        this.enemy = enemy;
        this.timeAfterPrev = timeAfterPrev;
    }
}