using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    public Text interactionText;
    public GameObject interactionPanel;

    void Start()
    {
        if (interactionPanel != null)
            interactionPanel.SetActive(false);
    }

    public void ShowInteraction(string text)
    {
        if (interactionText != null)
            interactionText.text = text;
        if (interactionPanel != null)
            interactionPanel.SetActive(true);
    }

    public void HideInteraction()
    {
        if (interactionPanel != null)
            interactionPanel.SetActive(false);
    }
}