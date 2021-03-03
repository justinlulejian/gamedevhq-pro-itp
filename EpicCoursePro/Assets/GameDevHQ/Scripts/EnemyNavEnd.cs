using System;
using UnityEngine;

public class EnemyNavEnd : MonoBehaviour
{
    public static event Action<GameObject> onEnemyCollision;

    private void OnTriggerEnter(Collider other)
    {
        // TODO(improvement): Make this more dynamic.
        if (other.gameObject.CompareTag("Enemy"))
        {
            onEnemyCollision?.Invoke(other.gameObject);
        }
    }
}
