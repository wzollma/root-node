using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavRing : MonoBehaviour
{
    [SerializeField] int numPathsInward;
    [SerializeField] float radius;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public Vector3 getNewPosOnRing(Vector3 curPos, float distanceToTravel)
    {
        float curAngle = posToAngle(curPos);

        float angleToRotate = distanceToTravel / radius;

        return angleToPos(curAngle + angleToRotate);
    }

    float posToAngle(Vector3 pos)
    {
        return Mathf.Atan2(pos.z - transform.position.z, pos.x - transform.position.x);
    }

    Vector3 angleToPos(float angle)
    {
        float x = transform.position.x + radius * Mathf.Cos(angle);
        float z = transform.position.z + radius * Mathf.Sin(angle);

        return new Vector3(x, 0, z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);

        
    }
}
