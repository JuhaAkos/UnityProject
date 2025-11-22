using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EstusUICounter : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI counterText;
    [SerializeField] UnityEngine.UI.Image EstusImage;
        
    public void ChangeEstusCounterText(int EstusCount)
    {
        counterText.text = EstusCount.ToString();
    }

    public void ChangeCounterIconColor(){
        //Color32(233,189,122,65);
        EstusImage.color = new Color32(119,135,142,40);
    }
}
