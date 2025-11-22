using UnityEngine;
using UnityEngine.SceneManagement;

namespace JA {   
    public class ControlInfoPanel : MonoBehaviour
    {

        [SerializeField] private CanvasGroup controlPanel;
        [SerializeField] private CanvasGroup mainMenuPanel;

        public void ShowMainMenuPanel()
        {            
            controlPanel.alpha = 0;
            controlPanel.interactable = false;
            mainMenuPanel.alpha = 1;
            mainMenuPanel.interactable = true;
            mainMenuPanel.blocksRaycasts = true;
        }


    }
}