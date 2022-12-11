using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickStart
{
    public class Menu : MonoBehaviour
    {
        public void LoadScene()
        {
            SceneManager.LoadScene("GameList");
        }

        public void LoadLeaderboard()
        {
            SceneManager.LoadScene("LeaderBoard");
        }
        public void LoadMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
