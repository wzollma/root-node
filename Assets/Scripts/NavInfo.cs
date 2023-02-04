using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NavInfo
{
    public Transform enemyTrans;
    public float maxDist;
    public Vector3 endPos;
    public bool movingClockwise;

    public NavInfo(Transform enemyTrans, float maxDist) : this(enemyTrans, maxDist, Vector3.zero, false) { }

    public NavInfo(Transform enemyTrans, float maxDist, Vector3 endPos, bool movingClockwise)
    {
        this.enemyTrans = enemyTrans;
        this.maxDist = maxDist;
        this.endPos = endPos;
        this.movingClockwise = movingClockwise;
    }
}
