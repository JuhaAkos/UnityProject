using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace JA { 
    public class VictoryScreen : MonoBehaviour
    {
        public CanvasGroup victoryScreen;
        private bool fadeInActive = false;
        private bool isLoadedIn = false;
        private float fadeInDelay = 0f;
        private float exitToMenuDelay = 0f;


        //CALL FROM ENEMY DEATHSTATE KEY EVENT
        public void VictoryScreenFadeIn()
        {
            fadeInActive = true;
        }


        public void ReturnToMainMenu()
        {

        }

        private void Start()
        {
            victoryScreen.alpha = 0;
        }

        private void Update()
        {
            if (fadeInDelay > 2 && victoryScreen.alpha < 1)
            {
                victoryScreen.alpha += (Time.deltaTime / 6);
            }

            if (fadeInActive)
            {
                fadeInDelay += (Time.deltaTime / 2);
            }

            if (victoryScreen.alpha >= 1)
            {
                exitToMenuDelay += (Time.deltaTime / 2);
            }     

            if (exitToMenuDelay >= 2)
            {
                SceneManager.LoadScene("MainMenu");
            }  
        }
    }
}