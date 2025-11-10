using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EstusUICounter : MonoBehaviour
{
    //public Text CounterText;  // public if you want to drag your text object in there manually
    [SerializeField] TMPro.TextMeshProUGUI counterText;

    private void Start()
    {
        //counterText = GetComponent<TMPro.TextMeshProUGUI>();
    }
        
    public void ChangeEstusCounterText(int EstusCount)
    {
        //Debug.Log("ezitt: " + counterText);
        counterText.text = EstusCount.ToString();
    }
}
