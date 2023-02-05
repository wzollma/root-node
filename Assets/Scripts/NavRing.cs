using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavRing : MonoBehaviour, NavElement
{
    [SerializeField] int numPathsInward;
    [SerializeField] float pathOffset; // input as degrees, then internally used as radians
    [SerializeField] float radius;

    NavRing ringInside;

    List<NavLine> navLines;

    void Awake()
    {
        pathOffset = pathOffset * Mathf.Deg2Rad;

        float radiansPerPath = 2 * Mathf.PI / numPathsInward;

        if (navLines == null || navLines.Count != numPathsInward)
        {
            navLines = new List<NavLine>();
            for (int i = 0; i < numPathsInward; i++)
            {
                float curAngle = i * radiansPerPath + pathOffset;                
                navLines.Add(new NavLine(angleToPos(curAngle, radius), angleToPos(curAngle, radius - getPathLength()), curAngle));
            }
        }
    }

    // returns true if reached the start of the desired path (finished traveling on ring)
    public bool setNextPos(NavInfo navInfo)
    {
        Vector3 prePos = navInfo.enemyTrans.position;

        float preAngle = posModAngle(posToAngle(prePos));
        float targetAngle = posModAngle(posToAngle(navInfo.endPos));
        float angleToRotate = navInfo.maxDist / radius;        
        if (navInfo.movingClockwise)
            angleToRotate *= -1;

        float newAngle = posModAngle(preAngle + angleToRotate);
        bool reachedDestination = Mathf.Abs(shortestDistRadians(newAngle, targetAngle)) > Mathf.Abs(shortestDistRadians(preAngle, targetAngle));
        if (reachedDestination)
        {
            newAngle = targetAngle;//navInfo.enemyTrans.position = navInfo.endPos;
        }            

        Vector3 newPosNoY = angleToPos(newAngle, radius);
        navInfo.enemyTrans.position = newPosNoY + Vector3.up * prePos.y;        


        return reachedDestination;
    }

    public float posToAngle(Vector3 pos)
    {
        return Mathf.Atan2(pos.z - transform.position.z, pos.x - transform.position.x);
    }

    public Vector3 angleToPos(float angle, float radius)
    {
        float x = transform.position.x + radius * Mathf.Cos(angle);
        float z = transform.position.z + radius * Mathf.Sin(angle);

        return new Vector3(x, 0, z);
    }

    public static float posModAngle(float angle)
    {
        return (angle + Mathf.PI * 2) % (Mathf.PI * 2);
    }

    public float positiveModAngle(float angle)
    {
        return NavRing.posModAngle(angle);
    }

    public static float shortestDistRadians(float start, float stop)
    {
        float twoPI = Mathf.PI * 2;
        float modDiff = (stop - start) % twoPI;
        float shortestDistance = Mathf.PI - Mathf.Abs(Mathf.Abs(modDiff) - Mathf.PI);
        return (modDiff + twoPI) % twoPI < Mathf.PI ? shortestDistance *= 1 : shortestDistance *= -1;
    }    

    public float getPathLength()
    {
        if (ringInside == null)
            return 0;

        return radius - ringInside.radius;
    }

    public int getRandNavLineIndex()
    {
        return Random.Range(0, navLines.Count);
    }

    public NavLine getNavLineByIndex(int index)
    {
        if (index < 0 || index >= navLines.Count)
            return null;

        return navLines[index];
    }

    public void setInnerRing(NavRing innerRing)
    {
        ringInside = innerRing;
    }

    public float getRadius()
    {
        return radius;
    }

    #if UNITY_EDITOR
    private const bool debug = false;
    private void OnDrawGizmos()
    {
        if (!debug)
            return;
        
        //Gizmos.DrawWireSphere(transform.position, radius);
        
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius);

        if (navLines != null)
        {
            foreach (NavLine p in navLines)
                Gizmos.DrawLine(p.getStartPos(), p.getEndPos());
        }
    }
    #endif
}
