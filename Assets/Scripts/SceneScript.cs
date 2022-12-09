using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace QuickStart
{
    public class SceneScript : NetworkBehaviour
    {
        public SceneReference sceneReference;
        public TMP_Text canvasStatusText;
        public PlayerScript playerScript;
        public TMP_Text canvasAmmoText;
        public TMP_Text respawnCountdownText;
        public Button buttonReady, buttonDeath, buttonRespawn;
        public Camera topViewCamera;

        public bool countdown = false;

        private void Start()
        {
            //Make sure to attach these Buttons in the Inspector
            buttonReady.onClick.AddListener(ButtonReady);
            //you could choose to fully hide the server only buttons from clients, but for this guide we will show them to have less code involved
            buttonDeath.onClick.AddListener(ButtonDeath);
            buttonRespawn.onClick.AddListener(ButtonRespawn);
        }

        private void Update()
        {
            if (playerScript.isDead == true && countdown == false)
            {
                countdown = true;
                StartCoroutine(RespawnTimer());
            }
        }

        // Changes ready state for all players
        [SyncVar(hook = nameof(OnReadyChanged))]
        public int readyStatus = 0;

        void OnReadyChanged(int _Old, int _New)
        {
            //this hook is fired when readyStatus is changed via client cmd or directly by host
            // you dont need to use _New, as the variable readyStatus is changed before hook is called

            if (readyStatus == 1)
            {
                canvasStatusText.text = "Server Says Go!";
                buttonReady.gameObject.SetActive(false);
            }
            // updating our canvas UI
            SetupScene();
        }

        //[ServerCallback]
        public void ButtonReady()
        {
            // you can use the [ServerCallback] tag if server is only ever going to use the function, or do a check inside for  if( isServer ) { }
            if (isServer)
            {
                // optional checks to wait for full team, you can add this in connection joining callbacks, or via UI, upto you.
                //if (NetworkServer.connections.Count > 2)
                //{
                readyStatus = 1;
                //}
                //else
                //{
                //    playerStatusText.text = "Not enough players.";
                //}
            }
            else
            {
                canvasStatusText.text = "Server only feature";
            }
        }

        // For faster prototyping, we will have these as buttons, but eventually they will be in your raycast, or trigger code
        public void ButtonDeath()
        {
            playerScript.CmdPlayerStatus(true);
        }

        public void ButtonRespawn()
        {
            playerScript.CmdPlayerStatus(false);
        }

        //respawn timer
        public IEnumerator RespawnTimer()
        {
            if (countdown == true)
            {
                
                respawnCountdownText.gameObject.SetActive(true);
                respawnCountdownText.text = "Respawn in: 3";
                Debug.Log("Respawn in: 3");
                yield return new WaitForSeconds(1);
                respawnCountdownText.text = "Respawn in: 2";
                Debug.Log("Respawn in: 2");
                yield return new WaitForSeconds(1);
                respawnCountdownText.text = "Respawn in: 1";
                Debug.Log("Respawn in: 1");
                yield return new WaitForSeconds(1);
                respawnCountdownText.gameObject.SetActive(false);
                playerScript.CmdPlayerStatus(false);
                countdown = false;
            }
        }

        public void SetupScene()
        {
            if (isServer == false)
            {
                buttonReady.interactable = false;
            }

            if (readyStatus == 0)
            {
                buttonRespawn.interactable = false;
                buttonDeath.interactable = false;
            }
            else if (playerScript)
            {
                // quick check to make sure playerScript is set before checking its variables to prevent errors
                if (playerScript.isDead == true)
                {
                    buttonRespawn.interactable = true;
                    buttonDeath.interactable = false;
                }
                else if (playerScript.isDead == false)
                {
                    buttonRespawn.interactable = false;
                    buttonDeath.interactable = true;
                }
            }
        }

        [SyncVar(hook = nameof(OnStatusTextChanged))]
        public string statusText;

        void OnStatusTextChanged(string _Old, string _New)
        {
            //called from sync var hook, to update info on screen for all players
            canvasStatusText.text = statusText;
        }

        public void ButtonSendMessage()
        {
            if (playerScript != null)
                playerScript.CmdSendPlayerMessage();
        }

        public void ButtonChangeScene()
        {
            if (isServer)
            {
                Scene scene = SceneManager.GetActiveScene();
                if (scene.name == "Main")
                {
                    NetworkManager.singleton.ServerChangeScene("MyOtherScene");
                }
                else
                {
                    NetworkManager.singleton.ServerChangeScene("Main");
                }
            }
            else
            {
                Debug.Log("You are not Host.");
            }
        }

        public void UIAmmo(int _value)
        {
            canvasAmmoText.text = "Ammo: " + _value;
        }
    }
}
