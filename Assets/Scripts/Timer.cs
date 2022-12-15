using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Mirror;

namespace QuickStart
{
    public class Timer : NetworkBehaviour
    {
        public PlayerScript playerScript;
        public SceneScript sceneScript;
        public ScoreController scoreController;
        public DBScript DBScript;
        public float timeRemaining = 180;

        [SyncVar(hook = nameof(OnBoolChanged))]
        public bool timerIsRunning = false;

        public TMP_Text timeText;
        public TMP_Text winnerText;
        private void Start()
        {
            // Starts the timer automatically
            timerIsRunning = true;
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        }
        void Update()
        {
            RunTimer();
        }
        [Server]
        void RunTimer()
        {
            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    DisplayTime(timeRemaining);
                }
                else
                {
                    Debug.Log("Time has run out!");
                    timeRemaining = 0;
                    timerIsRunning = false;
                    sceneScript.readyStatus = 0;
                }
            }
        }
        void OnBoolChanged(bool _Old, bool _New)
        {
            if (timerIsRunning == false && sceneScript.readyStatus == 1)
            {
                //Show winner text
                winnerText.gameObject.SetActive(true);
                //finds highest score and corrosponding player
                var maxValue = scoreController.scores.Values.Max();
                var maxValueKey = scoreController.scores.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                //Display winner in console and on screen
                winnerText.text = "Winner: " + maxValueKey + " - Score: " + maxValue;
                Debug.Log("The winner is: " + maxValueKey + " with a score of " + maxValue);
                Debug.Log("Run ShowWinner in Timer");
                RunShowWinner();
            }
        }
        [ClientRpc]
        void RunShowWinner()
        {
            StartCoroutine(ShowWinner());
        }

        //Coroutines can only be run locally, so it has to be called using ClientRpc
        public IEnumerator ShowWinner()
        {
            Debug.Log("ShowWinner running");
            
            yield return new WaitForSeconds(3f);
            GoToLeaderboard();
        }

        [Command(requiresAuthority = false)]
        void GoToLeaderboard()
        {
            //Adds highest score to Database
            var maxValue = scoreController.scores.Values.Max();
            var maxValueKey = scoreController.scores.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            DBScript.AddScore(maxValueKey, maxValue);
            //Go to Leaderboard
            NetworkManager.singleton.ServerChangeScene("LeaderBoard");
        }
        [ClientRpc]
        void DisplayTime(float timeToDisplay)
        {
            timeToDisplay += 1;
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}