using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] float startSpeed;
    [SerializeField] float startDamage;
    [SerializeField] float startBaseDamage;

    List<NavNode> path;

    void Start()
    {
        
    }
    
    void Update()
    {
        Move();
    }

    void Move()
    {
        
    }

    void AttackTreeNode()
    {

    }

    void AttackRoot()
    {

    }

    void AttackBase()
    {

    }
}
