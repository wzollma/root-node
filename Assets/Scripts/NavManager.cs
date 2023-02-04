using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavManager : MonoBehaviour
{
    public static NavManager instance;

    [SerializeField] float spawnCooldown;

    List<NavRing> allNavRings;
    [SerializeField] Enemy enemy;
    float lastTimeSpawned;

    Enemy lastEnemy;
    Camera mainCam;

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

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        Vector3 mouseInput = Input.mousePosition;
        Vector3 mousePos = mainCam.ViewportToWorldPoint(new Vector3(mouseInput.x, mouseInput.y, 1));
        //Debug.Log(mousePos);
        Debug.Log(getShortestDistanceToPath(mousePos));
    }

    /// <summary>
    /// Creates a random path.  Sets enemyTrans at the start of that path.
    /// </summary>
    /// <returns></returns>
    public List</*NavElement*/int> getPathIndeces()
    {
        List<int> indeces = new List<int>();

        // gets a random navLine index from each of the navRings
        foreach (NavRing r in allNavRings)
        {
            indeces.Add(r.getRandNavLineIndex());
        }

        return indeces;
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
        }
        lastEnemy = enemyTrans.GetComponent<Enemy>();

        // sets enemyTrans at the start of the path
        enemyTrans.position = (path[0] as NavLine).getStartPos();

        if (enemy == null)
            enemy = enemyTrans.GetComponent<Enemy>();

        return path;
    }

    public float getShortestDistanceToPath(Vector3 position)
    {
        Vector2 flatPos = flattenVector3(position);

        float curMinDist = float.MaxValue;

        // checks all rings & their navlines
        foreach (NavRing r in allNavRings)
        {
            // check this ring
            float angle = r.posToAngle(position);
            Vector2 projectedPosCircle = flattenVector3(r.angleToPos(angle, r.getRadius()));
            float curDistCircle = Vector2.Distance(projectedPosCircle, flatPos);

            if (curDistCircle < curMinDist)
                curMinDist = curDistCircle;

            // check all NavLines from this ring
            for (int i = 0; i < 20; i++)
            {
                NavLine curNavLine = r.getNavLineByIndex(i);

                if (curNavLine == null)
                    break;

                Vector2 startPos = flattenVector3(curNavLine.getStartPos());
                Vector2 endPos = flattenVector3(curNavLine.getEndPos());

                Vector2 projectedPosLine = getProjectedPointOnLine(startPos, endPos, flatPos);
                float curDistLine = Vector2.Distance(projectedPosLine, position);

                if (curDistLine < curMinDist)
                    curMinDist = curDistLine;
            }
        }

        return curMinDist;
    }

    Vector2 flattenVector3(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }


    /**
    * Get projected point P' of P on line e1.
    * @return projected point p.
*/
    public Vector2 getProjectedPointOnLine(Vector2 v1, Vector2 v2, Vector2 pos)
    {
        // get dot product of e1, e2
        Vector2 e1 = new Vector2(v2.x - v1.x, v2.y - v1.y);
        Vector2 e2 = new Vector2(pos.x - v1.x, pos.y - v1.y);
        double valDp = Vector2.Dot(e1, e2);
        // get length of vectors
        double lenLineE1 = Mathf.Sqrt(e1.x * e1.x + e1.y * e1.y);
        double lenLineE2 = Mathf.Sqrt(e2.x * e2.x + e2.y * e2.y);
        double cos = valDp / (lenLineE1 * lenLineE2);
        // length of v1P'
        double projLenOfLine = cos * lenLineE2;
        Vector2 p = new Vector2((int)(v1.x + (projLenOfLine * e1.x) / lenLineE1),
                            (int)(v1.y + (projLenOfLine * e1.y) / lenLineE1));
        return p;
    }
}
