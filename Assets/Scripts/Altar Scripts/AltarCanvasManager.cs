using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AltarCanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject altarPanel;
    [SerializeField] private TextMeshProUGUI altarNameText;
    [SerializeField] private TextMeshProUGUI altarPriceText;


    public void ShowAltarInfo(string altarName, int price)
    {
        altarPanel.SetActive(true);
        altarNameText.text = altarName;
        altarPriceText.text = $"{price} Score";
    }

    public void HideAltarInfo()
    {
        altarPanel.SetActive(false);
    }
}
