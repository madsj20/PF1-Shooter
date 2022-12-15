using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace QuickStart
{
    public class EnemySpawner : NetworkBehaviour
    {
        public List<Transform> zombieSpawnpoints = new List<Transform>();
        public GameObject enemy;

        private void Start()
        {
            StartCoroutine(SpawnZombiesTimer());
        }

        public void SpawnZombies()
        {
            int randomRange = Random.Range(0, zombieSpawnpoints.Count);
            int zombieAmount = Random.Range(5, 25);
            for (int i = 0; i < zombieAmount; i++)
            {
                Instantiate(enemy, zombieSpawnpoints[randomRange]);
            }
            StartCoroutine(SpawnZombiesTimer());
        }

        public IEnumerator SpawnZombiesTimer()
        {
            int time = 0;
            time = Random.Range(3, 15);
            yield return new WaitForSeconds(time);
            SpawnZombies();
    }
    }
}
