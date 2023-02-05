using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public float timeBetweenEnemySpawns = .25f;
    [SerializeField] Enemy[] allEnemies; // probably should be ordered in ascending difficulty (or order of when we want them introduced)
    [SerializeField] LineRenderer lineRendPrefab;

    /*const*/
    [SerializeField] float TIME_BETWEEN_WAVES = 4;
    [SerializeField] float TIME_UNTIL_WAVE_ANIM = 0;
    [SerializeField] float waveAnimSpeed = 32;
    [SerializeField] float timeAnimOnScreen = 5;
    [SerializeField] float waveAnimCooldown = .1f;
    [SerializeField] int wavesUntilBossWave = 10;
    [SerializeField] float difficultyMultiplier = 1.2f;    

    int waveNum;
    Wave curWave;
    float curDifficulty; // just for debugging
    float curWaveSize; // just for debugging;
    float lastTimeEndedWave;

    bool hasShownPaths;
    Wave nextWave;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        waveNum = 1;
        lastTimeEndedWave = -TIME_BETWEEN_WAVES;
    }

    void Update()
    {
        if (curWave != null)
            curWave.tryToSpawn();
        else if (!hasShownPaths && Time.time - lastTimeEndedWave >= TIME_UNTIL_WAVE_ANIM) {
            nextWave = createNewWave();
            StartCoroutine(showPaths(nextWave));
            hasShownPaths = true;
        }
        else if (Time.time - lastTimeEndedWave >= TIME_BETWEEN_WAVES)
        {
            curWave = nextWave;

            int numEnemies = 0;
            foreach (WaveEnemy waveEnemy in curWave.enemiesToSpawn)
            {
                if (waveEnemy.enemy == null)
                    continue;

                numEnemies++;
            }
            //Debug.Log($"{waveNum} - {numEnemies} enemies");

            hasShownPaths = false;
            waveNum++;
        }
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
        float difficultyScoreTotal = curDifficulty = calculateNewDifficultyScore(waveNum);
        //Debug.Log($"Wave start window: {enemyWindow}     diff: {difficultyScoreTotal}");

        Wave newWave = new Wave();
        newWave.enemiesToSpawn = new List<WaveEnemy>();
        int subWaveEnemiesLeft = 0;
        List<int> subwavePath = new List<int>();
        bool canMakeMoreSubwaves = true;

        while (true)
        {
            bool isFreshSubwave = subWaveEnemiesLeft == 0;            

            if (isFreshSubwave && canMakeMoreSubwaves)
            {
                if (waveNum <= 3) //subwaveIsFullWave
                    canMakeMoreSubwaves = false;

                Vector2Int subWaveBounds = calculateSubwaveBounds(waveNum);
                //Debug.Log($"subWaveBounds: ({subWaveBounds.x}, {subWaveBounds.y})");

                if (canMakeMoreSubwaves)
                    subWaveEnemiesLeft = Random.Range(subWaveBounds.x, subWaveBounds.y + 1);
                else
                    subWaveEnemiesLeft = 100; // arbitrarily long number

                subwavePath = NavManager.instance.getPathIndeces();

                newWave.addSubwavePathIndeces(subwavePath);

                //Debug.Log($"subwave len: {subWaveEnemiesLeft}");
            }

            enemyWindow = getNewValidEnemyWindow(enemyWindow, difficultyScoreTotal);
            //Debug.Log($"newly valid window: {enemyWindow}");

            // stop spawning if there are no more valid enemies
            if (enemyWindow.y < enemyWindow.x || enemyWindow.x < 0)
                break;

            Enemy selectedEnemyPrefab = getRandomEnemyFromWindow(enemyWindow);

            difficultyScoreTotal -= selectedEnemyPrefab.getDifficulty();

            newWave.addEnemy(selectedEnemyPrefab, selectedEnemyPrefab.transform.position, subwavePath);

            subWaveEnemiesLeft--;
        }        

        return newWave;
    }

    Enemy getRandomEnemyFromWindow(Vector2Int enemyWindow)
    {
        return allEnemies[Random.Range(enemyWindow.x, enemyWindow.y + 1)];
    }

    Vector2Int getNewValidEnemyWindow(Vector2Int enemyWindow, float difficultyScoreLeft)
    {
        // removes out of bounds extries
        if (enemyWindow.y >= allEnemies.Length)
            enemyWindow.y = allEnemies.Length - 1;
        if (enemyWindow.x < 0)
            enemyWindow.x = 0;

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

        if (min > deterministicMax)
            min = deterministicMax - 1;
        if (min < 0)
            min = 0;

        if (max < min)
            max = min;

        return new Vector2Int(min, max);
    }

    float calculateNewDifficultyScore(int waveNum)
    {
        float extraMultiplier;
        int wavesFromBossWave = (waveNum) % wavesUntilBossWave;

        if (wavesFromBossWave == 0)
            extraMultiplier = difficultyMultiplier / 2;
        else if (wavesFromBossWave == wavesUntilBossWave - 1)
            extraMultiplier = -(difficultyMultiplier - 1);
        else
            extraMultiplier = 0;

        return (Mathf.Pow(difficultyMultiplier, waveNum - 2) * (difficultyMultiplier + extraMultiplier));        
    }

    Vector2Int calculateSubwaveBounds(int waveNum)
    {
        return new Vector2Int(waveNum, waveNum * 2);
    }

    public int getNumEnemies() {
        if (curWave == null)
            return 0;

        return curWave.getNumEnemiesAlive();
    }

    public float getDiff()
    {
        List<Enemy> en = curWave.getEnemiesAlive();

        float sum = 0;

        foreach (Enemy e in en) {
            if (e != null)
                sum += e.getDifficulty();
        }

        return sum;
    }

    public float waveDiff()
    {
        return calculateNewDifficultyScore(waveNum);
    }

    public bool hasMachineEnemies()
    {
        if (curWave == null)
            return false;

        List<Enemy> aliveEnemies = curWave.getEnemiesAlive();

        if (aliveEnemies == null)
            return false;

        foreach (Enemy e in aliveEnemies)
        {
            if (e != null && e.isMachine)
                return true;
        }

        return false;
    }

    IEnumerator showPaths(Wave waveToShow) {
        List<List<NavElement>> paths = new List<List<NavElement>>();
        List<LineRenderer> lineRends = new List<LineRenderer>();
        List<int> pathCurIndex = new List<int>();

        foreach(List<int> indeces in waveToShow.getAllSubwavePathIndeces()) {
            LineRenderer newRend = Instantiate(lineRendPrefab, transform.position, Quaternion.identity);

            paths.Add(NavManager.instance.getPath(newRend.transform, indeces));
                        
            newRend.SetPosition(0, newRend.transform.position);
            newRend.positionCount = 500;

            lineRends.Add(newRend);

            pathCurIndex.Add(0);
        }

        int positionNum = 1;

        while (paths.Count > 0) {
            for (int i = 0; i < paths.Count; i++) {
                LineRenderer curLineRend = lineRends[i];
                int curIndex = pathCurIndex[i];
                NavElement curNavElement = paths[i][curIndex];
                NavInfo info = NavManager.instance.getPathInfo(curLineRend.transform, paths[i], curNavElement, curIndex, waveAnimSpeed);

                // sets the line renderer points
                curLineRend.SetPosition(positionNum, curLineRend.transform.position);

                bool traversedCurNavElement = curNavElement.setNextPos(info);
        
                if (traversedCurNavElement)
                {
                    pathCurIndex[i]++;

                    if (pathCurIndex[i] >= paths[i].Count)
                    {
                        paths.RemoveAt(i);                        
                        pathCurIndex.RemoveAt(i);
                        lineRends.RemoveAt(i);
                        lineRends.Add(curLineRend);
                        i--;
                        continue;
                    }                
                }
            }

            positionNum++;

            yield return new WaitForSeconds(waveAnimCooldown);
        }

        for (int i = lineRends.Count - 1; i >= 0; i--)
        {
            LineRenderer lineRend = lineRends[i];

            lineRend.positionCount = positionNum;
            lineRend.enabled = true;
        }

        yield return new WaitForSeconds(timeAnimOnScreen);

        for (int i = lineRends.Count - 1; i >= 0; i--)
        {
            LineRenderer lineRend = lineRends[i];

            lineRends.RemoveAt(i);
            Destroy(lineRend.gameObject);
        }            
    }
}