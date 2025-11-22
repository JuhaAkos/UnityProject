using UnityEngine;
using UnityEngine.SceneManagement;

namespace JA {   
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadeOutScreen;
        [SerializeField] private CanvasGroup controlPanel;
        [SerializeField] private CanvasGroup mainMenuPanel;
        [SerializeField] private GameObject fadeOutScreenObj;
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

        public void ShowControlsPanel()
        {            
            controlPanel.alpha = 1;
            controlPanel.interactable = true;
            mainMenuPanel.alpha = 0;
            mainMenuPanel.interactable = false;
            mainMenuPanel.blocksRaycasts = false;
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
                SceneManager.LoadScene("PlayScene");
            }
        }

    }
}