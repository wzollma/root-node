using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public delegate void OnDieDelegate(float damageToBase);
    public event OnDieDelegate OnDie;
    [SerializeField] float startSpeed;
    [SerializeField] float startDamage;
    [SerializeField] float startBaseDamage;
    [SerializeField] float difficultyScore;
    [SerializeField] float startHealth;

    List<NavElement> path;
    int curPathIndex;

    float curSpeed;
    float curDamage;
    float curBaseDamage;
    float health;

    void Start()
    {
        // sets enemy at beginning of path
        //path = NavManager.instance.getPath(transform);

        curSpeed = startSpeed;
        curDamage = startDamage;
        curBaseDamage = startBaseDamage;
    }
    
    void Update()
    {
        Move();
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

        NavInfo info;
        float maxDist = /*curSpeed*/startSpeed * Time.deltaTime;
        if (curNavElement is NavRing)
        {
            //Debug.Log("is ring");
            NavLine nextNavLine = (path[curPathIndex + 1] as NavLine);
            float prevAngle = NavRing.posModAngle((path[curPathIndex - 1] as NavLine).getAngle());
            float nextAngle = NavRing.posModAngle(nextNavLine.getAngle());
            bool nextGreater = nextAngle > prevAngle;
            float largerAngle = nextGreater ? nextAngle : prevAngle;
            float smallerAngle = nextGreater ? prevAngle : nextAngle;
            bool overHalfApart = Mathf.Abs(nextAngle - prevAngle) > Mathf.PI && Mathf.Abs(smallerAngle + Mathf.PI * 2 - largerAngle) > Mathf.PI;

            float signedAngle = nextAngle - prevAngle;
            float twoPI = Mathf.PI * 2;            
            signedAngle = (signedAngle + Mathf.PI) % twoPI;
            overHalfApart = signedAngle > Mathf.PI;

            info = new NavInfo(transform, maxDist, nextNavLine.getStartPos(), NavRing.shortestDistRadians(prevAngle, nextAngle) < 0);
        }
        else
        {
            //Debug.Log("is line");
            info = new NavInfo(transform, maxDist/*, (curNavElement as NavLine).getEndPos()*/);
        }

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
        OnDie(curBaseDamage);

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

    private void OnDrawGizmos()
    {
        if (!enabled)
            return;

        Gizmos.DrawSphere(transform.position, .2f);
    }
}
