using UnityEngine;

public class PlasticInteractable : MonoBehaviour
{
    private bool playerNearby = false;

    void Start()
{
    if (PlayerPrefs.GetInt("Plastic_" + gameObject.name, 0) == 1)
        gameObject.SetActive(false); 
}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered plastic cleanup area");
            playerNearby = true;
            InteractionUI.Instance.ShowPrompt("Press E to clean\nup plastic");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            InteractionUI.Instance.HidePrompt();
        }
    }



    void Update()
{
    if (!playerNearby) return;
    if (UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
    {
        InteractionUI.Instance?.HidePrompt();
        FindObjectOfType<HUDManager>()?.AddPlasticCleaned();
        PlayerPrefs.SetInt("Plastic_" + gameObject.name, 1);
        gameObject.SetActive(false);
    }
}
}