using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DefenseNodes;

public class Enemy : MonoBehaviour
{
    public delegate void OnDieDelegate(float damageToBase);
    public event OnDieDelegate OnDie;
    [SerializeField] float startSpeed;
    [SerializeField] float startDamage;
    [SerializeField] float startBaseDamage;
    [SerializeField] float difficultyScore;
    [SerializeField] float startHealth;
    [SerializeField] float attackCooldown;

    List<NavElement> path;
    int curPathIndex;

    float curSpeed;
    float curDamage;
    float curBaseDamage;
    float health;
    float lastAttackTime;

    List<Node> treesInRange;

    void Start()
    {
        // sets enemy at beginning of path
        //path = NavManager.instance.getPath(transform);

        treesInRange = new List<Node>();

        curSpeed = startSpeed;
        curDamage = startDamage;
        curBaseDamage = startBaseDamage;
    }
    
    void Update()
    {
        Move();

        AttackTreeNode();
    }

    void Move()
    {
        if (path.Count <= 0)
        {
            Debug.LogError($"enemy ({name}) initialized with an empty path");
            enabled = false;
            return;
        }

        NavElement curNavElement = path[curPathIndex];

        NavInfo info = NavManager.instance.getPathInfo(transform, path, curNavElement, curPathIndex, curSpeed);
        // sets enemyTrans to endLocation if was traversed
        bool traversedCurNavElement = curNavElement.setNextPos(info);
        
        if (traversedCurNavElement)
        {
            curPathIndex++;

            if (curPathIndex >= path.Count)
            {
                AttackBase();
                return;
            }                
        }
    }

    void AttackTreeNode()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Node nodeToAttack = getClosestTree();

            if (nodeToAttack != null)
            {
                nodeToAttack.Damage(curDamage);

                lastAttackTime = Time.time;

                Debug.Log($"enemy ({name}) attacked tree ({nodeToAttack.name})");
            }            
        }        
    }

    void AttackRoot()
    {

    }

    void AttackBase()
    {
        Die();
    }

    void Die()
    {
        //OnDie(curBaseDamage);

        Destroy(gameObject);
    }

    public float getDifficulty()
    {
        return difficultyScore;
    }

    public void takeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
            Die();
    }

    public void setPath(List<int> pathIndeces)
    {
        path = NavManager.instance.getPath(transform, pathIndeces);
    }

    Node getClosestTree()
    {
        float minDist = float.MaxValue;
        Node curClosestTree = null;

        foreach (Node n in treesInRange)
        {
            if (n == null)
                continue;

            Vector3 curTreePos = n.transform.position;
            Vector3 thisPos = transform.position;
            
            curTreePos.y = thisPos.y = 0;

            float curDist = Vector3.Distance(curTreePos, thisPos);

            if (curDist < minDist)
            {
                minDist = curDist;
                curClosestTree = n;
            }
        }

        return curClosestTree;
    }

    void OnTriggerEnter(Collider other)
    {
        treesInRange.Add(other.GetComponent<Node>());
    }

    private void OnTriggerExit(Collider other)
    {
        treesInRange.Remove(other.GetComponent<Node>());
    }

    private void OnDrawGizmos()
    {
        if (!enabled)
            return;

        Gizmos.DrawSphere(transform.position, .2f);
    }
}
