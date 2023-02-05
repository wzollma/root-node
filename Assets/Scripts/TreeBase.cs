using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreeBase : MonoBehaviour
{
    public static TreeBase instance;

    [SerializeField] float startHealth;

    float health;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        health = startHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!enabled)
            return;

        health -= damage;

        if (health <= 0)
        {
            // lose the game
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Game Over");

            enabled = false;
        }            
    }
}
