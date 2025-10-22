using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    float fadeSpeed = 0.2f;
    public CanvasGroup deathScreen;
    private bool fadeInActive;
    private bool yetToExit;
    private float timingDelay = 0f;


    //CALL FROM DEATH ANIMATION KEY EVENT
    public void DeathScreenFadeIn()
    {
        fadeInActive = true;
    }


    public void ReturnToMainMenu()
    {

    }
    
    private void Update()
    {

        if (deathScreen.alpha < 1 && fadeInActive)
        {
            deathScreen.alpha += (Time.deltaTime / 2);         
        }

        if (timingDelay < 3 && fadeInActive)
        {
            timingDelay += (Time.deltaTime / 2);
        }
        
        if (timingDelay >= 3) {
            fadeInActive = false;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
