using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavManager : MonoBehaviour
{
    public static NavManager instance;

    [SerializeField] float spawnCooldown;

    List<NavRing> allNavRings;
    Enemy enemy;
    float lastTimeSpawned;

    Enemy lastEnemy;

    private void Awake()
    {
        instance = this;

        // stores all child navRings
        allNavRings = new List<NavRing>();
        allNavRings.AddRange(GetComponentsInChildren<NavRing>());

        // sets all outRings to have the correct innerRing
        for (int i = 0; i < allNavRings.Count - 1; i++)
        {
            allNavRings[i].setInnerRing(allNavRings[i + 1]);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (!lastEnemy.enabled)
        {
            Instantiate(enemy, transform.position, Quaternion.identity).enabled = true;
            lastTimeSpawned = Time.time;
        }
    }

    /// <summary>
    /// Creates a random path.  Sets enemyTrans at the start of that path.
    /// </summary>
    /// <returns></returns>
    public List<NavElement> getPath(Transform enemyTrans)
    {
        List<int> indeces = new List<int>();

        // gets a random navLine index from each of the navRings
        foreach (NavRing r in allNavRings)
            indeces.Add(r.getRandNavLineIndex());

        return getPath(enemyTrans, indeces);
    }

    /// <summary>
    /// Creates a path based on the indeces "seed".  Sets enemyTrans at the start of that path.
    /// </summary>
    /// <param name="indeces">NavLine indeces corresponding to each NavRing, in progressively inward order</param>
    /// <returns></returns>
    public List<NavElement> getPath(Transform enemyTrans, List<int> indeces)
    {
        List<NavElement> path = new List<NavElement>();

        // don't attempt to create a path that will not be the right size
        if (indeces.Count != allNavRings.Count)
        {
            Debug.LogError($"getPathByIndeces() mismatch between indeces and allNavRings lengths ({indeces.Count} != {allNavRings.Count})");
            return path;
        }        

        // assumes order goes line-ring- ... -line
        for (int i = 0; i < allNavRings.Count - 1; i++)
        {
            NavRing curRing = allNavRings[i];

            // don't add first ring because enemies will start on NavLines
            if (i != 0)
            {
                path.Add(curRing);
            }

            // adds navLine coming from ring
            path.Add(curRing.getNavLineByIndex(indeces[0]));

            // allows sequential reading of indeces
            indeces.RemoveAt(0);
        }

        for (int i = 0; i < path.Count - 1; i++)
        {
            NavElement e = path[i];

            if (!(e is NavRing))
                continue;

            NavLine p = path[i - 1] as NavLine;
            NavLine n = path[i + 1] as NavLine;

            Debug.Log($"p: {NavRing.posModAngle(p.getAngle())}    n: {NavRing.posModAngle(n.getAngle())}     c: {Mathf.Abs(NavRing.posModAngle(n.getAngle()) - NavRing.posModAngle(p.getAngle())) > Mathf.PI}");
        }
        lastEnemy = enemyTrans.GetComponent<Enemy>();

        // sets enemyTrans at the start of the path
        enemyTrans.position = (path[0] as NavLine).getStartPos();

        if (enemy == null)
            enemy = enemyTrans.GetComponent<Enemy>();

        return path;
    }
}
