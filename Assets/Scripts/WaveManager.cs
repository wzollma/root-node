using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [SerializeField] float startDifficulty;
    [SerializeField] Enemy allEnemies; // probably should be ordered in ascending difficulty (or order of when we want them introduced)

    /*const*/
    [SerializeField] float TIME_BETWEEN_WAVES = 4;
    [SerializeField] int wavesUntilBossWave = 10;
    [SerializeField] float difficultyMultiplier = 1.2f;
    [SerializeField] float bossWaveDifficultyMult = 2;
    [SerializeField] float bossWaveDifficultyMult = 2;

    int waveNum;
    Wave curWave;
    float curDifficulty;
    float lastTimeEndedWave;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        waveNum = 1;
        curDifficulty = startDifficulty;
    }

    void Update()
    {
        if (curWave == null && Time.time - lastTimeEndedWave >= TIME_BETWEEN_WAVES)
            createNewWave();
    }

    public void completeCurrentWave()
    {
        curWave = null;
        lastTimeEndedWave = Time.time;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="waveSize"></param>
    /// <param name="enemyWindow">Inclusive x and y</param>
    /// <param name="difficultyScoreTotal"></param>
    /// <returns></returns>
    Wave createNewWave()
    {
        Vector2Int enemyWindow = calculateEnemyWindow(waveNum);
        float difficultyScoreTotal = curDifficulty = calculateNewDifficultyScore();

        while (true)
        {
            enemyWindow = getNewValidEnemyWindow(enemyWindow);

            // stop spawning if there are no more valid enemies
            if (enemyWindow.y < enemyWindow.x)
                break;

            Enemy newEnemy = getRandomEnemyFromWindow(enemyWindow, difficultyScoreLeft);

            difficultyScoreTotal -= newEnemy.getDifficulty();
        }

        waveNum++;
    }

    Enemy getRandomEnemyFromWindow(Vector2Int enemyWindow, float difficultyScoreLeft)
    {
        return allEnemies[Random.Range(enemyWindow.x, enemyWindow.y + 1)];
    }

    Vector2Int getNewValidEnemyWindow(Vector2Int enemyWindow)
    {
        // scans through window, removing indeces that are too expensive
        for (int i = enemyWindow.y; i >= enemyWindow.x; i--)
        {
            if (allEnemies[i].getDifficulty() > difficultyScoreLeft)
                enemyWindow.y--;
        }

        if (enemyWindow.y < enemyWindow.x && enemyWindow.x > 0)
            enemyWindow.x = 0;

        return enemyWindow;
    }

    Vector2Int calculateEnemyWindow(int waveNum)
    {
        int deterministicMin = waveNum / wavesUntilBossWave;
        int min = deterministicMin + Random.Range(-1, 2);
        int deterministicMax = ((waveNum - 1) / 2);
        int max = deterministicMax + Random.Range(-1, 1);

        if (min < 0)
            min = 0;
        else if (min > deterministicMax)
            min = deterministicMax - 1;

        if (max < min)
            max = min;
    }

    float calculateNewDifficultyScore(int waveNum)
    {
        float extraMultiplier;
        int wavesFromBossWave = (waveNum - 1) % wavesUntilBossWave;

        if (wavesFromBossWave == 0)
            extraMultiplier = difficultyMultiplier;
        else if (wavesFromBossWave == wavesUntilBossWave - 1)
            extraMultiplier = -difficultyMultiplier;
        else
            extraMultiplier = 0;

        return (Mathf.Pow(difficultyMultiplier, waveNum - 1) * (difficultyMultiplier + extraMultiplier);        
    }
}