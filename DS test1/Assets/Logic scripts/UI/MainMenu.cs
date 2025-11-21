using UnityEngine;
using UnityEngine.SceneManagement;

namespace JA {   
    public class MainMenu : MonoBehaviour
    {
        public CanvasGroup fadeOutScreen;
        public GameObject fadeOutScreenObj;
        private bool fadedOutActive = false;

        public void PlayGame()
        {
            fadeOutScreenObj.SetActive(true);
            fadedOutActive = true;                     
        }

        public void CloseApplication()
        {
            
            Debug.Log("Closing app...");
            Application.Quit();
        }

        private void Update()
        {
            if (fadedOutActive && fadeOutScreen.alpha < 1)
            {
                fadeOutScreen.alpha += (Time.deltaTime);
            }

            if (fadeOutScreen.alpha >= 1)
            {
                Cursor.visible = false;
                SceneManager.LoadScene("SampleScene");
            }
        }

    }
}