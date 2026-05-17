using UnityEngine;

public class BoatPrompt : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            InteractionUI.Instance.ShowPrompt(
                "A manatee is trapped!\n Try to move the boat to free it!");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            InteractionUI.Instance.HidePrompt();
    }
}