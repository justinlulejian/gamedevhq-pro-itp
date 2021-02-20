using System;
using UnityEngine;

public class EnemyNavEnd : MonoBehaviour
{
    public static event Action<GameObject> OnEnemyCollision;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            OnEnemyCollision.Invoke(other.gameObject);
        }
    }
}
