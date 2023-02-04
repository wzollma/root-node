using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode : MonoBehaviour
{
    public List<NavNode> neighbors;

    private void OnDrawGizmos()
    {
        foreach (NavNode n in neighbors)
        {
            Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }
}
