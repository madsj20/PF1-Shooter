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
