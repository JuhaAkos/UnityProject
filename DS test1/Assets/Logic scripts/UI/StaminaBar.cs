using UnityEngine;
using UnityEngine.UI;

namespace JA {
    public class StaminaBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        private void Start()
        {
            slider = GetComponent<Slider>();
        }

        public void SetMaxStamina(float maxStamina)
        {
            slider.maxValue = maxStamina;
            slider.value = maxStamina;
        }

        public void SetCurrentStamina(float currentStamina)
        {
            slider.value = currentStamina;
        }
    }
}