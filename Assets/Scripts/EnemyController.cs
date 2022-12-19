using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

namespace QuickStart
{
    public class EnemyController : NetworkBehaviour
    {
        private SceneScript sceneScript;
        private ScoreController ScoreController;
        private DBScript DBScript;
        private Timer timer;

        public NavMeshAgent agent;
        public GameObject player;
        public int hp = 2;

        Vector3 playerPos;

        private void Awake()
        {
            sceneScript = GameObject.Find("SceneReference").GetComponent<SceneReference>().sceneScript;
            ScoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
            timer = GameObject.Find("TimerController").GetComponent<Timer>();
            DBScript = GameObject.Find("DBHandler").GetComponent<DBScript>();
        }
        void Start()
        {
            StartCoroutine(ForceEnemyTargetPosition());
        }

        private IEnumerator ForceEnemyTargetPosition()
        {
            float Xmax = 95f;
            float Xmin = -9.75f;
            float Zmax = 49f;
            float Zmin = -49f;
            float xCord = Random.Range(Xmax, Xmin);
            float zCord = Random.Range(Zmax, Zmin);
            SetDestination(new Vector3(xCord, 0, zCord));
            yield return new WaitForSeconds(Random.Range(3f, 7f));
            StartCoroutine(ForceEnemyTargetPosition());
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerPos = other.GetComponent<Transform>().position;
                SetDestination(new Vector3(playerPos.x, 0, playerPos.z));
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                hp -= 1;
                if (hp <= 0)
                {
                    Destroy(gameObject);
                }
                pepepopo(other.gameObject);
                // Show Dictionary
                string scoresString = "";
                foreach (KeyValuePair<string, int> score in ScoreController.scores)
                {
                    scoresString += score.Key + ": " + score.Value + "\n";
                }
                ScoreController.scoresText.text = scoresString;
                Destroy(other.gameObject);
            }
        }

        private void SetDestination(Vector3 destination)
        {
            agent.SetDestination(destination);
        }
        
        [Server]
        public void AddScore(string other)
        {
            ScoreController.scores[other] += 1;
        }
        [Server]
        void pepepopo(GameObject other)
        {
            AddScore(other.gameObject.GetComponent<Bullet>().playerRef);
        }

    }
}
