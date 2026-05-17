using UnityEngine;

public class BorderPrompt : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            InteractionUI.Instance.ShowPrompt(
                "You've reached the edge! Try exploring another direction...");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            InteractionUI.Instance.HidePrompt();
    }
}