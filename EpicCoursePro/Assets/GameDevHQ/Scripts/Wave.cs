using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Scripts
{
    [CreateAssetMenu(fileName = "Wave.asset", menuName = "SpawnManager/Wave")]
    public class Wave : ScriptableObject
    {
        public int amountToSpawn;
        public List<GameObject> enemyTypesToSpawn;
        public List<GameObject> fixedSpawnSequence;
        [Tooltip("Whether the enemy types specified will be spawned randomly. True ignore fixed " +
                 "spawn sequence if specified.")]
        public bool randomSpawnOn;
        [Tooltip("How long the wave will wait before starting.")]
        public float timeBeforeStart = 5f;
    }
}