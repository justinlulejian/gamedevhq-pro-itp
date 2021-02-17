using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNavEnd : MonoBehaviour
{
    public static Action<GameObject> enemyCollision;
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyCollision.Invoke(other.gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyCollision.Invoke(other.gameObject);
        }
    }
}
