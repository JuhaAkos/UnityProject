using UnityEngine;
using UnityEngine.SceneManagement;

namespace JA {
    public class MainMenu : MonoBehaviour
    {

        public void PlayGame()
        {
            SceneManager.LoadScene("SampleScene");
        }

        public void CloseApplication()
        {
            Debug.Log("Closing app...");
            Application.Quit();
        }

    }
}