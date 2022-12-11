using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickStart
{
    public class Leaderboard : MonoBehaviour
    {
        public void LoadMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}