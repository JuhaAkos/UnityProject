using UnityEngine;

public class EstusHandler : MonoBehaviour
{
    Animator anim;
    [SerializeField] GameObject lightSource;
    public bool isLightActive;
    [SerializeField] private int healAmount = 60;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        isLightActive = anim.GetBool("isLightActive");        
    }

    // Update is called once per frame
    void Update()
    {
        isLightActive = anim.GetBool("isLightActive");

        if (!isLightActive && lightSource.activeSelf)
        {
            lightSource.SetActive(false);
        }

        if (isLightActive && !lightSource.activeSelf)
        {
            lightSource.SetActive(true);
        }
    }

    public int GetHealAmount()
    {
        return healAmount;
    }

    public  void SetLightActive(bool status)
    {
        isLightActive = status;
        anim.SetBool("isLightActive", status);
        return;
    }
}
