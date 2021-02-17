using System;
using UnityEngine;

public class EnemyNavEnd : MonoBehaviour
{
    public static Action<GameObject> EnemyCollision;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyCollision.Invoke(other.gameObject);
        }
    }
}
