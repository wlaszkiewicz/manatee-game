using UnityEngine;

public class NetPrompt : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            InteractionUI.Instance.ShowPrompt(
                "The fish are trapped!\nTry to free them by ramming the net!");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            InteractionUI.Instance.HidePrompt();
    }
}