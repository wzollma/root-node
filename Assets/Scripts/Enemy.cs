using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DefenseNodes;

public class Enemy : MonoBehaviour
{
    public event Action OnDestroyed = delegate {  };
    [SerializeField] float startSpeed;
    [SerializeField] float startDamage;
    [SerializeField] float startBaseDamage;
    [SerializeField] float difficultyScore;
    [SerializeField] float startHealth;
    [SerializeField] float attackCooldown;
    [SerializeField] float timeToRotate;
    [SerializeField] float yRotToFaceForward;
    [SerializeField] int moneyToGive;
    public bool isChainsaw;
    public bool isMachine;

    List<NavElement> path;
    int curPathIndex;

    float curSpeed;
    float curDamage;
    float curBaseDamage;
    float health;
    float lastAttackTime;

    List<Node> nodesInRange;

    Quaternion lastDesiredRot;
    float timeStartRotLerp;

    void Start()
    {
        // sets enemy at beginning of path
        //path = NavManager.instance.getPath(transform);

        nodesInRange = new List<Node>();

        curSpeed = startSpeed;
        curDamage = startDamage;
        curBaseDamage = startBaseDamage;
        health = startHealth;
    }
    
    void Update()
    {
        Vector3 prevPos = transform.position;

        Move();

        Quaternion newDesiredRot = getDesiredRotation(prevPos);

        if (!newDesiredRot.Equals(lastDesiredRot)) {
            if (!isRotateOnCooldown())
                timeStartRotLerp = Time.time;  
            
            lastDesiredRot = newDesiredRot;
        }

        transform.rotation = lastDesiredRot;//Quaternion.Lerp(transform.rotation, lastDesiredRot, Mathf.Clamp01((Time.time - timeStartRotLerp) / timeToRotate));

        AttackTreeNode();

        curSpeed = startSpeed;
    }

    public void MultiplySpeedNextMove(float multiply)
    {
        curSpeed *= multiply;
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
        TreeBase.instance.TakeDamage(curBaseDamage);

        Die(false);
    }

    void Die(bool fromTree)
    {
        //OnDie(curBaseDamage);
        if (fromTree)
            NodeSpawner.Singleton.addMoney(moneyToGive);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OnDestroyed.Invoke();
    }

    public float getDifficulty()
    {
        return difficultyScore;
    }

    public void takeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
            Die(true);
    }

    public void setPath(List<int> pathIndeces)
    {
        path = NavManager.instance.getPath(transform, pathIndeces);
    }

    Node getClosestTree()
    {
        float minDist = float.MaxValue;
        Node curClosestTree = null;

        foreach (Node n in nodesInRange)
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

    Quaternion getDesiredRotation(Vector3 prevPos) {
        Quaternion prevRot = transform.rotation;

        transform.LookAt(prevPos);

        transform.Rotate(new Vector3(0, yRotToFaceForward, 0), Space.Self);

        Quaternion newRot = transform.rotation;

        transform.rotation = prevRot;

        return newRot;
    }

    bool isRotateOnCooldown() {
        return Time.time - timeStartRotLerp < timeToRotate;
    }

    void OnTriggerEnter(Collider other)
    {
        Node node = other.GetComponent<Node>();
        nodesInRange.Add(node);
    }

    private void OnTriggerExit(Collider other)
    {
        nodesInRange.Remove(other.GetComponent<Node>());
    }

    private void OnDrawGizmos()
    {
        if (!enabled)
            return;

        Gizmos.DrawSphere(transform.position, .2f);
    }
}
