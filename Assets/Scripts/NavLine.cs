using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NavLine : NavElement
{
    Vector3 startPos;
    Vector3 endPos;
    float angle;

    public NavLine(Vector3 start, Vector3 end, float angle)
    {
        startPos = start;
        endPos = end;
        this.angle = angle;
    }

    // returns true if reached the end of the NavLine
    public bool setNextPos(NavInfo info)
    {
        bool reachedEnd = Vector3.Distance(info.enemyTrans.position - Vector3.up * info.enemyTrans.position.y, endPos) <= info.maxDist;

        // sets enemyTrans position accordingly
        info.enemyTrans.position = reachedEnd ? endPos : Vector3.MoveTowards(info.enemyTrans.position, endPos, info.maxDist);

        return reachedEnd;
    }

    public Vector3 getStartPos()
    {
        return startPos;
    }

    public Vector3 getEndPos()
    {
        return endPos;
    }

    public float getAngle()
    {
        return angle;
    }
}
