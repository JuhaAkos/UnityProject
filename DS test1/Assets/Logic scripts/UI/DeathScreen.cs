using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace JA { 
    public class DeathScreen : MonoBehaviour
    {
        public CanvasGroup deathScreen;
        public CanvasGroup fadeInScreen;
        private bool fadeInActive;
        private bool yetToExit;
        private float timingDelay = 0f;
        private bool isLoadedIn = false;
        private float fadeInDelay = 0f;


        //CALL FROM TAKEDAMAGE DEATH OPTION
        public void DeathScreenFadeIn()
        {
            fadeInActive = true;
        }


        public void ReturnToMainMenu()
        {

        }

        private void Start()
        {
            fadeInScreen.alpha = 1;
        }

        private void Update()
        {
            if (isLoadedIn)
            {
                if (deathScreen.alpha < 1 && fadeInActive)
                {
                    deathScreen.alpha += (Time.deltaTime / 4);
                }

                if (timingDelay < 3 && fadeInActive)
                {
                    timingDelay += (Time.deltaTime / 2);
                }

                if (timingDelay >= 3)
                {
                    fadeInActive = false;
                    SceneManager.LoadScene("MainMenu");
                }
            }
            else
            {
                if (fadeInDelay > 0.1)
                {
                    fadeInScreen.alpha -= (Time.deltaTime / 3);

                    if (fadeInScreen.alpha == 0)
                    {
                        isLoadedIn = true;
                    }
                }
                else
                {
                    fadeInDelay += (Time.deltaTime / 3);
                }

            }


        }
    }
}