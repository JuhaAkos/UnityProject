using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject bossUIPanel;
    [SerializeField] private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void SetBossMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void SetBossCurrentHealth(int currentHealth)
    {
        slider.value = currentHealth;
    }

    public void DisableBossUI()
    {
        if (!bossUIPanel.activeSelf)
        {
            return;
        }
        bossUIPanel.SetActive(false);
    }
    
    public void EnableBossUI()
    {
        if (bossUIPanel.activeSelf)
        {
            return;
        }
        bossUIPanel.SetActive(true);
    }
}
