using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace QuickStart
{
    public class EnemyController : MonoBehaviour
    {
        private SceneScript sceneScript;
        private ScoreController ScoreController;
        private DBScript DBScript;
        private Timer timer;
        [SerializeField] private PlayerScript playerScript;

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
                Debug.Log("Player Position " + playerPos);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                hp -= 1;
                Destroy(other.gameObject);
                if (hp <= 0)
                {
                    Destroy(gameObject);
                }
                playerScript.AddScore(other.gameObject.GetComponent<Bullet>().playerRef);
                // Show Dictionary
                string scoresString = "";
                foreach (KeyValuePair<string, int> score in ScoreController.scores)
                {
                    scoresString += score.Key + ": " + score.Value + "\n";
                }
                ScoreController.scoresText.text = scoresString;
            }
        }

        private void SetDestination(Vector3 destination)
        {
            agent.SetDestination(destination);
        }
    }
}
