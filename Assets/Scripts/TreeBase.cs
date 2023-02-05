using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class TreeBase : MonoBehaviour
{
    [SerializeField] float startHealth;

    float health;

    void Start()
    {
        health = startHealth;
    }

    void TakeDamage(float damage)
    {
        if (!enabled)
            return;

        health -= damage;

        if (health <= 0)
        {
            // lose the game
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Game Over");
            enabled = false;
        }            
    }

    public void addEnemyToEvent(Enemy enemy)
    {
        enemy.OnDie += TakeDamage;
    }
}
