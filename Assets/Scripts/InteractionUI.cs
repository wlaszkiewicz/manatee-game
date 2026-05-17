using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance;
    public TMP_Text promptText;

    void Awake() => Instance = this;

    public void ShowPrompt(string message) => promptText.text = message;
    public void HidePrompt() => promptText.text = "";
}